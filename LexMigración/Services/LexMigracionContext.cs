using LexMigración.Models;
using Microsoft.EntityFrameworkCore;

namespace LexMigración.Services
{
    public class LexMigracionContext : DbContext
    {
       
        // Cada propiedad representa una tabla en tu base de datos.
        public DbSet<TestimonioModel> Testimonios { get; set; }
        public DbSet<ProtocoloModel> Protocolos { get; set; }
        public DbSet<RegistroIndice> Indices { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Expediente> Expedientes { get; set; } // <--- Tabla de Expedientes añadida

        
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            
            string connectionString = "Server=ALONDRA\\SQLEXPRESS;Database=LexMigracion;Trusted_Connection=True;TrustServerCertificate=True;";

            optionsBuilder.UseSqlServer(connectionString);
        }
    }
}