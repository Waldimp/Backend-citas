using WebCasosSiapp.Concretes.Contexts;
using WebCasosSiapp.Interfaces;
using WebCasosSiapp.Models.PRO.Views;

namespace WebCasosSiapp.Concretes;

public class VwCaseConcrete: IVwCases
{
    private readonly DatabaseContext _ctx;

    public VwCaseConcrete(DatabaseContext ctx)
    {
        _ctx = ctx;
    }
    
    public List<VwCaso>? Index()
    {
        return _ctx.VwCases?.ToList();
    }
}