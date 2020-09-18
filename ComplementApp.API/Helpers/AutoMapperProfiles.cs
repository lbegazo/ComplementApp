using System.Globalization;
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

            #endregion User

            #region Usuario

            CreateMap<Usuario, UsuarioParaDetalleDto>()
                .ForMember(u => u.AreaNombre, opt => opt.MapFrom(u => u.Area.Nombre))
                .ForMember(u => u.CargoNombre, opt => opt.MapFrom(u => u.Cargo.Nombre));

            CreateMap<UsuarioParaRegistrarDto, Usuario>();
            CreateMap<UsuarioParaActualizar, Usuario>();
            CreateMap<UsuarioParaDetalleDto, Usuario>();

            #endregion Usuario

            #region PlanPago

            CreateMap<PlanPago, PlanPagoDto>()
                .ForMember(u => u.ViaticosDescripcion, opt => opt.MapFrom(u => u.Viaticos ? "SI" : "NO"))
                .ForMember(u => u.MesPagoDescripcion, opt => opt.MapFrom(u => u.MesPago > 0 && u.MesPago < 13 ? CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(u.MesPago).ToUpper() : string.Empty))
                .ForMember( u => u.IdentificacionTercero, opt => opt.MapFrom(u => u.Tercero.NumeroIdentificacion))
                .ForMember( u => u.NombreTercero, opt => opt.MapFrom(u => u.Tercero.Nombre));
            CreateMap<PlanPagoDto, PlanPago>();

            #endregion PlanPago

            #region Transaccion

            CreateMap<Transaccion, TransaccionDto>();

            #endregion
        
            #region Parametro Liquidación Tercero

            CreateMap<ParametroLiquidacionTerceroDto, ParametroLiquidacionTercero>();

            #endregion
        }
    }
}