using System.Collections.Generic;
using System.Threading.Tasks;
using ComplementApp.API.Models;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using ComplementApp.API.Dtos;
using ComplementApp.API.Interfaces;
using ComplementApp.API.Helpers;
using System.Globalization;
using System;

namespace ComplementApp.API.Data
{
    public class SolicitudPagoRepository : ISolicitudPagoRepository
    {
        private readonly DataContext _context;
        private readonly IGeneralInterface _generalInterface;

        public SolicitudPagoRepository(DataContext context, IGeneralInterface generalInterface)
        {
            _context = context;
            this._generalInterface = generalInterface;
        }

        #region Registro de Solicitud de Pago

        public async Task<PagedList<CDPDto>> ObtenerCompromisosParaSolicitudRegistroPago(int usuarioId, int perfilId,
                                                                                         int? terceroId, UserParams userParams)
        {
            IQueryable<CDPDto> lista = null;
            var usuario = await _context.Usuario.Where(x => x.UsuarioId == usuarioId).FirstOrDefaultAsync();

            if (perfilId == (int)PerfilUsuario.Administrador || perfilId == (int)PerfilUsuario.CoordinadorFinanciero)
            {
                lista = (from c in _context.CDP
                         join t in _context.Tercero on c.TerceroId equals t.TerceroId
                         where c.Instancia == (int)TipoDocumento.Compromiso
                         where c.PciId == userParams.PciId
                         where c.SaldoActual > 0 //Saldo Disponible
                         where c.TerceroId == terceroId || terceroId == null
                         select new CDPDto()
                         {
                             Crp = c.Crp,
                             Detalle4 = c.Detalle4.Length > 100 ? c.Detalle4.Substring(0, 100) + "..." : c.Detalle4,
                             NumeroIdentificacionTercero = t.NumeroIdentificacion,
                             NombreTercero = t.Nombre,
                         })
                      .Distinct()
                      .OrderBy(x => x.Crp);
            }
            else if (perfilId == (int)PerfilUsuario.SupervisorContractual)
            {
                lista = (from c in _context.CDP
                         join t in _context.Tercero on c.TerceroId equals t.TerceroId
                         join p in _context.ParametroLiquidacionTercero on t.TerceroId equals p.TerceroId into ParametroTercero
                         from pt in ParametroTercero.DefaultIfEmpty()
                         join con in _context.Contrato on c.Crp equals con.ContratoId into Contrato
                         from contra in Contrato.DefaultIfEmpty()
                         where c.Instancia == (int)TipoDocumento.Compromiso
                         where c.PciId == pt.PciId
                         where c.PciId == contra.PciId
                         where c.PciId == userParams.PciId
                         where c.SaldoActual > 0 //Saldo Disponible
                         where contra.Supervisor1Id == usuarioId
                         where c.TerceroId == terceroId || terceroId == null
                         where pt.ModalidadContrato != (int)ModalidadContrato.ContratoPrestacionServicio
                         select new CDPDto()
                         {
                             Crp = c.Crp,
                             Detalle4 = c.Detalle4.Length > 100 ? c.Detalle4.Substring(0, 100) + "..." : c.Detalle4,
                             NumeroIdentificacionTercero = t.NumeroIdentificacion,
                             NombreTercero = t.Nombre,
                         })
                        .Distinct()
                        .OrderBy(x => x.Crp);
            }
            else if (perfilId == (int)PerfilUsuario.Contratista)
            {
                terceroId = usuario.TerceroId;

                lista = (from c in _context.CDP
                         join t in _context.Tercero on c.TerceroId equals t.TerceroId
                         where c.Instancia == (int)TipoDocumento.Compromiso
                         where c.PciId == userParams.PciId
                         where c.SaldoActual > 0 //Saldo Disponible
                         where c.TerceroId == terceroId
                         select new CDPDto()
                         {
                             Crp = c.Crp,
                             Detalle4 = c.Detalle4.Length > 100 ? c.Detalle4.Substring(0, 100) + "..." : c.Detalle4,
                             NumeroIdentificacionTercero = t.NumeroIdentificacion,
                             NombreTercero = t.Nombre,
                         })
                        .Distinct()
                        .OrderBy(x => x.Crp);
            }
            return await PagedList<CDPDto>.CreateAsync(lista, userParams.PageNumber, userParams.PageSize);
        }

