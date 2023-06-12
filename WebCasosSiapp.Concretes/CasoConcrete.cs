using System.Net;
using ServiceStack;
using ServiceStack.Text;
using WebCasosSiapp.Concretes.Contexts;
using WebCasosSiapp.Concretes.Functions;
using WebCasosSiapp.Interfaces;
using WebCasosSiapp.Models.PRO;
using WebCasosSiapp.Models.SIS;
using WebCasosSiapp.ViewModels.Requests;

namespace WebCasosSiapp.Concretes;

public class CasoConcrete : ICaso
{
    private readonly DatabaseContext _context;

    public CasoConcrete(DatabaseContext context)
    {
        _context = context;
    }

    public object NuevoCaso(NuevoCasoRequest request)
    {
        Caso caso = new Caso();
        try
        {
            //agregar caso
            var fechaUnix = DateTime.Now.ToUnixTimeMs();
            caso.Id = Generals.GetUlid();
            caso.Codigo = fechaUnix.ToString("x8"); //Convertida a Hexadecimal
            caso.FechaCreacion = DateTime.Now;
            caso.Abierto = true;
            caso.CasoAsociado = null;
            caso.ComentarioApertura = request.ComentarioApertura;
            _context.Caso.Add(caso);
            if (_context.SaveChanges() == 1)
            {
                //Buscar el id de la version activa del proceso en VersionProceso
                VersionProcesos verPro =
                    _context.VersionProcesos.FirstOrDefault(v => v.ProcesoId == request.ProcesoId && v.Activo == true);

                if (verPro == null)
                    return new HttpError(HttpStatusCode.BadRequest, "No existe versión activa para este proceso. ");

                //buscar la primera actividad
                ActividadVersiones act =
                    _context.ActividadVersiones.FirstOrDefault(versiones =>
                        versiones.VersionProcesoId == verPro.Id && versiones.Activo == true && versiones.TipoActividad == "Inicial");

                if (act != null)
                {
                    if (request.Clientes == null)
                    {
                        //hacer busqueda por el usuario
                        var personaEmpleado = _context.Empleados
                            .FirstOrDefault(emp => emp.CodigoUsuario == request.Responsable.UsuarioId);
                        if (personaEmpleado == null)
                        {
                            return new HttpError(HttpStatusCode.BadRequest, "Error al ingresar el cliente. ");
                        }
                        
                        CasoCliente casoCliente = new CasoCliente();
                        casoCliente.Id = Generals.GetUlid();
                        casoCliente.CasoId = caso.Id;
                        casoCliente.ClienteId = personaEmpleado.CodigoPersona;
                        _context.CasoCliente.Add(casoCliente);
                    }
                    else
                    {
                        // Agregar caso Cliente
                        foreach (var cliente in request.Clientes)
                        {
                            CasoCliente casoCliente = new CasoCliente();
                            casoCliente.Id = Generals.GetUlid();
                            casoCliente.CasoId = caso.Id;
                            casoCliente.ClienteId = cliente;
                            _context.CasoCliente.Add(casoCliente);
                        }
                    }
                    
                    int resCliente = _context.SaveChanges();
                    if (resCliente > 0)
                    {
                        NuevoPasoRequest nuevoPasoRequest = new NuevoPasoRequest();
                        nuevoPasoRequest.Caso = caso;
                        nuevoPasoRequest.ActividadVersionId = act.Id;
                        nuevoPasoRequest.TipoMovimiento = "Monousuario";
                        nuevoPasoRequest.Responsable = request.Responsable;
                        
                        // Crear Paso
                        Paso paso = CrearPaso(nuevoPasoRequest);

                        return caso != null ? 
                            new HttpResult(caso, HttpStatusCode.OK) : 
                            new HttpError(HttpStatusCode.BadRequest, "Error al guardar caso. ");
                    }

                    return new HttpError(HttpStatusCode.BadRequest, "Error al ingresar los clientes. ");
                }

                return new HttpError(HttpStatusCode.BadRequest, "No se encontró actividad para este proceso. ");
            }

            return new HttpError(HttpStatusCode.BadRequest,
                "Error en la petición. ");
        }
        catch (Exception ex)
        {
            return new HttpError(HttpStatusCode.BadRequest,
                "Error en la petición: " + ex.Message);
        }

    }

