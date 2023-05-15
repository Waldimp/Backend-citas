using System.Net;
using ServiceStack;
using WebCasosSiapp.Concretes.Contexts;
using WebCasosSiapp.Concretes.Functions;
using WebCasosSiapp.Interfaces;
using WebCasosSiapp.Models.PRO;

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
}