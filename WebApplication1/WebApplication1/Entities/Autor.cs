using BibliotecaAPI.Validations;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BibliotecaAPI.Entities
{
    public class Autor
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Se requiere el campo {0}")]
        [StringLength(150, ErrorMessage = "El campo {0} debe tener {1} caracteres o menos")]
        [PrimeraLetra]
        public required string Nombres { get; set; }

        [Required(ErrorMessage = "Se requiere el campo {0}")]
        [StringLength(150, ErrorMessage = "El campo {0} debe tener {1} caracteres o menos")]
        [PrimeraLetra]
        public required string Apellidos { get; set; }
        [StringLength(20, ErrorMessage = "El campo {0} debe tener {1} caracteres o menos")]

        public string? identificacion { get; set; }
        public List<AutorLibro> Libros { get; set; } = [];

    }
}
