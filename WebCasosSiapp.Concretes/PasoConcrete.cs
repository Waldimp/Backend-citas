using System.Net;
using Microsoft.EntityFrameworkCore;
using ServiceStack;
using WebCasosSiapp.Concretes.Contexts;
using WebCasosSiapp.Concretes.Functions;
using WebCasosSiapp.Interfaces;
using WebCasosSiapp.Models.PRO;
using WebCasosSiapp.ViewModels.Responses;

namespace WebCasosSiapp.Concretes;

public class PasoConcrete : IPaso
{
    private readonly DatabaseContext _context;
    
    public PasoConcrete(DatabaseContext context)
    {
        _context = context;
    }

    public SignalResponse MarcarPasoLeido(string PasoId, string? user)
    {
        try
        {
            if (user == null)
                return new SignalResponse
                {
                    Response = new HttpError(HttpStatusCode.BadRequest, "Usuario no válido. "),
                    Responsables = null,
                    VersionId = null
                };
            
            EstadoPaso estadoPaso = new EstadoPaso();
            estadoPaso.Id = Generals.GetUlid();
            estadoPaso.PasoId = PasoId;
            estadoPaso.Estado = "En proceso";
            estadoPaso.FechaCreacion = DateTime.Now;
            estadoPaso.AsignadoPor = user;

            var version = _context.ActividadVersiones
                .Where(av => av.Id == _context.Paso.Where(p => p.Id == PasoId).Select(p => p.ActividadVersionId).Single())
                .Select(av => av.VersionProcesoId).Single();
                            
            _context.EstadoPaso.Add(estadoPaso);
            if (_context.SaveChanges() == 1)
            {
                return new SignalResponse
                {
                    Response = new HttpResult(estadoPaso, HttpStatusCode.OK),
                    Responsables = _context.Responsable?.Where(r => r.UsuarioId == user && r.PasoId == PasoId).ToList(),
                    VersionId = version
                };
            }

            return new SignalResponse
            {
                Response = new HttpError(HttpStatusCode.BadRequest, "Error al marcar como leido. ")
            };
        }
        catch (Exception ex)
        {
            return new SignalResponse
            {
                Response = new HttpError(HttpStatusCode.BadRequest,
                    "Error en la petición: " + ex.Message)
            };
        }

    }

    public object AutoasignarPaso(string PasoId, string? user)
    {
        try
        {
            if (user == null)
                return new HttpError(HttpStatusCode.BadRequest,"Error en el usuario de sesión.");

            List<Responsable> responsables = _context.Responsable.Where(r => r.PasoId == PasoId && r.Activo == true).ToList();
            if (responsables.Count > 0)
            {
                return new HttpError(HttpStatusCode.BadRequest,"Ya existe un usuario asignado a este paso.");
            }

            // Agregar responsable
            Responsable responsable = new Responsable();
            responsable.Id = Generals.GetUlid();
            responsable.PasoId = PasoId;
            responsable.FechaCreacion = DateTime.Now;
            responsable.Activo = true;
            responsable.UsuarioId = user;
            responsable.AsignadoPor = user;
            
            _context.Responsable.Add(responsable);
            int res = _context.SaveChanges();
            if (res > 0)
            {
                EstadoPaso estadoNuevo = new EstadoPaso()
                {
                    Id = Generals.GetUlid(),
                    PasoId = PasoId,
                    Estado = "Nuevo",
                    FechaCreacion = DateTime.Now,
                    AsignadoPor = user
                };
                _context.EstadoPaso.Add(estadoNuevo);
            
                EstadoPaso estadoProceso = new EstadoPaso()
                {
                    Id = Generals.GetUlid(),
                    PasoId = PasoId,
                    Estado = "En proceso",
                    FechaCreacion = DateTime.Now,
                    AsignadoPor = user
                };
                _context.EstadoPaso.Add(estadoProceso);

                if (_context.SaveChanges() > 1)
                {
                    return new HttpResult("Ingresado correctamente.", HttpStatusCode.OK);
                }
            }
        }
        catch (Exception ex)
        {
            return new HttpError(HttpStatusCode.BadRequest,"Error en la petición: " + ex.Message);
        }
        return null;
    }

