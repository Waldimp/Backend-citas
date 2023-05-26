using Microsoft.EntityFrameworkCore;
using WebCasosSiapp.Models.PRO;
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
    public DbSet<Vw_CasosApertura>? VwCasosApertura { get; set; }
    public DbSet<VwCasoTiempoRes>? VwCasosTiempoResponsables { get; set; }
    public DbSet<Caso>? Caso { get; set; }
    public DbSet<CasoCliente>? CasoCliente { get; set; }
    public DbSet<Paso>? Paso { get; set; }
    public DbSet<EstadoPaso>? EstadoPaso { get; set; }
    public DbSet<Responsable>? Responsable { get; set; }
    public DbSet<ActividadVersiones>? ActividadVersiones { get; set; }
    public DbSet<VersionProcesos>? VersionProcesos { get; set; }
    public DbSet<PersonasNaturales>? PersonasNaturales { get; set; }
    public DbSet<ProcesoFijoUsuario>? ProcesoFijoUsuario { get; set; }

    // Model builder
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UsuarioPerfil>().HasKey(k => new { k.CodigoPerfil, k.CodigoUsuario });
        modelBuilder.Entity<ActividadVersiones>().HasKey(k => new { k.Id, k.VersionProcesoId, k.ActividadId });
        modelBuilder.Entity<VersionProcesos>().HasKey(k => new { k.Id, k.ProcesoId });
        modelBuilder.Entity<PersonasNaturales>().HasKey(k => new { k.CodigoPersona });
        modelBuilder.Entity<ProcesoFijoUsuario>().HasKey(k => new { k.Id, k.ProcesoId, k.UsuarioId });
        
        modelBuilder.Entity<Paso>()
            .HasOne<Caso>()
            .WithMany(v => v.Pasos)
            .HasForeignKey(a => a.CasoId)
            .HasPrincipalKey(v => v.Id);
        
        modelBuilder.Entity<CasoCliente>()
            .HasOne<Caso>()
            .WithMany(v => v.Clientes)
            .HasForeignKey(a => a.CasoId)
            .HasPrincipalKey(v => v.Id);
        
        modelBuilder.Entity<EstadoPaso>()
            .HasOne<Paso>()
            .WithMany(a => a.Estados)
            .HasForeignKey(r => r.PasoId)
            .HasPrincipalKey(a => a.Id);
        
        modelBuilder.Entity<Responsable>()
            .HasOne<Paso>()
            .WithMany(v => v.Responsable)
            .HasForeignKey(s=>s.PasoId)
            .HasPrincipalKey(a=>a.Id);
    }
}