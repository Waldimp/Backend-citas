using System.Net;
using Microsoft.AspNetCore.SignalR;
using ServiceStack;
using ServiceStack.Text;
using WebCasosSiapp.Concretes.Contexts;
using WebCasosSiapp.Concretes.Functions;
using WebCasosSiapp.Interfaces;
using WebCasosSiapp.Models.PRO;
using WebCasosSiapp.Models.SIS;
using WebCasosSiapp.ViewModels.Requests;
using WebCasosSiapp.ViewModels.Responses;

namespace WebCasosSiapp.Concretes;

public class CasoConcrete : ICaso
{
    private readonly DatabaseContext _context;

    public CasoConcrete(DatabaseContext context)
    {
        _context = context;
    }

    public SignalResponse NuevoCaso(NuevoCasoRequest request)
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
                    return new SignalResponse
                    {
                        Response = new HttpError(HttpStatusCode.BadRequest,
                            "No existe versión activa para este proceso. "),
                        Responsables = null,
                        VersionId = null
                    };

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
                            return new SignalResponse
                            {
                                Response = new HttpError(HttpStatusCode.BadRequest, "Error al ingresar el cliente. "),
                                Responsables = null,
                                VersionId = null
                            };
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
                        nuevoPasoRequest.TipoSeleccion = "monousuario";
                        nuevoPasoRequest.Responsable = request.Responsable;
                        
                        // Crear Paso
                        Paso paso = CrearPaso(nuevoPasoRequest);

