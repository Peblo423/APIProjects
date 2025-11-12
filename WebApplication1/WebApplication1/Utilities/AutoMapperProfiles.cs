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
                config => config.MapFrom(ent => MapNombreYApellidoAutor(ent)));

            // De Autor a AutorConLibrosDto 
            CreateMap<Autor, AutorConLibrosDto>()
                .ForMember(dto => dto.NombreCompleto,
                config => config.MapFrom(ent => MapNombreYApellidoAutor(ent)));

            // De AutorCreacionDto a Autor
            CreateMap<AutorCreacionDto, Autor>();

            // De Autor a AutorPatchDto
            CreateMap<Autor, AutorPatchDto>().ReverseMap();
            #endregion

            #region MapeoLibros
            // De Libro a LibroDto
            CreateMap<Libro, LibroDto>();

            // De Libro a LibroDto
            CreateMap<Libro, LibroConAutoresDto>();

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

            #region MapeoAutoresLibros

            CreateMap<AutorLibro, LibroDto>()
                .ForMember(dto => dto.Id,
                config => config.MapFrom(ent => 
                ent.LibroId))
                .ForMember(dto => dto.Título,
                config => config.MapFrom(ent => 
                ent.Libro!.Título));

            CreateMap<AutorLibro, AutorDto>()
                .ForMember(dto => dto.Id,
                config => config.MapFrom(ent =>
                ent.AutorId))
                .ForMember(dto => dto.NombreCompleto,
                config => config.MapFrom(ent =>
                MapNombreYApellidoAutor(ent.Autor!)));

            CreateMap<LibroCreacionDto, AutorLibro>()
                .ForMember(ent => ent.Libro,
                config => config.MapFrom(dto =>
                new Libro { Título = dto.Título }));
            #endregion
        }
        private string MapNombreYApellidoAutor(Autor autor)
        {
            return $"{autor.Nombres} {autor.Apellidos}";
        }
    }
}
