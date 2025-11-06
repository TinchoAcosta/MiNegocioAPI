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
        public DbSet<ServicioPropio> ServicioPropios { get; set; }
        public DbSet<Promo> Promo { get; set; }
        public DbSet<Turno> Turno { get; set; }
        public DbSet<Pago> Pago { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Turno>()
                .Property(t => t.estado)
                .HasConversion<string>();
        }
    }
}