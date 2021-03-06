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
                .ForMember(u => u.IdentificacionTercero, opt => opt.MapFrom(u => u.Tercero.NumeroIdentificacion))
                .ForMember(u => u.NombreTercero, opt => opt.MapFrom(u => u.Tercero.Nombre))
                .ForMember(u => u.ModalidadContrato, opt => opt.MapFrom(u => u.Tercero.ModalidadContrato))
                .ForMember(u => u.TipoPago, opt => opt.MapFrom(u => u.Tercero.TipoPago));
            CreateMap<PlanPagoDto, PlanPago>();

            #endregion PlanPago

            #region Transaccion

            CreateMap<Transaccion, TransaccionDto>();

            #endregion

            #region Parametro Liquidación Tercero

            CreateMap<ParametroLiquidacionTerceroDto, ParametroLiquidacionTercero>();

            #endregion

            #region Deducciones

            CreateMap<Deduccion, DeduccionDto>();

            #endregion

            #region Formato Solicitud Pago

            CreateMap<FormatoSolicitudPagoParaGuardarDto, FormatoSolicitudPago>();

            #endregion Formato Solicitud Pago

            #region Clave Presupuestal Contable

            CreateMap<ClavePresupuestalContableDto, ClavePresupuestalContable>()
                .ForMember(u => u.TerceroId, opt => opt.MapFrom(u => u.Tercero.Id))
                .ForMember(u => u.RubroPresupuestalId, opt => opt.MapFrom(u => u.RubroPresupuestal.Id))
                .ForMember(u => u.FuenteFinanciacionId, opt => opt.MapFrom(u => u.FuenteFinanciacion.Id))
                .ForMember(u => u.SituacionFondoId, opt => opt.MapFrom(u => u.SituacionFondo.Id))
                .ForMember(u => u.RecursoPresupuestalId, opt => opt.MapFrom(u => u.RecursoPresupuestal.Id))
                .ForMember(u => u.UsoPresupuestalId, opt => opt.MapFrom(u => u.UsoPresupuestal.Id))
                .ForMember(u => u.RelacionContableId, opt => opt.MapFrom(u => u.RelacionContable.Id));

            #endregion Clave Presupuestal Contable
        }
    }
}