    public object ObtenerCaso(string id)
    {
        var caso = _context.Caso.Where(c => c.Id == id).ToList();
        _context.Paso.ToList();
        _context.CasoCliente.ToList();
        _context.EstadoPaso.ToList();
        _context.Responsable.ToList();
        _context.PersonasNaturales.ToList();
        return caso;
    }

    public Paso CrearPaso(NuevoPasoRequest request)
    {
        // Crear Paso
        Paso paso = new Paso();
        paso.Id = Generals.GetUlid();
        paso.CasoId = request.Caso.Id;
        paso.ActividadVersionId = request.ActividadVersionId;
        _context.Paso.Add(paso);

        if (_context.SaveChanges() == 1)
        {
            //Crear Estado Paso
            EstadoPaso estadoPaso = new EstadoPaso();
            estadoPaso.Id = Generals.GetUlid();
            estadoPaso.PasoId = paso.Id;
            estadoPaso.Estado = request.TipoMovimiento == "Autoservicio" ? "Grupo" : "Nuevo";
            estadoPaso.FechaCreacion = DateTime.Now;
            estadoPaso.AsignadoPor = "SIAPP";
            _context.EstadoPaso.Add(estadoPaso);

            if (request.TipoMovimiento != "Autoservicio") // Si es autoservicio no se agrega responsable
            {
                //Crear Responsable
                Responsable responsable = new Responsable();
                responsable.Id = Generals.GetUlid();
                responsable.PasoId = paso.Id;
                responsable.FechaCreacion = DateTime.Now;

                if (request.TipoMovimiento == "Ciclico")
                {
                    Ciclo ciclo = AsignacionCiclica(request.ActividadVersionId);
                    if (ciclo != null)
                    {
                        responsable.UsuarioId = ciclo.UsuarioId;
                        responsable.AsignadoPor = "SIAPP";
                    }
                }
                else
                { // Asignacion Manual o Monousuario
                    responsable.UsuarioId = request.Responsable.UsuarioId;
                    responsable.AsignadoPor = request.Responsable.AsignadoPor;
                }
                
                _context.Responsable.Add(responsable);
            }
            
            int res = _context.SaveChanges();
            if (res > 1)
            {
                _context.CasoCliente.ToList();
                _context.EstadoPaso.ToList();
                _context.Responsable.ToList();
                _context.PersonasNaturales.ToList();
                return paso;
            }
        }
        return null;
    }

    public object FijarProcesoUsuario(string ProcesoId, string UsuarioId)
    {
        try
        {
            ProcesoFijoUsuario procesoFijo = new ProcesoFijoUsuario();
            procesoFijo.Id = Generals.GetUlid();
            procesoFijo.ProcesoId = ProcesoId;
            procesoFijo.UsuarioId = UsuarioId;
            _context.ProcesoFijoUsuario.Add(procesoFijo);
            if (_context.SaveChanges() == 1)
            {
                return new HttpResult(procesoFijo, HttpStatusCode.OK);
            }
            return new HttpError(HttpStatusCode.BadRequest, "Error al fijar proceso. ");
        }
        catch (Exception ex)
        {
            return new HttpError(HttpStatusCode.BadRequest,
                "Error en la petición: " + ex.Message);
        }
    }

