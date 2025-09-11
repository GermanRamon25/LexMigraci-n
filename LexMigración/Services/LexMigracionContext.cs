using LexMigración.Models;
using Microsoft.EntityFrameworkCore;

namespace LexMigración.Services
{
    public class LexMigracionContext : DbContext
    {
        // --- DbSet para cada Tabla ---
        // Cada propiedad representa una tabla en tu base de datos.
        public DbSet<Anexo> Anexos { get; set; }
        public DbSet<ProtocoloModel> Protocolos { get; set; }
        public DbSet<RegistroIndice> Indices { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Expediente> Expedientes { get; set; } // <--- Tabla de Expedientes añadida

        /// <summary>
        /// Configura la conexión a la base de datos SQL Server.
        /// </summary>
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // IMPORTANTE: Reemplaza "NOMBRE_DE_TU_SERVIDOR" con el nombre de tu
            // instancia de SQL Server (ej: "MI-PC\SQLEXPRESS" o el nombre que uses en SSMS).
            string connectionString = "Server=GERMAN25\\SQLEXPRESS;Database=LexMigracionDB;Trusted_Connection=True;TrustServerCertificate=True;";

            optionsBuilder.UseSqlServer(connectionString);
        }
    }
}