    public object DatosDePaso(string PasoId)
    {
        try
        {
            Paso paso = _context.Paso.FirstOrDefault(p => p.Id == PasoId);
            if (paso == null)
            {
                return new HttpError(HttpStatusCode.BadRequest, "Paso no encontrado.");
            }

            Caso caso = _context.Caso.FirstOrDefault(c => c.Id == paso.CasoId);
            _context.Actividades.ToList();
            _context.Registro.Where(r => r.PasoId == PasoId).ToList();
            var rel = _context.Relaciones?.Select(r => new RelacionesExt()
            {
                Id = r.Id,
                Accion = r.Accion,
                TipoMovimiento = r.TipoMovimiento,
                TipoSeleccion = r.TipoSeleccion,
                ActividadVersionDestino = r.ActividadVersionDestino,
                ActividadVersionOrigen = r.ActividadVersionOrigen,
                NombreActividadDestino = _context.Actividades
                    .Where(a => a.Id == _context.ActividadVersiones.Where(av => av.Id == r.ActividadVersionDestino)
                        .Select(av => av.ActividadId).Single()).Select(a => a.Nombre).Single(),
                NombreColoquialActividadDestino = _context.Actividades
                    .Where(a => a.Id == _context.ActividadVersiones.Where(av => av.Id == r.ActividadVersionDestino)
                        .Select(av => av.ActividadId).Single()).Select(a => a.NombreColoquial).Single(),
            }).ToList();
            
            var actVersiones = _context.ActividadVersiones?.FirstOrDefault(c => c.Id == paso.ActividadVersionId);

            var actividadVersiones = new ActividadVersionesExt
            {
                Actividad = actVersiones.Actividad,
                Activo = actVersiones.Activo,
                Favorable = actVersiones.Favorable,
                Id = actVersiones.Id,
                Iterativo = actVersiones.Iterativo,
                Observaciones = actVersiones.Observaciones,
                Relaciones = rel.FindAll(r => r.ActividadVersionOrigen == actVersiones.Id),
                Relevancia = actVersiones.Relevancia,
                ActividadId = actVersiones.ActividadId,
                MedirTiempo = actVersiones.MedirTiempo,
                TiempoIdeal = actVersiones.TiempoIdeal,
                TipoActividad = actVersiones.TipoActividad,
                TipoEtapa = actVersiones.TipoEtapa,
                ResumenDatos = actVersiones.ResumenDatos,
                VersionProcesoId = actVersiones.VersionProcesoId
            };
            
            // Obtener todos los casos y a partir de ellos obtener los registros de todos los casos
            List<Paso> pasosCaso = _context.Paso.Where(p => p.CasoId == caso.Id).ToList();
            List<Registro> allRegistros = new List<Registro>();
            foreach (var reg in pasosCaso.Select(pasoC => _context.Registro.Where(r => r.PasoId == pasoC.Id).ToList()))
            {
                allRegistros.AddRange(reg);
            }

            // Guardando solo los datos del caso
            Caso casoDatos = new Caso();
            casoDatos.Id = caso.Id;
            casoDatos.Codigo = caso.Codigo;
            casoDatos.FechaCreacion = caso.FechaCreacion;
            casoDatos.Abierto = caso.Abierto;
            casoDatos.ComentarioApertura = caso.ComentarioApertura;
            casoDatos.CasoAsociado = caso.CasoAsociado;
            casoDatos.Pasos = null;
            
            // Datos del proceso
            var proceso = _context.Procesos.Where(p =>
                p.Id == _context.VersionProcesos.Where(vp => vp.Id == actividadVersiones.VersionProcesoId)
                    .Select(vp => vp.ProcesoId).Single()).Select(p => new ProcesoResumen
            {
                Codigo = p.Codigo,
                Id = p.Id,
                Interno = p.Interno,
                Nombre = p.Nombre,
                Unidad = _context.Unidades.Where(u => u.CodigoUnidades == p.CodigoUnidad).Select(u => u.Unidades)
                    .Single(),
                Version = _context.VersionProcesos.Where(v => v.Id == actividadVersiones.VersionProcesoId)
                    .Select(v => v.NumeroVersion).Single()
            }).Single();

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
                }).Where(c => c.CasoId == casoDatos.Id).OrderBy(c => c.Apellidos).ToList();
 
            //Creando response 
            PasoResponse response = new PasoResponse();
            response.Id = PasoId;
            response.Caso = casoDatos;
            response.Estados = _context.EstadoPaso.Where(ep => ep.PasoId == PasoId).ToList();
            response.Responsables = _context.Responsable.Where(r => r.PasoId == PasoId).ToList();
            response.Observaciones = _context.Observaciones.Where(o => o.PasoId == PasoId).ToList();
            response.Secciones = _context.Secciones.Where(s => s.ActividadVersionId == actividadVersiones.Id).OrderBy(s => s.Orden).ToList();
            response.Registros = allRegistros;
            response.Actividad = actividadVersiones;
            response.Proceso = proceso;
            response.Clientes = clientes;
            
            return new HttpResult(response, HttpStatusCode.OK);
        }
        catch (Exception ex)
        {
            return new HttpError(HttpStatusCode.BadRequest,
                "Error en la petición: " + ex.Message);
        }
    }
}