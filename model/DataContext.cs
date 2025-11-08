using Microsoft.EntityFrameworkCore;

namespace MiNegocioAPI.model
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {

        }

        public DbSet<Usuario> Usuario { get; set; }
        public DbSet<Cliente> Cliente { get; set; }
        public DbSet<ServicioBase> ServicioBase { get; set; }
        public DbSet<ServicioPropio> ServicioPropio { get; set; }
        public DbSet<Promo> Promo { get; set; }
        public DbSet<Turno> Turno { get; set; }
        public DbSet<Pago> Pago { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Turno>()
                .Property(t => t.estado)
                .HasConversion<string>();
                
             // 🔹 ServicioPropio -> Usuario
            modelBuilder.Entity<ServicioPropio>()
                .HasOne(sp => sp.usuario)
                .WithMany(u => u.servicioPropios)
                .HasForeignKey(sp => sp.usuarioId)
                .OnDelete(DeleteBehavior.Restrict);

            // 🔹 ServicioPropio -> ServicioBase
            modelBuilder.Entity<ServicioPropio>()
                .HasOne(sp => sp.servicioBase)
                .WithMany()
                .HasForeignKey(sp => sp.servicioId)
                .OnDelete(DeleteBehavior.Restrict);

            // 🔹 Turno -> Cliente
            modelBuilder.Entity<Turno>()
                .HasOne(t => t.cliente)
                .WithMany(c => c.turnos)
                .HasForeignKey(t => t.clienteId)
                .OnDelete(DeleteBehavior.Restrict);

            // 🔹 Turno -> ServicioPropio
            modelBuilder.Entity<Turno>()
                .HasOne(t => t.servicio)
                .WithMany(sp => sp.turnos)
                .HasForeignKey(t => t.servicioId)
                .OnDelete(DeleteBehavior.Restrict);

            // 🔹 Turno -> Promo
            modelBuilder.Entity<Turno>()
                .HasOne(t => t.promo)
                .WithMany(p => p.turnos)
                .HasForeignKey(t => t.promoId)
                .OnDelete(DeleteBehavior.Restrict);

            // 🔹 Promo -> ServicioPropio
            modelBuilder.Entity<Promo>()
                .HasOne(p => p.servicioPropio)
                .WithMany(sp => sp.promos)
                .HasForeignKey(p => p.servicioPropioId)
                .OnDelete(DeleteBehavior.Restrict);

            // 🔹 Pago -> Turno
            modelBuilder.Entity<Pago>()
                .HasOne(p => p.turno)
                .WithMany(t => t.pagos)
                .HasForeignKey(p => p.turnoId)
                .OnDelete(DeleteBehavior.Restrict);    
        }
    }
}