using System.Net;
using ServiceStack;
using ServiceStack.Text;
using WebCasosSiapp.Concretes.Contexts;
using WebCasosSiapp.Concretes.Functions;
using WebCasosSiapp.Interfaces;
using WebCasosSiapp.Models.PRO;
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
            var fechaUnix = DateTime.Now.ToUnixTime();
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

                //buscar la ultima actividad
                ActividadVersiones act =
                    _context.ActividadVersiones.FirstOrDefault(versiones =>
                        versiones.VersionProcesoId == verPro.Id && versiones.Activo == true);

                if (act != null)
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

                    int resCliente = _context.SaveChanges();
                    if (resCliente > 0)
                    {
                        // Crear Paso
                        Paso paso = new Paso();
                        paso.Id = Generals.GetUlid();
                        paso.CasoId = caso.Id;
                        paso.ActividadVersionId = act.Id;

                        _context.Paso.Add(paso);

                        //Crear Estado Paso
                        EstadoPaso estadoPaso = new EstadoPaso();
                        estadoPaso.Id = Generals.GetUlid();
                        estadoPaso.PasoId = paso.Id;
                        estadoPaso.Estado = "Nuevo";
                        estadoPaso.FechaCreacion = DateTime.Now;
                        estadoPaso.AsignadoPor = "SIAPP";

                        _context.EstadoPaso.Add(estadoPaso);

                        //Crear Responsable
                        Responsable responsable = new Responsable();
                        responsable.Id = Generals.GetUlid();
                        responsable.PasoId = paso.Id;
                        responsable.UsuarioId = request.Responsable.UsuarioId;
                        responsable.FechaCreacion = DateTime.Now;
                        responsable.AsignadoPor = request.Responsable.AsignadoPor;

                        _context.Responsable.Add(responsable);

                        int res = _context.SaveChanges();
                        if (res == 3)
                        {
                            _context.Paso.ToList();
                            _context.CasoCliente.ToList();
                            _context.EstadoPaso.ToList();
                            _context.Responsable.ToList();
                            _context.PersonasNaturales.ToList();
                            return new HttpResult(caso, HttpStatusCode.OK);
                        }

                        return new HttpError(HttpStatusCode.BadRequest, "Error al guardar caso. ");
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
}