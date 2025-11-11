using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BibliotecaAPI.Entities
{
    public class Libro
    {
        public int Id { get; set; }
        [Required]
        [StringLength(150, ErrorMessage = "El campo {0} debe tener {1} caracteres o menos")]
        public required string Título { get; set; }
        public int AutorId { get; set; } 
        public Autor? Autor { get; set; }

        public List<Comentario> Comentarios { get; set; } = [];
    }
}