        public async Task<PagedList<CDPDto>> ObtenerSolicitudesPagoParaAprobar(int usuarioId, int? terceroId, UserParams userParams)
        {
            IQueryable<CDPDto> lista = null;
            var perfilId = 0;

            var usuarioPerfil = await _context.UsuarioPerfil.Where(x => x.UsuarioId == usuarioId).FirstOrDefaultAsync();
            if (usuarioPerfil != null)
            {
                perfilId = usuarioPerfil.PerfilId;
            }

            if (perfilId == (int)PerfilUsuario.Administrador)
            {
                lista = (from s in _context.FormatoSolicitudPago
                         join c in _context.CDP on s.Crp equals c.Crp
                         join t in _context.Tercero on c.TerceroId equals t.TerceroId
                         where s.EstadoId == (int)EstadoSolicitudPago.Generado
                         where c.Instancia == (int)TipoDocumento.Compromiso
                         where c.PciId == userParams.PciId
                         where s.PciId == c.PciId
                         where c.SaldoActual > 0 //Saldo Disponible                     
                         where c.TerceroId == terceroId || terceroId == null
                         select new CDPDto()
                         {
                             CdpId = s.FormatoSolicitudPagoId,
                             Crp = c.Crp,
                             Detalle4 = c.Detalle4.Length > 100 ? c.Detalle4.Substring(0, 100) + "..." : c.Detalle4,
                             NumeroIdentificacionTercero = t.NumeroIdentificacion,
                             NombreTercero = t.Nombre,
                             FormatoSolicitudPagoId = s.FormatoSolicitudPagoId
                         })
                        .Distinct()
                        .OrderBy(x => x.CdpId);
            }
            else
            {
                lista = (from s in _context.FormatoSolicitudPago
                         join c in _context.CDP on s.Crp equals c.Crp
                         join t in _context.Tercero on c.TerceroId equals t.TerceroId
                         where s.SupervisorId == usuarioId
                         where s.EstadoId == (int)EstadoSolicitudPago.Generado
                         where c.Instancia == (int)TipoDocumento.Compromiso
                         where c.PciId == userParams.PciId
                         where s.PciId == c.PciId
                         where c.SaldoActual > 0 //Saldo Disponible                     
                         where c.TerceroId == terceroId || terceroId == null
                         select new CDPDto()
                         {
                             CdpId = s.FormatoSolicitudPagoId,
                             Crp = c.Crp,
                             Detalle4 = c.Detalle4.Length > 100 ? c.Detalle4.Substring(0, 100) + "..." : c.Detalle4,
                             NumeroIdentificacionTercero = t.NumeroIdentificacion,
                             NombreTercero = t.Nombre,
                             FormatoSolicitudPagoId = s.FormatoSolicitudPagoId
                         })
                        .Distinct()
                        .OrderBy(x => x.CdpId);
            }

            return await PagedList<CDPDto>.CreateAsync(lista, userParams.PageNumber, userParams.PageSize);
        }

        public async Task<FormatoSolicitudPago> ObtenerFormatoSolicitudPagoBase(int formatoSolicitudPagoId)
        {
            return await _context.FormatoSolicitudPago.FirstOrDefaultAsync(u => u.FormatoSolicitudPagoId == formatoSolicitudPagoId);
        }

