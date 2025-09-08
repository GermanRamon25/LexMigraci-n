using LexMigración.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using LexMigración.Models;
using Microsoft.EntityFrameworkCore;

namespace LexMigración.Services
{
    public class LexMigracionContext : DbContext
    {
        public DbSet<Anexo> Anexos { get; set; }
        public DbSet<ProtocoloModel> Protocolos { get; set; } // <-- CAMBIO AQUÍ
        public DbSet<RegistroIndice> Indices { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=lexmigracion.db");
        }
    }
}