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

        // --- NUEVA PROPIEDAD PARA GUARDAR EL CONTENIDO ---
        public string ContenidoArchivo { get; set; }
    }
}