        public async Task<FormatoSolicitudPagoDto> ObtenerFormatoSolicitudPago(int crp, int pciId)
        {
            var lista = (from c in _context.CDP
                         join t in _context.Tercero on c.TerceroId equals t.TerceroId
                         where c.Crp == crp
                         where c.PciId == pciId
                         where c.Instancia == (int)TipoDocumento.Compromiso
                         select new CDPDto()
                         {
                             Cdp = c.Cdp,
                             Crp = c.Crp,
                             Fecha = c.Fecha, //Fecha compromiso
                             Detalle4 = c.Detalle4, //objeto contrato
                             TerceroId = c.TerceroId,
                             ValorInicial = c.ValorInicial,
                             Operacion = c.Operacion, //Valor adicion/reduccion
                             ValorTotal = c.ValorTotal, //valor actual
                             SaldoActual = c.SaldoActual, //saldo actual
                             Detalle7 = c.Detalle7,
                         });

            var listaAgrupada = (from i in lista
                                 group i by new
                                 {
                                     i.Cdp,
                                     i.Crp,
                                     i.Fecha,
                                     i.Detalle4,
                                     i.TerceroId,
                                     i.ValorInicial,
                                     i.Operacion,
                                     i.ValorTotal,
                                     i.SaldoActual,
                                     i.Detalle7,
                                 }
                          into grp
                                 select new CDPDto()
                                 {
                                     Cdp = grp.Key.Cdp,
                                     Crp = grp.Key.Crp,
                                     Fecha = grp.Key.Fecha,
                                     Detalle4 = grp.Key.Detalle4,
                                     Detalle7 = grp.Key.Detalle7,
                                     TerceroId = grp.Key.TerceroId,
                                     ValorInicial = grp.Sum(i => i.SaldoActual),
                                     Operacion = grp.Sum(i => i.Operacion),
                                     ValorTotal = grp.Sum(i => i.ValorTotal),
                                     SaldoActual = grp.Sum(i => i.SaldoActual)
                                 });

            var formato = await (from c in listaAgrupada
                                 join t in _context.Tercero on c.TerceroId equals t.TerceroId
                                 join co in _context.Contrato on c.Crp equals co.Crp
                                 join p in _context.ParametroLiquidacionTercero on t.TerceroId equals p.TerceroId into ParametroTercero
                                 from pt in ParametroTercero.DefaultIfEmpty()
                                 join ap in _context.TipoAdminPila on pt.TipoAdminPilaId equals ap.TipoAdminPilaId into AdminPila
                                 from adminPila in AdminPila.DefaultIfEmpty()

                                 join sup1 in _context.Usuario on co.Supervisor1Id equals sup1.UsuarioId into Supervisor1
                                 from super1 in Supervisor1.DefaultIfEmpty()
                                 join carg1 in _context.Cargo on super1.CargoId equals carg1.CargoId into Cargo1
                                 from cargoSuper1 in Cargo1.DefaultIfEmpty()
                                 join tercSup1 in _context.Tercero on super1.TerceroId equals tercSup1.TerceroId into TerceroSupervisor1
                                 from tercSuper1 in TerceroSupervisor1.DefaultIfEmpty()

                                 join sup2 in _context.Usuario on co.Supervisor2Id equals sup2.UsuarioId into Supervisor2
                                 from super2 in Supervisor2.DefaultIfEmpty()
                                 join carg2 in _context.Cargo on super2.CargoId equals carg2.CargoId into Cargo2
                                 from cargoSuper2 in Cargo2.DefaultIfEmpty()
                                 join tercSup2 in _context.Tercero on super2.TerceroId equals tercSup2.TerceroId into TerceroSupervisor2
                                 from tercSuper2 in TerceroSupervisor2.DefaultIfEmpty()
                                 where pt.PciId == pciId
                                 where co.PciId == pciId

                                 select new FormatoSolicitudPagoDto()
                                 {
                                     FormatoSolicitudPagoId = 666,
                                     FechaSistema = _generalInterface.ObtenerFechaHoraActual(),
                                     TipoAdminPila = adminPila.TipoAdminPilaId > 0 ? adminPila.Nombre : string.Empty,

                                     Cdp = new CDPDto()
                                     {
                                         Cdp = c.Cdp,
                                         Crp = c.Crp,
                                         Fecha = c.Fecha, //Fecha compromiso
                                         Detalle4 = c.Detalle4, //objeto contrato
                                         Detalle7 = c.Detalle7, //modalidad de contrato
                                         ValorInicial = c.ValorInicial,
                                         Operacion = c.Operacion, //Valor adicion/reduccion
                                         ValorTotal = c.ValorTotal, //valor actual
                                         SaldoActual = c.SaldoActual, //saldo actual
                                         SupervisorId = super1.UsuarioId,
                                     },
                                     Contrato = new ContratoDto()
                                     {
                                         ContratoId = co.ContratoId,
                                         NumeroContrato = co.NumeroContrato,
                                         Crp = co.Crp,
                                         FechaInicio = co.FechaInicio,
                                         FechaFinal = co.FechaFinal,
                                         FechaRegistro = co.FechaRegistro,
                                         FechaExpedicionPoliza = co.FechaExpedicionPoliza,

                                         Supervisor1 = new UsuarioParaDetalleDto()
                                         {
                                             UsuarioId = super1.UsuarioId > 0 ? super1.UsuarioId : 0,
                                             Nombres = super1.Nombres,
                                             Apellidos = super1.Apellidos,
                                             NombreCompleto = co.Supervisor1Id > 0 ? super1.Nombres + " " + super1.Apellidos : string.Empty,
                                             CargoNombre = cargoSuper1.CargoId > 0 ? cargoSuper1.Nombre : string.Empty,
                                             NumeroIdentificacion = tercSuper1.NumeroIdentificacion,
                                         },
                                         Supervisor2 = new UsuarioParaDetalleDto()
                                         {
                                             UsuarioId = co.Supervisor2Id.HasValue ? co.Supervisor2Id.Value : 0,
                                             Nombres = super2.Nombres,
                                             Apellidos = super2.Apellidos,
                                             NombreCompleto = co.Supervisor2Id > 0 ? super2.Nombres + " " + super2.Apellidos : string.Empty,
                                             CargoNombre = cargoSuper2.CargoId > 0 ? cargoSuper2.Nombre : string.Empty,
                                             NumeroIdentificacion = tercSuper2.NumeroIdentificacion,
                                         }
                                     },
                                     Tercero = new TerceroDto()
                                     {
                                         TerceroId = t.TerceroId,
                                         Nombre = t.Nombre,
                                         NumeroIdentificacion = t.NumeroIdentificacion,
                                         Email = t.Email,
                                         Telefono = t.Telefono,
                                         Direccion = t.Direccion.Length > 45 ? t.Direccion.Substring(0, 45) + "..." : t.Direccion,
                                         FechaExpedicionDocumento = t.FechaExpedicionDocumento.HasValue ? (t.FechaExpedicionDocumento.Value.ToString() != "0001-01-01 00:00:00.0000000" ? t.FechaExpedicionDocumento.Value : null) : null,
                                         RegimenTributario = t.RegimenTributario,
                                         DeclaranteRentaDescripcion = t.DeclaranteRenta ? "SI" : "NO",
                                         FacturadorElectronicoDescripcion = pt.FacturaElectronica ? "SI" : "NO"
                                     },

                                 }).FirstOrDefaultAsync();

            return formato;
        }

