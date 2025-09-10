using LexMigración.Models;
using Microsoft.EntityFrameworkCore;

namespace LexMigración.Services
{
    public class LexMigracionContext : DbContext
    {
        public DbSet<Anexo> Anexos { get; set; }
        public DbSet<ProtocoloModel> Protocolos { get; set; }
        public DbSet<RegistroIndice> Indices { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // --- CAMBIO IMPORTANTE ---
            // Hemos reemplazado UseSqlite por UseSqlServer
            // y hemos puesto una nueva cadena de conexión.

            // Reemplaza "NOMBRE_DE_TU_SERVIDOR" por el nombre de tu instancia de SQL Server.
            // Puede ser algo como "LAPTOP-12345" o ".\SQLEXPRESS".
            string connectionString = "Server=GERMAN25\\SQLEXPRESS;Database=LexMigracionDB;Trusted_Connection=True;TrustServerCertificate=True;";

            optionsBuilder.UseSqlServer(connectionString);
        }
    }
}