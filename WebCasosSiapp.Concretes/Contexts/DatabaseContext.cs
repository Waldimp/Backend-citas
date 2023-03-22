using Microsoft.EntityFrameworkCore;

namespace WebCasosSiapp.Concretes.Contexts;

public class DatabaseContext: DbContext
{
    public DatabaseContext(DbContextOptions<DatabaseContext> options): base(options){}
    
}