        public async Task<FormatoSolicitudPagoDto> ObtenerFormatoSolicitudPagoXId(int formatoSolicitudPagoId)
        {
            var lista = (from s in _context.FormatoSolicitudPago
                         join c in _context.CDP on s.Crp equals c.Crp
                         join t in _context.Tercero on c.TerceroId equals t.TerceroId
                         where s.FormatoSolicitudPagoId == formatoSolicitudPagoId
                         where c.Instancia == (int)TipoDocumento.Compromiso
                         where s.PciId == c.PciId
                         select new CDPDto()
                         {
                             Cdp = c.Cdp,
                             Crp = c.Crp,
                             Fecha = c.Fecha, //Fecha compromiso
                             Detalle4 = c.Detalle4, //objeto contrato
                             TerceroId = c.TerceroId,
                             Detalle7 = c.Detalle7,//Modalidad de contrato
                             ValorInicial = c.ValorInicial,
                             Operacion = c.Operacion, //Valor adicion/reduccion
                             ValorTotal = c.ValorTotal, //valor actual
                             SaldoActual = c.SaldoActual, //saldo actual
                         });

            var listaAgrupada = (from i in lista
                                 group i by new
                                 {
                                     i.Cdp,
                                     i.Crp,
                                     i.Fecha,
                                     i.Detalle4,
                                     i.TerceroId,
                                     i.Detalle7,
                                     i.ValorInicial,
                                     i.Operacion,
                                     i.ValorTotal,
                                     i.SaldoActual
                                 }
                          into grp
                                 select new CDPDto()
                                 {
                                     Cdp = grp.Key.Cdp,
                                     Crp = grp.Key.Crp,
                                     Fecha = grp.Key.Fecha,
                                     Detalle4 = grp.Key.Detalle4,
                                     TerceroId = grp.Key.TerceroId,
                                     Detalle7 = grp.Key.Detalle7,
                                     ValorInicial = grp.Sum(i => i.SaldoActual),
                                     Operacion = grp.Sum(i => i.Operacion),
                                     ValorTotal = grp.Sum(i => i.ValorTotal),
                                     SaldoActual = grp.Sum(i => i.SaldoActual)
                                 });

            var formato = await (from s in _context.FormatoSolicitudPago
                                 join c in listaAgrupada on s.Crp equals c.Crp
                                 join t in _context.Tercero on c.TerceroId equals t.TerceroId
                                 join ae in _context.ActividadEconomica on s.ActividadEconomicaId equals ae.ActividadEconomicaId
                                 join pp in _context.PlanPago on s.PlanPagoId equals pp.PlanPagoId
                                 join co in _context.Contrato on c.Crp equals co.Crp
                                 join plt in _context.ParametroLiquidacionTercero on t.TerceroId equals plt.TerceroId into ParametroTercero
                                 from par in ParametroTercero.DefaultIfEmpty()
                                 join ap in _context.TipoAdminPila on par.TipoAdminPilaId equals ap.TipoAdminPilaId into AdminPila
                                 from adminPila in AdminPila.DefaultIfEmpty()

                                 join sup1 in _context.Usuario on co.Supervisor1Id equals sup1.UsuarioId into Supervisor1
                                 from super1 in Supervisor1.DefaultIfEmpty()
                                 join carg1 in _context.Cargo on super1.CargoId equals carg1.CargoId into Cargo1
                                 from cargoSuper1 in Cargo1.DefaultIfEmpty()
                                 join tercSup1 in _context.Tercero on super1.TerceroId equals tercSup1.TerceroId into TerceroSupervisor1
                                 from tercSuper1 in TerceroSupervisor1.DefaultIfEmpty()

                                 join sup2 in _context.Usuario on co.Supervisor2Id equals sup2.UsuarioId into Supervisor2
                                 from super2 in Supervisor2.DefaultIfEmpty()
                                 join carg2 in _context.Cargo on super2.CargoId equals carg2.CargoId into Cargo2
                                 from cargoSuper2 in Cargo2.DefaultIfEmpty()
                                 join tercSup2 in _context.Tercero on super2.TerceroId equals tercSup2.TerceroId into TerceroSupervisor2
                                 from tercSuper2 in TerceroSupervisor2.DefaultIfEmpty()

                                 where s.FormatoSolicitudPagoId == formatoSolicitudPagoId
                                 where s.PciId == pp.PciId
                                 where s.PciId == co.PciId
                                 where s.PciId == par.PciId
                                 select new FormatoSolicitudPagoDto()
                                 {
                                     FormatoSolicitudPagoId = s.FormatoSolicitudPagoId,
                                     PlanPagoId = s.PlanPagoId,
                                     FechaInicio = s.FechaInicio,
                                     FechaFinal = s.FechaFinal,
                                     FechaSistema = s.FechaRegistro.Value,
                                     ValorFacturado = s.valorFacturado,
                                     MesId = s.MesId,
                                     MesDescripcion = DateTimeFormatInfo.CurrentInfo.GetMonthName(s.MesId).ToUpper(),
                                     NumeroPlanilla = s.NumeroPlanilla,
                                     NumeroFactura = s.NumeroFactura,
                                     Observaciones = s.Observaciones,
                                     BaseCotizacion = s.BaseCotizacion,
                                     TipoAdminPila = adminPila.TipoAdminPilaId > 0 ? adminPila.Nombre : string.Empty,

                                     Cdp = new CDPDto()
                                     {
                                         Cdp = c.Cdp,
                                         Crp = c.Crp,
                                         Fecha = c.Fecha, //Fecha compromiso
                                         Detalle4 = c.Detalle4, //objeto contrato
                                         Detalle7 = c.Detalle7, //Modalidad de Contrato
                                         ValorInicial = c.ValorInicial,
                                         Operacion = c.Operacion, //Valor adicion/reduccion
                                         ValorTotal = c.ValorTotal, //valor actual
                                         SaldoActual = c.SaldoActual, //saldo actual
                                     },
                                     ActividadEconomica = new ValorSeleccion()
                                     {
                                         Id = ae.ActividadEconomicaId,
                                         Codigo = ae.Codigo,
                                         Nombre = ae.Nombre,
                                     },
                                     Contrato = new ContratoDto()
                                     {
                                         ContratoId = co.ContratoId,
                                         NumeroContrato = co.NumeroContrato,
                                         Crp = co.Crp,
                                         FechaInicio = co.FechaInicio,
                                         FechaFinal = co.FechaFinal,
                                         FechaRegistro = co.FechaRegistro,
                                         FechaExpedicionPoliza = co.FechaExpedicionPoliza,

                                         Supervisor1 = new UsuarioParaDetalleDto()
                                         {
                                             UsuarioId = super1.UsuarioId > 0 ? super1.UsuarioId : 0,
                                             Nombres = super1.Nombres,
                                             Apellidos = super1.Apellidos,
                                             NombreCompleto = co.Supervisor1Id > 0 ? super1.Nombres + " " + super1.Apellidos : string.Empty,
                                             CargoNombre = cargoSuper1.CargoId > 0 ? cargoSuper1.Nombre : string.Empty,
                                             NumeroIdentificacion = tercSuper1.NumeroIdentificacion,
                                         },
                                         Supervisor2 = new UsuarioParaDetalleDto()
                                         {
                                             UsuarioId = co.Supervisor2Id.HasValue ? co.Supervisor2Id.Value : 0,
                                             Nombres = co.Supervisor2Id.HasValue ? super2.Nombres : string.Empty,
                                             Apellidos = super2.UsuarioId > 0 ? super2.Apellidos : string.Empty,
                                             NombreCompleto = super2.UsuarioId > 0 ? super2.Nombres + " " + super2.Apellidos : string.Empty,
                                             CargoNombre = cargoSuper2.CargoId > 0 ? cargoSuper2.Nombre : string.Empty,
                                             NumeroIdentificacion = tercSuper2.NumeroIdentificacion,
                                         }
                                     },
                                     Tercero = new TerceroDto()
                                     {
                                         TerceroId = t.TerceroId,
                                         Nombre = t.Nombre,
                                         NumeroIdentificacion = t.NumeroIdentificacion,
                                         Email = t.Email,
                                         Telefono = t.Telefono,
                                         Direccion = t.Direccion.Length > 45 ? t.Direccion.Substring(0, 45) + "..." : t.Direccion,
                                         FechaExpedicionDocumento = t.FechaExpedicionDocumento.HasValue ? (t.FechaExpedicionDocumento.Value.ToString() != "0001-01-01 00:00:00.0000000" ? t.FechaExpedicionDocumento.Value : null) : null,
                                         RegimenTributario = t.RegimenTributario,
                                         DeclaranteRentaDescripcion = t.DeclaranteRenta ? "SI" : "NO",
                                         FacturadorElectronicoDescripcion = par.FacturaElectronica ? "SI" : "NO",
                                         ModalidadContrato = par.TerceroId > 0 ? par.ModalidadContrato : 0,
                                     },
                                     PlanPago = new PlanPagoDto()
                                     {
                                         PlanPagoId = pp.PlanPagoId,
                                         NumeroPago = pp.NumeroPago,
                                         ValorFacturado = pp.ValorFacturado.HasValue ? pp.ValorFacturado.Value : 0,
                                         ViaticosDescripcion = pp.Viaticos ? "SI" : "NO",
                                     }
                                 }).FirstOrDefaultAsync();
            return formato;
        }

