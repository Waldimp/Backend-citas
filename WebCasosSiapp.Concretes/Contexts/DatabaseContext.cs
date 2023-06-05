using Microsoft.EntityFrameworkCore;
using WebCasosSiapp.Models.HUM;
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
    public DbSet<Ciclo>? Ciclo { get; set; }
    public DbSet<ActividadVersionPerfil> ActividadVersionPerfil { get; set; }
    public DbSet<Usuarios>? Usuarios { get; set; }
    public DbSet<Empleados> Empleados { get; set; }
    public DbSet<Relaciones> Relaciones { get; set; }
    public DbSet<Secciones> Secciones { get; set; }
    public DbSet<PerfilSeccion> PerfilSeccion { get; set; }
    public DbSet<Actividades> Actividades { get; set; }
    public DbSet<Observaciones>? Observaciones { get; set; }
    public DbSet<Registro>? Registro { get; set; }
    // Model builder
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UsuarioPerfil>().HasKey(k => new { k.CodigoPerfil, k.CodigoUsuario });
        modelBuilder.Entity<ActividadVersiones>().HasKey(k => new { k.Id, k.VersionProcesoId, k.ActividadId });
        modelBuilder.Entity<VersionProcesos>().HasKey(k => new { k.Id, k.ProcesoId });
        modelBuilder.Entity<PersonasNaturales>().HasKey(k => new { k.CodigoPersona });
        modelBuilder.Entity<ProcesoFijoUsuario>().HasKey(k => new { k.Id, k.ProcesoId, k.UsuarioId });
        modelBuilder.Entity<Observaciones>().HasKey(k => new { k.Id, k.CreadoPor });
        modelBuilder.Entity<Usuarios>().HasKey(k => new { k.CodigoUsuario });
        modelBuilder.Entity<Ciclo>().HasKey(k => new { k.Id, k.UsuarioId, k.ActividadVersionId });
        modelBuilder.Entity<Empleados>().HasKey(k => new { k.CodigoEmpleado });

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
            .WithMany(v => v.Responsables)
            .HasForeignKey(s=>s.PasoId)
            .HasPrincipalKey(a=>a.Id);

        modelBuilder.Entity<Observaciones>()
            .HasOne<Paso>()
            .WithMany(a => a.Observaciones)
            .HasForeignKey(r => r.PasoId)
            .HasPrincipalKey(a => a.Id);

        modelBuilder.Entity<ActividadVersionPerfil>()
            .HasOne<UsuarioPerfil>(p=>p.Perfil)
            .WithMany()
            .HasForeignKey(s=>s.PerfilId)
            .HasPrincipalKey(a=>a.CodigoPerfil);
        
        modelBuilder.Entity<ActividadVersiones>()
            .HasOne<VersionProcesos>()
            .WithMany(v => v.Actividades)
            .HasForeignKey(a => a.VersionProcesoId)
            .HasPrincipalKey(v => v.Id);
        
        // Actividades versiones y relaciones
        modelBuilder.Entity<Relaciones>()
            .HasOne<ActividadVersiones>()
            .WithMany(a => a.Relaciones)
            .HasForeignKey(r => r.ActividadVersionOrigen)
            .HasPrincipalKey(a => a.Id);

        modelBuilder.Entity<Secciones>()
            .HasOne<ActividadVersiones>()
            .WithMany(v => v.Secciones)
            .HasForeignKey(s=>s.ActividadVersionId)
            .HasPrincipalKey(a=>a.Id);
    }
}