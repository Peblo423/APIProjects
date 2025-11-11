using System.ComponentModel.DataAnnotations;

namespace BibliotecaAPI.DTOs
{
    public class ComentarioCreacionDto
    {
        [Required]
        public required string Cuerpo { get; set; }
    }
}