        public async Task<ICollection<CDPDto>> ObtenerPagosRealizadosXCompromiso(long crp, int pciId)
        {
            var lista = await (from c in _context.CDP
                               where c.Instancia == (int)TipoDocumento.OrdenPago
                               where c.PciId == pciId
                               where c.Crp == crp
                               select new CDPDto()
                               {
                                   Cdp = c.Cdp,
                                   Crp = c.Crp,
                                   OrdenPago = c.OrdenPago,
                                   Detalle1 = c.Detalle1.ToUpper(), //Estado OP
                                   Fecha = c.Fecha, //Fecha Orden Pago
                                   Detalle5 = c.Detalle5, //Supervisor
                                   Detalle4 = c.Detalle4, //objeto contrato
                                   ValorInicial = c.ValorInicial, //Valor Bruto
                                   Operacion = c.Operacion, //Valor deducciones
                                   ValorTotal = c.ValorTotal, //valor neto
                               })
                                 .Distinct()
                                 .OrderBy(c => c.OrdenPago)
                                 .ToListAsync();
            return lista;
        }

        public async Task<Numeracion> ObtenerUltimaNumeracionDisponible(int pciId)
        {
            Numeracion numeracion = new Numeracion();
            var lista = await (from c in _context.Numeracion
                               where c.PciId == pciId
                               where c.Utilizado == false
                               select c)
                              .ToListAsync();

            if (lista != null && lista.Count > 0)
            {
                numeracion = lista.OrderBy(x => x.Consecutivo).First();
            }
            return numeracion;
        }

