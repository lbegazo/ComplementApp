using System.Linq;
using AutoMapper;
using ComplementApp.API.Dtos;
using ComplementApp.API.Models;

namespace ComplementApp.API.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            #region User

            CreateMap<User, UserForDetailedDto>()
                    .ForMember(dest => dest.PhotoUrl,
                               opt => opt.MapFrom(src => src.Photos.FirstOrDefault(p => p.IsMain).Url))
                    .ForMember(dest => dest.Age,
                                opt => opt.MapFrom(src => src.DayOfBirth.CalculateAge()));
            CreateMap<User, UserForListDto>()
                    .ForMember(dest => dest.PhotoUrl,
                                opt => opt.MapFrom(src => src.Photos.FirstOrDefault(p => p.IsMain).Url))
                    .ForMember(dest => dest.Age,
                               opt => opt.MapFrom(src => src.DayOfBirth.CalculateAge()));
            CreateMap<Photo, PhotoForDetailedDto>();
            CreateMap<UserForUpdateDto, User>();
            CreateMap<UserForRegisterDto, User>();

            #endregion

            #region Usuario

            CreateMap<Usuario, UsuarioParaDetalleDto>()
                .ForMember(u => u.AreaDescripcion, opt => opt.MapFrom(u => u.Cargo.Descripcion))
                .ForMember(u => u.CargoDescripcion, opt => opt.MapFrom(u => u.Area.Descripcion));

            CreateMap<UsuarioParaRegistrarDto, Usuario>();
            CreateMap<UsuarioParaActualizar, Usuario>();
            CreateMap<UsuarioParaDetalleDto, Usuario>();


            #endregion User
        }
    }
}