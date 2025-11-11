using Microsoft.EntityFrameworkCore;

namespace BibliotecaAPI.Entities
{

    [PrimaryKey(nameof(LibroId), nameof(AutorId))]
    public class AutorLibro
    {
        public int AutorId { get; set; }
        public int LibroId { get; set; }

        public int Orden { get; set; }
        public Autor? Autor { get; set; }
        public Libro? Libro { get; set; }
    }
}
