using LexMigración.Models;
using Microsoft.EntityFrameworkCore;

namespace LexMigración.Services
{
    public class LexMigracionContext : DbContext
    {
       
       
        public DbSet<TestimonioModel> Testimonios { get; set; }
        public DbSet<ProtocoloModel> Protocolos { get; set; }
        public DbSet<RegistroIndice> Indices { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Expediente> Expedientes { get; set; } 

        
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            
            string connectionString = "Server=ALONDRA\\SQLEXPRESS;Database=LexMigracion;Trusted_Connection=True;TrustServerCertificate=True;";

            optionsBuilder.UseSqlServer(connectionString);
        }
    }
}