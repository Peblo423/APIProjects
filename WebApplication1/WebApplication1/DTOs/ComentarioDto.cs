using System.ComponentModel.DataAnnotations;

namespace BibliotecaAPI.DTOs
{
    public class ComentarioDto
    {
        public Guid Id { get; set; }
        // Acá no va required porque esto es para lectura
        public required string Cuerpo { get; set; }
        public DateTime FechaPublicacion { get; set; }
    }
}
