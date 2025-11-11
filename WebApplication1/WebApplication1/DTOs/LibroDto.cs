namespace BibliotecaAPI.DTOs
{
    public class LibroDto
    {
        public int Id { get; set; }
        public required string Título { get; set; }
        public int AutorId { get; set; }
        public required string AutorNombre { get; set; }
    }
}
