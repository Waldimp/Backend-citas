using WebCasosSiapp.Concretes.Contexts;
using WebCasosSiapp.Interfaces;

namespace WebCasosSiapp.Concretes;

public class HubDataConcrete : IHubData
{
    private readonly DatabaseContext _ctx;

    public HubDataConcrete(DatabaseContext context)
    {
        _ctx = context;
    }

    /*
     * Function: GetProcessList
     * Get a list of process associates a specific user
     */
    public object GetProcessList(string user)
    {
        if (_ctx.UserProfiles != null)
        {
            // Find user profiles associates to user code
            var profilesAssociates = _ctx.UserProfiles.Where(up => up.CodigoUsuario == user).ToList();
        }

        return null;
    }

    private string CurrentRecordSql()
    {
        return "";
    }
}