using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LexMigración.Models
{
    public class ProtocoloModel // <-- NOMBRE CAMBIADO
    {
        public int Id { get; set; }
        public string ExpedienteId { get; set; }
        public string NumeroEscritura { get; set; }
        public DateTime Fecha { get; set; }
        public string Extracto { get; set; }
        public string TextoCompleto { get; set; }
        public bool Firmado { get; set; }
    }
}
