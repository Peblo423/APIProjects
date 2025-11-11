using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BibliotecaAPI.Entities
{
    public class Libro
    {
        public int Id { get; set; }
        [Required]
        public required string Título { get; set; }
        public int AutorId { get; set; } 
        public Autor? Autor { get; set; }
    }
}
