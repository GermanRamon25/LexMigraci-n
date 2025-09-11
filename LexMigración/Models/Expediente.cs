using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LexMigración.Models
{
    public class Expediente
    {
        public int Id { get; set; }
        public string Identificador { get; set; }
        public string NombreCliente { get; set; }
        public string TipoCaso { get; set; }
        public DateTime FechaCreacion { get; set; }
    }
}
