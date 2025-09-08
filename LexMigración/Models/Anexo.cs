using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LexMigración.Models
{
    public class Anexo
    {
        public int Id { get; set; }
        public string ExpedienteId { get; set; }
        public int FoliadoInicio { get; set; }
        public int FoliadoFin { get; set; }
        public int TotalHojas { get; set; }
        public string Estado { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
