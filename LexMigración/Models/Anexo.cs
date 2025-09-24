using System;

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
        public string NombreArchivo { get; set; }
        public string ContenidoArchivo { get; set; }

        // --- *** NUEVOS CAMPOS AÑADIDOS *** ---
        public string Volumen { get; set; }
        public string Libro { get; set; }
        public string Folio { get; set; }
    }
}