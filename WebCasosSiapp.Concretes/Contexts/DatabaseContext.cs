using Microsoft.EntityFrameworkCore;
using WebCasosSiapp.Models.PRO.Views;
using WebCasosSiapp.Models.SIS;

namespace WebCasosSiapp.Concretes.Contexts;

public class DatabaseContext: DbContext
{
    public DatabaseContext(DbContextOptions<DatabaseContext> options): base(options){}
    
    // Schema SIS
    public DbSet<UsuarioPerfil>? UserProfiles { get; set; }
    // Schema PRO
    public DbSet<VwCaso>? VwCases { get; set; }
    public DbSet<VwCasoResumen>? VwCasoResumenes { get; set; }
    public DbSet<VwCasoActividadNuevo>? VwCasoActividadesNuevas { get; set; }

    // Model builder
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UsuarioPerfil>().HasKey(k => new { k.CodigoPerfil, k.CodigoUsuario });
    }
}