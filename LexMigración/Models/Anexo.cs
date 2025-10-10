using System;

namespace LexMigración.Models
{
    public class Anexo
    {
        public int Id { get; set; }
        public string ExpedienteId { get; set; }
        public string NumeroEscritura { get; set; }
        public string Estado { get; set; }
        public DateTime CreatedAt { get; set; }
        public string NombreArchivo { get; set; }
        public string ContenidoArchivo { get; set; }
        public string Volumen { get; set; }
        public string Libro { get; set; }
       
    }
}