    public object EliminarProcesoFijoUsuario(string ProcesoId, string UsuarioId)
    {
        try
        {
            ProcesoFijoUsuario procesoFijoEli = new ProcesoFijoUsuario();
            procesoFijoEli = _context.ProcesoFijoUsuario.FirstOrDefault(prFijo =>
                prFijo.ProcesoId == ProcesoId && prFijo.UsuarioId == UsuarioId);
            _context.ProcesoFijoUsuario.Remove(procesoFijoEli);
            
            if (_context.SaveChanges() == 1)
            {
                return new HttpResult(procesoFijoEli, HttpStatusCode.OK);
            }

            return new HttpError(HttpStatusCode.BadRequest, "Error al eliminar proceso fijado. ");
        }
        catch (Exception ex)
        {
            return new HttpError(HttpStatusCode.BadRequest,
                "Error en la petición: " + ex.Message);
        }
    }

    public Ciclo AsignacionCiclica(string ActividadVersionId)
    {
        try
        {
            //Obtenemos los perfiles de la actividad version
            List<UsuarioPerfil> usuariosList = new List<UsuarioPerfil>();
            List<ActividadVersionPerfil> perfiles = _context.ActividadVersionPerfil
                .Where(av => av.ActividadVersionId == ActividadVersionId && av.Rol == "responsable" ).ToList();
            
            foreach (var perfil in perfiles)
            { // Obtenemos los usuarios de los perfiles seleccionados
                var usuariosPerfil =
                    _context.UserProfiles.Where(usuarP => usuarP.CodigoPerfil == perfil.PerfilId).ToList();
                var usuarios = _context.Usuarios.ToList();
                usuariosList.AddRange(usuariosPerfil);
            }
            
            usuariosList = usuariosList.DistinctBy(usu => usu.CodigoUsuario).ToList(); // Eliminamos usuarios repetidos por tener ambos perfiles
            usuariosList = usuariosList.OrderBy(usu => usu.Usuario.ApellidosUsuario).ToList(); // Ordenar por apellidos

            //Obtenemos los ciclos que nada más sean true (iniciales)
            List<Ciclo> ultimoCicloIniciado = _context.Ciclo.Where(c => c.ActividadVersionId == ActividadVersionId && c.Nuevo == true)
                .OrderByDescending(c => c.CreatedAt).ToList();

            //Creamos valores iniciales del ciclo a insertar
            Ciclo ciclo = new Ciclo();
            ciclo.Id = Generals.GetUlid();
            ciclo.ActividadVersionId = ActividadVersionId;
            ciclo.CreatedAt = DateTime.Now;
            
            if (ultimoCicloIniciado.Count == 0) // Si no existe ni siquiera un ciclo iniciado, se inicia el primero
            { // Se inicia nuevo ciclo
                ciclo.UsuarioId = usuariosList.First().CodigoUsuario;
                ciclo.Nuevo = true;
                _context.Ciclo.Add(ciclo);
                if (_context.SaveChanges() == 1)
                {
                    return ciclo;
                }
            }
            else
            {
                // Obtenemos los usuarios agregados al ultimo ciclo que tiene true
                List<Ciclo> ciclosActual = _context.Ciclo
                    .Where(c => c.ActividadVersionId == ActividadVersionId && c.CreatedAt >= ultimoCicloIniciado[0].CreatedAt ).ToList();
                
                // empezamos a recorrer los datos del ciclo con los usuarios disponibles
                foreach (var usuario in usuariosList)
                {
                    var existeEnCiclo = ciclosActual.FirstOrDefault(c => c.UsuarioId == usuario.CodigoUsuario);
                    if (existeEnCiclo == null) // Si no existe en el ciclo actual lo inserta
                    {
                        ciclo.UsuarioId = usuario.CodigoUsuario;
                        ciclo.Nuevo = false;

                        _context.Ciclo.Add(ciclo);
                        if (_context.SaveChanges() == 1)
                        {
                            return ciclo;
                        }
                    }
                }
                
                // si salimos del ciclo entonces creamos nuevamente el ciclo
                ciclo.UsuarioId = usuariosList.First().CodigoUsuario;
                ciclo.Nuevo = true;
                _context.Ciclo.Add(ciclo);
                if (_context.SaveChanges() == 1)
                {
                    return ciclo;
                }
            }
            
            return null; 
        }
        catch (Exception ex)
        {
            return null;
        }
    }
}