using AutoMapper;
using BibliotecaAPI.DTOs;
using BibliotecaAPI.Entities;

namespace BibliotecaAPI.Utilidades
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            #region MapeoAutores
            // De Autor a AutorDto 
            CreateMap<Autor, AutorDto>()
                .ForMember(dto => dto.NombreCompleto,
                config => config.MapFrom(autor => MapNombreYApellidoAutor(autor)));

            // De AutorCreacionDto a Autor
            CreateMap<AutorCreacionDto, Autor>();

            CreateMap<Autor, AutorPatchDto>().ReverseMap();
            #endregion

            #region MapeoLibros
            // De Libro a LibroDto
            //CreateMap<Libro, LibroDto>()
            //    .ForMember(dto => dto.AutorNombre, config =>
            //    config.MapFrom(ent => MapNombreYApellidoAutor(ent.Autor!)));

            // De LibroCreacionDto a Libro
            CreateMap<LibroCreacionDto, Libro>()

                // Regla especial para el miembro 'Autores' (de la entidad Libro)
                .ForMember( ent => ent.Autores,

                    // "La configuración es: TOMA LOS DATOS DESDE..."
                    config => config.MapFrom(dto =>

                        // "...el 'dto.AutoresIds' (la List<int>)..."
                        dto.AutoresIds.Select(id =>

                        // "...y por CADA 'id' en esa lista, 
                        //    crea un NUEVO objeto 'AutorLibro' 
                        //    y pon ese 'id' en su propiedad 'AutorId'."
                        new AutorLibro { AutorId = id })));
            #endregion

            #region MapeoComentarios

            CreateMap<Comentario,ComentarioDto>();
            CreateMap<ComentarioCreacionDto, Comentario>();
            CreateMap <ComentarioPatchDto, Comentario>().ReverseMap();

            #endregion
        }
        private string MapNombreYApellidoAutor(Autor autor) => $"{autor.Nombres} {autor.Apellidos}";

    }
}
