using WebCasosSiapp.Concretes.Contexts;
using WebCasosSiapp.Interfaces;
using WebCasosSiapp.Models.PRO.Views;

namespace WebCasosSiapp.Concretes;

public class VwCasoConcrete: IVwCasos
{
    private readonly DatabaseContext _ctx;

    public VwCasoConcrete(DatabaseContext ctx)
    {
        _ctx = ctx;
    }
    
    public List<VwCaso>? Index()
    {
        return _ctx.VwCases?.ToList();
    }
}