                        return caso != null
                            ? new SignalResponse
                            {
                                Response = new HttpResult(caso, HttpStatusCode.OK), Responsables = paso.Responsables,
                                VersionId = verPro.Id
                            }
                            : new SignalResponse
                            {
                                Response = new HttpError(HttpStatusCode.BadRequest, "Error al guardar caso. "),
                                Responsables = null,
                                VersionId = null
                            };
                    }

                    return new SignalResponse
                    {
                        Response = new HttpError(HttpStatusCode.BadRequest, "Error al ingresar los clientes. "),
                        Responsables = null,
                        VersionId = null
                    };
                }

                return new SignalResponse
                {
                    Response = new HttpError(HttpStatusCode.BadRequest, "No se encontró actividad para este proceso. "),
                    Responsables = null,
                    VersionId = null
                };
            }

            return new SignalResponse
            {
                Response = new HttpError(HttpStatusCode.BadRequest,
                    "Error en la petición. "),
                Responsables = null,
                VersionId = null
            };
        }
        catch (Exception ex)
        {
            return new SignalResponse
            {
                Response = new HttpError(HttpStatusCode.BadRequest,
                    "Error en la petición: " + ex.Message),
                Responsables = null
            };
        }

    }

    public object ObtenerCaso(string id)
    {
        try
        {
            Caso caso = _context.Caso.First(c => c.Id == id);
            _context.ActividadVersiones.ToList();
            _context.Actividades.ToList();
            _context.Paso.ToList();
            //_context.CasoCliente.ToList();
            _context.EstadoPaso.ToList();
            _context.Responsable.ToList();
            _context.Secciones.ToList();
            _context.Registro.ToList();
            List<Finalizacion> final = new List<Finalizacion>();
            
            foreach (var paso in caso.Pasos)
            {
                if (caso.Abierto == false)
                {
                    final.AddRange(_context.Finalizacion.Where(f => f.PasoId == paso.Id).ToList());
                }
                
                foreach (var seccion in paso.ActividadVersion.Secciones)
                {
                    seccion.Registros = null;
                }

                if (paso.Responsables != null)
                {
                    foreach (var responsable in paso.Responsables)
                    {
                        responsable.Usuario =
                            _context.Usuarios.Where(u => u.CodigoUsuario == responsable.UsuarioId).Select(u => new Usuarios()
                            {
                              CodigoUsuario = u.CodigoUsuario,
                              NombresUsuario = u.NombresUsuario,
                              ApellidosUsuario = u.ApellidosUsuario
                            }).Single();
                    }
                }
            }
            //_context.PersonasNaturales.ToList();
            
            ProcesoResumen? proceso = null;
            if (caso.Pasos.Count > 0)
            {
                proceso = _context.Procesos.Where(p =>
                    p.Id == _context.VersionProcesos.Where(vp => vp.Id == caso.Pasos[0].ActividadVersion.VersionProcesoId)
                        .Select(vp => vp.ProcesoId).Single()).Select(p => new ProcesoResumen
                {
                    Codigo = p.Codigo,
                    Id = p.Id,
                    Interno = p.Interno,
                    Nombre = p.Nombre,
                    Unidad = _context.Unidades.Where(u => u.CodigoUnidades == p.CodigoUnidad).Select(u => u.Unidades)
                        .Single(),
                    Version = _context.VersionProcesos.Where(v => v.Id == caso.Pasos[0].ActividadVersion.VersionProcesoId)
                        .Select(v => v.NumeroVersion).Single()
                }).Single();
                
            }
            
            var clientes = _context.CasoCliente.Join(_context.PersonasNaturales, cc => cc.ClienteId,
                pn => pn.CodigoPersona, (cc, p) => new ClienteResumen
                {
                    Apellidos = p.Apellido1 + (p.ApellidoCasada != null && p.ApellidoCasada != "-"
                        ? " " + p.ApellidoCasada
                        : p.Apellido2 != null && p.Apellido2 != "-"
                            ? " " + p.Apellido2
                            : ""),
                    Nombres = p.Nombre1 + (p.Nombre2 != null && p.Nombre2 != "-" ? " " + p.Nombre2 : "") +
                              (p.Nombre3 != null && p.Nombre3 != "-" ? " " + p.Nombre3 : ""),
                    Documento = p.CodigoNumeroDui,
                    Foto = p.Foto,
                    CodigoPersona = p.CodigoPersona,
                    FechaNacimiento = p.FechaNacimiento,
                    Sexo = p.Sexo, CasoId = cc.CasoId
                }).Where(c => c.CasoId == id).OrderBy(c => c.Apellidos).ToList();
            
            CasoResponse response = new CasoResponse()
            {
                Caso = caso,
                Proceso = proceso,
                Clientes = clientes,
                Finalizacion = final.Count > 0 ? final[0] : null
            };
            
            return new HttpResult(response, HttpStatusCode.OK);
        }
        catch (Exception ex)
        {
            return new HttpError(HttpStatusCode.BadRequest,
                "Error en la petición: " + ex.Message);
        }
    }

    public Paso CrearPaso(NuevoPasoRequest request)
    {
        // Evaluar si es necesario crear un nuevo paso o reutilizar uno existente
        var paso = _context.Paso.FirstOrDefault(p =>
            p.ActividadVersionId == request.ActividadVersionId && p.CasoId == request.Caso.Id);

        var isOk = true;

        if (paso == null)
        {
            // Crear Paso
            paso = new Paso();
            paso.Id = Generals.GetUlid();
            paso.CasoId = request.Caso.Id;
            paso.ActividadVersionId = request.ActividadVersionId;
            _context.Paso.Add(paso);
            isOk = _context.SaveChanges() == 1;
        }

        if (!isOk) return null;

        //Crear Estado Paso
        EstadoPaso estadoPaso = new EstadoPaso();
        estadoPaso.Id = Generals.GetUlid();
        estadoPaso.PasoId = paso.Id;
        estadoPaso.Estado = request.TipoSeleccion == "autoservicio" ? "Grupo" : "Nuevo";
        estadoPaso.FechaCreacion = DateTime.Now;
        estadoPaso.AsignadoPor = "SIAPP";
        _context.EstadoPaso.Add(estadoPaso);
            
        //Crear Responsable
        Responsable responsable = new Responsable();
        if (request.TipoSeleccion != "autoservicio") // Si es autoservicio no se agrega responsable
        {
                
            responsable.Id = Generals.GetUlid();
            responsable.PasoId = paso.Id;
            responsable.Activo = true;
            responsable.FechaCreacion = DateTime.Now;

            if (request.TipoSeleccion == "ciclico")
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
        if (res > 0)
        {
            if (request.TipoSeleccion == "manual" || request.TipoSeleccion == "monousuario" || request.TipoSeleccion == "accion")
            {
                responsable.Usuario =
                    _context.Usuarios.Where(u => u.CodigoUsuario == responsable.UsuarioId).Select(u => new Usuarios()
                    {
                        CodigoUsuario = u.CodigoUsuario,
                        NombresUsuario = u.NombresUsuario,
                        ApellidosUsuario = u.ApellidosUsuario
                    }).Single();
            }
                
            _context.CasoCliente.ToList();
            _context.EstadoPaso.ToList();
            _context.Responsable.ToList();
            _context.PersonasNaturales.ToList();
            return paso;
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
                if (_context.SaveChanges() > 0)
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
                        if (_context.SaveChanges() > 0)
                        {
                            return ciclo;
                        }
                    }
                }
                
                // si salimos del ciclo entonces creamos nuevamente el ciclo
                ciclo.UsuarioId = usuariosList.First().CodigoUsuario;
                ciclo.Nuevo = true;
                _context.Ciclo.Add(ciclo);
                if (_context.SaveChanges() > 0)
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

    public SignalResponse FinalizarPaso(string pasoId, FinalizarPasoRequest request, string? usuarioId)
    {
        try
        {
            Paso pasoActual = _context.Paso.Find(pasoId);
            Caso caso = _context.Caso.FirstOrDefault(c => c.Id == pasoActual.CasoId);
            
            if (request.RelacionId != null) // Se creará otro paso
            {
                Relaciones? relacionDestino = _context.Relaciones.Find(request.RelacionId);
                if (relacionDestino == null)
                {
                    return new SignalResponse
                    {
                        Response = new HttpError(HttpStatusCode.BadRequest, "Relación no encontrada")
                    };
                }

                if (relacionDestino.TipoSeleccion == "ciclico" || relacionDestino.TipoSeleccion == "manual" ||
                    relacionDestino.TipoSeleccion == "accion")
                { // Comprobar que tenga un perfil
                    //Obtenemos los perfiles de la actividad version
                    List<ActividadVersionPerfil> perfiles = _context.ActividadVersionPerfil
                        .Where(av => av.ActividadVersionId == relacionDestino.ActividadVersionDestino && av.Rol == "responsable" ).ToList();
                    if (perfiles.Count == 0)
                    {
                        return new SignalResponse
                        {
                            Response = new HttpError(HttpStatusCode.BadRequest,
                                "No hay perfiles asociados para la siguiente actividad.")
                        };
                    }
                }
                
                // Crear Estado Paso Finalizado
                EstadoPaso estadoPaso = new EstadoPaso();
                estadoPaso.Id = Generals.GetUlid();
                estadoPaso.PasoId = pasoId;
                estadoPaso.Estado = "Finalizado";
                estadoPaso.FechaCreacion = DateTime.Now;
                estadoPaso.AsignadoPor = usuarioId;
                _context.EstadoPaso.Add(estadoPaso);

                var responsableActual = _context.Responsable.Single(r => r.PasoId == pasoId && r.Activo == true);
                responsableActual.Activo = false;

                _context.Responsable.Update(responsableActual);

                _context.SaveChanges();
                
                ResponsableRequest responsable = new ResponsableRequest
                {
                    UsuarioId = request.ResponsableId,
                    AsignadoPor = usuarioId,
                };
                
                NuevoPasoRequest nuevoPasoRequest = new NuevoPasoRequest();
                nuevoPasoRequest.Caso = caso;
                nuevoPasoRequest.ActividadVersionId = relacionDestino.ActividadVersionDestino;
                nuevoPasoRequest.TipoSeleccion = relacionDestino.TipoSeleccion;
                nuevoPasoRequest.Responsable = responsable;
                        
                // Crear Paso
                Paso paso = CrearPaso(nuevoPasoRequest);
                
                // Llamar Notificaciones Signal R

                return caso != null
                    ? new SignalResponse
                    {
                        Response = new HttpResult(caso, HttpStatusCode.OK),
                        VersionId = _context.ActividadVersiones
                            ?.Where(av => av.Id == nuevoPasoRequest.ActividadVersionId)
                            .Select(av => av.VersionProcesoId).Single(),
                        Responsables = _context.Responsable?.Where(r => r.UsuarioId == usuarioId && r.PasoId == paso.Id).ToList()
                    }
                    : new SignalResponse
                    {
                        Response = new HttpError(HttpStatusCode.BadRequest, "Error al guardar caso. ")
                    };
            }
            
            
            // Se finaliza el paso y el CASO
            if (request.Estado == null)
            {
                return new SignalResponse
                {
                    Response = new HttpError(HttpStatusCode.BadRequest,
                        "El estado es requerido para finalizar la actividad")
                };
            }
            
            // Crear Estado Paso Finalizado
            EstadoPaso estadoPasoFinalizado = new EstadoPaso();
            estadoPasoFinalizado.Id = Generals.GetUlid();
            estadoPasoFinalizado.PasoId = pasoId;
            estadoPasoFinalizado.Estado = "Finalizado";
            estadoPasoFinalizado.FechaCreacion = DateTime.Now;
            estadoPasoFinalizado.AsignadoPor = usuarioId;
            _context.EstadoPaso.Add(estadoPasoFinalizado);
            _context.SaveChanges();
                
            // Actualizar Caso a Campo Abierto false (finalizado)
            caso.Abierto = false;
            _context.Caso.Update(caso);
            if (_context.SaveChanges() == 1)
            {
                // Agregar finalizacion
                Finalizacion final = new Finalizacion
                {
                    Id = Generals.GetUlid(),
                    PasoId = pasoId,
                    Justificacion = request.Justificacion,
                    Estado = request.Estado,
                    Activo = true,
                    FechaCreacion = DateTime.Now,
                    CreadoPor = usuarioId
                };
                _context.Finalizacion.Add(final);
                _context.SaveChanges();

                return new SignalResponse
                {
                    Response = new HttpResult(caso, HttpStatusCode.OK),
                    Responsables = _context.Responsable.Where(r => r.UsuarioId == usuarioId && r.PasoId == pasoId)
                        .ToList(),
                    VersionId = _context.ActividadVersiones
                        .Where(av => av.Id == _context.Paso.Where(p => p.Id == pasoId).Select(p => p.ActividadVersionId).Single())
                        .Select(av => av.VersionProcesoId).Single()
                };

            }

            return new SignalResponse
            {
                Response = new HttpError(HttpStatusCode.BadRequest, "Error al finalizar caso. ")
            };
        }catch (Exception ex)
        {
            return new SignalResponse
            {
                Response = new HttpError(HttpStatusCode.BadRequest,
                    "Error en la petición: " + ex.Message)
            };
        }
    }

    public SignalResponse CambioContexto(CambioContextoRequest request, string? usuario)
    {
        // Buscar el paso
        var pasoActual = _context.Paso.Single(p => p.Id == request.PasoActualId);
        var pasoDestino = _context.Paso.FirstOrDefault(p => p.Id == request.PasoDestinoId);

        var responsableResponse = _context.Responsable.Where(r => r.PasoId == pasoActual.Id)
            .OrderByDescending(p => p.FechaCreacion).Take(1).Single();
        
        // Crear el estado
        var estado = new EstadoPaso
        {
            Estado = (request.Tipo == "estado" && request.Estado != null && request.Estado != "Grupo")
                ? request.Estado
                : "Nuevo",
            Id = Generals.GetUlid(),
            AsignadoPor = usuario,
            FechaCreacion = DateTime.Now,
            PasoId = request.Tipo is "estado" or "responsable" ? pasoActual.Id : pasoDestino.Id
        };
        _context.EstadoPaso.Add(estado);
        
        // En caso de cambiar el responsable
        if (request.Tipo == "responsable" && request.ResponsableId != null)
        {
            // Buscar el responsable activo
            var responsableActivo = _context.Responsable.FirstOrDefault(r => r.Activo == true && r.PasoId == pasoActual.Id);
            if (responsableActivo != null)
            {
                responsableActivo.Activo = false;
                _context.Responsable.Update(responsableActivo);
            }

            var responsableActual = new Responsable
            {
                Id = Generals.GetUlid(),
                Activo = true,
                FechaCreacion = DateTime.Now,
                AsignadoPor = usuario,
                PasoId = pasoActual.Id,
                UsuarioId = request.ResponsableId
            };

            responsableResponse = responsableActual;

            _context.Responsable.Add(responsableActual);
        }
        
        if (request.Tipo != "responsable")
        {
            if (request.Tipo is "paso" or "caso")
            {
                // Reactivar al responsable de destino
                var responsableDestino = _context.Responsable.Where(r => r.PasoId == pasoDestino.Id)
                    .OrderByDescending(p => p.FechaCreacion).Select(r => new Responsable
                    {
                        Activo = true,
                        Id = Generals.GetUlid(),
                        AsignadoPor = usuario,
                        FechaCreacion = DateTime.Now,
                        PasoId = r.PasoId,
                        UsuarioId = r.UsuarioId
                    }).Single();
                responsableResponse = responsableDestino;
                _context.Responsable.Add(responsableDestino);

                if (request.Tipo == "paso")
                {
                    var estadoActual = new EstadoPaso
                    {
                        Estado = "Finalizado",
                        Id = Generals.GetUlid(),
                        AsignadoPor = usuario,
                        FechaCreacion = DateTime.Now,
                        PasoId = pasoActual.Id
                    };
                    _context.EstadoPaso.Add(estadoActual);
                }
                else
                {
                    // Almacenar registro de reapertura
                    var reapertura = new PasoReapertura
                    {
                        Id = Generals.GetUlid(),
                        Justificacion = request.Justificacion,
                        CreadoPor = usuario,
                        FechaCreacion = DateTime.Now,
                        PasoId = pasoDestino.Id
                    };
                    _context.PasoReaperturas.Add(reapertura);

                    var caso = _context.Caso.Single(c => c.Id == pasoActual.CasoId);
                    caso.Abierto = true;
                    _context.Caso.Update(caso);
                }
            }
        }

        if (_context.SaveChanges() > 0)
        {
            return new SignalResponse
            {
                Responsables = new List<Responsable> { responsableResponse },
                Response = new HttpResult(HttpStatusCode.OK, "Acción exitosa"),
                VersionId = _context.ActividadVersiones
                    .Where(av => av.Id == _context.Paso.Where(p => p.Id == pasoActual.Id)
                        .Select(p => p.ActividadVersionId).Single())
                    .Select(av => av.VersionProcesoId).Single()
            };
        }
        
        return new SignalResponse
        {
            Responsables = null,
            Response = new HttpError(HttpStatusCode.InternalServerError, "No se pudo guardar correctamente"),
            VersionId = null
        };
    }
}