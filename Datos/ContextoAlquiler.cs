using Microsoft.EntityFrameworkCore;
using SistemaAlquilerAutos.Models;

namespace SistemaAlquilerAutos.Datos;

public class ContextoAlquiler : DbContext
{
    public ContextoAlquiler(DbContextOptions<ContextoAlquiler> options) : base(options)
    {
    }

    public DbSet<Cliente> Clientes => Set<Cliente>();
    public DbSet<Vehiculo> Vehiculos => Set<Vehiculo>();
    public DbSet<Contrato> Contratos => Set<Contrato>();
    public DbSet<Pago> Pagos => Set<Pago>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Cliente>(entidad =>
        {
            entidad.ToTable("Clientes");
            entidad.HasIndex(x => x.Cedula).IsUnique();
            entidad.HasIndex(x => x.Correo).IsUnique();
            entidad.Ignore(x => x.NombreCompleto);
        });

        modelBuilder.Entity<Vehiculo>(entidad =>
        {
            entidad.ToTable("Vehiculos");
            entidad.HasIndex(x => x.Placa).IsUnique();
            entidad.Property(x => x.Estado).HasConversion<string>().HasMaxLength(20);
            entidad.Ignore(x => x.Descripcion);
        });

        modelBuilder.Entity<Contrato>(entidad =>
        {
            entidad.ToTable("Contratos");
            entidad.Property(x => x.Estado).HasConversion<string>().HasMaxLength(20);
            entidad.HasOne(x => x.Cliente)
                .WithMany(x => x.Contratos)
                .HasForeignKey(x => x.ClienteId)
                .OnDelete(DeleteBehavior.Restrict);
            entidad.HasOne(x => x.Vehiculo)
                .WithMany(x => x.Contratos)
                .HasForeignKey(x => x.VehiculoId)
                .OnDelete(DeleteBehavior.Restrict);
            entidad.Ignore(x => x.TotalPagado);
            entidad.Ignore(x => x.SaldoPendiente);
        });

        modelBuilder.Entity<Pago>(entidad =>
        {
            entidad.ToTable("Pagos");
            entidad.Property(x => x.Metodo).HasConversion<string>().HasMaxLength(20);
            entidad.Property(x => x.Estado).HasConversion<string>().HasMaxLength(20);
            entidad.HasOne(x => x.Contrato)
                .WithMany(x => x.Pagos)
                .HasForeignKey(x => x.ContratoId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