        public async Task<Numeracion> ObtenerNumeracionBase(int numeracionId)
        {
            var numeracion = await (from c in _context.Numeracion
                                    where c.NumeracionId == numeracionId
                                    select c)
                              .FirstOrDefaultAsync();


            return numeracion;
        }

        public async Task<Numeracion> ObtenerNumeracionxNumeroFactura(string numeroFactura)
        {
            var numeracion = await (from c in _context.Numeracion
                                    where c.NumeroConsecutivo == numeroFactura
                                    select c)
                              .FirstOrDefaultAsync();

            return numeracion;
        }


        #endregion Registro de Solicitud de Pago
        public async Task<ICollection<FormatoSolicitudPago>> ObtenerListaSolicitudPagoXPlanPagoIds(List<int> planPagoIds)
        {
            return await (from sp in _context.FormatoSolicitudPago
                          where planPagoIds.Contains(sp.PlanPagoId)
                          where sp.EstadoId == (int)EstadoSolicitudPago.Aprobado
                          select sp
                          ).ToListAsync();
        }
        public async Task<FormatoSolicitudPago> ObtenerSolicitudPagoXPlanPagoId(int planPagoId)
        {
            return await (from sp in _context.FormatoSolicitudPago
                          where sp.PlanPagoId == planPagoId
                          where sp.EstadoId == (int)EstadoSolicitudPago.Aprobado
                          select sp
                          ).FirstOrDefaultAsync();
        }
        static string UppercaseFirst(string s)
        {
            // Check for empty string.
            if (string.IsNullOrEmpty(s))
            {
                return string.Empty;
            }
            // Return char and concat substring.
            return char.ToUpper(s[0]) + s.Substring(1);
        }
    }
}