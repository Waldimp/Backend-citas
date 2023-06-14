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
                .Where(av => av.Id == _context.Paso.Where(p => p.Id == PasoId).Select(p => p.Id).Single())
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
                ResumenDatos = r.ResumenDatos,
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
            
            return new HttpResult(response, HttpStatusCode.OK);
        }
        catch (Exception ex)
        {
            return new HttpError(HttpStatusCode.BadRequest,
                "Error en la petición: " + ex.Message);
        }
    }
}