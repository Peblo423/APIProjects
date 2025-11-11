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
            CreateMap<Libro, LibroDto>()
                .ForMember(dto => dto.AutorNombre, config => 
                config.MapFrom(ent => MapNombreYApellidoAutor(ent.Autor!)));

            // De LibroCreacionDto a Libro
            CreateMap<LibroCreacionDto, Libro>();
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
