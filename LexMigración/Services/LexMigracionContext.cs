using LexMigración.Models;
using Microsoft.EntityFrameworkCore;

namespace LexMigración.Services
{
    public class LexMigracionContext : DbContext
    {
        // --- DbSet para cada Tabla ---
        // Cada una de estas propiedades representa una tabla en tu base de datos
        // y la conecta con su clase modelo correspondiente en C#.
        public DbSet<Anexo> Anexos { get; set; }
        public DbSet<ProtocoloModel> Protocolos { get; set; }
        public DbSet<RegistroIndice> Indices { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }

        /// <summary>
        /// Este método configura la conexión a la base de datos.
        /// </summary>
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // --- Cadena de Conexión a SQL Server ---
            // Aquí se define cómo se conectará la aplicación a tu servidor.

            // IMPORTANTE: Reemplaza "NOMBRE_DE_TU_SERVIDOR" con el nombre de tu
            // instancia de SQL Server (ej: "MI-PC\SQLEXPRESS" o "localhost").
            string connectionString = "Server=GERMAN25\\SQLEXPRESS;Database=LexMigracionDB;Trusted_Connection=True;TrustServerCertificate=True;";

            // Le indicamos a Entity Framework que use el proveedor de SQL Server
            // con la cadena de conexión que definimos.
            optionsBuilder.UseSqlServer(connectionString);
        }
    }
}