using AutoMapper;
using BibliotecaAPI.DTOs;
using BibliotecaAPI.Entities;

namespace BibliotecaAPI.Utilidades
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<Autor, AutorDto>()
                .ForMember(dto => dto.NombreCompleto,
                config => config.MapFrom(autor => $"{autor.Nombres} {autor.Apellidos}"));
            CreateMap<AutorCreacionDto, Autor>();
        }

    }
}
