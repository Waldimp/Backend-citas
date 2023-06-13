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

    public object MarcarPasoLeido(string PasoId)
    {
        try
        {
            EstadoPaso estadoPaso = new EstadoPaso();
            estadoPaso.Id = Generals.GetUlid();
            estadoPaso.PasoId = PasoId;
            estadoPaso.Estado = "En proceso";
            estadoPaso.FechaCreacion = DateTime.Now;
            estadoPaso.AsignadoPor = "SIAPP";
                            
            _context.EstadoPaso.Add(estadoPaso);
            if (_context.SaveChanges() == 1)
            {
                return new HttpResult(estadoPaso, HttpStatusCode.OK);
            }
            return new HttpError(HttpStatusCode.BadRequest, "Error al marcar como leido. ");
        }
        catch (Exception ex)
        {
            return new HttpError(HttpStatusCode.BadRequest,
                "Error en la petición: " + ex.Message);
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

            // Guardando solo los datos del caso
            Caso casoDatos = new Caso();
            casoDatos.Id = caso.Id;
            casoDatos.Codigo = caso.Codigo;
            casoDatos.FechaCreacion = caso.FechaCreacion;
            casoDatos.Abierto = caso.Abierto;
            casoDatos.ComentarioApertura = caso.ComentarioApertura;
            casoDatos.CasoAsociado = caso.CasoAsociado;
            
            //Creando response 
            PasoResponse response = new PasoResponse();
            response.Id = PasoId;
            response.Caso = casoDatos;
            response.Estados = _context.EstadoPaso.Where(ep => ep.PasoId == PasoId).ToList();
            response.Responsables = _context.Responsable.Where(r => r.PasoId == PasoId).ToList();
            response.Observaciones = _context.Observaciones.Where(o => o.PasoId == PasoId).ToList();
            response.Secciones = _context.Secciones.Where(s => s.ActividadVersionId == actividadVersiones.Id).OrderBy(s => s.Orden).ToList();

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