using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using ComplementApp.API.Dtos;
using ComplementApp.API.Dtos.Archivo;
using ComplementApp.API.Helpers;
using ComplementApp.API.Interfaces;
using ComplementApp.API.Models;
using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;

namespace ComplementApp.API.Data
{
    public class DetalleLiquidacionRepository : IDetalleLiquidacionRepository
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        private readonly IGeneralInterface _generalInterface;

        public DetalleLiquidacionRepository(DataContext context, IMapper mapper, IGeneralInterface generalInterface)
        {
            _mapper = mapper;
            _context = context;
            this._generalInterface = generalInterface;
        }

        public async Task RegistrarDetalleLiquidacion(DetalleLiquidacion detalleLiquidacion)
        {
            await _context.DetalleLiquidacion.AddAsync(detalleLiquidacion);
        }

        public async Task<FormatoCausacionyLiquidacionPagos> ObtenerDetalleFormatoCausacionyLiquidacionPago(long detalleLiquidacionId)
        {
            DeduccionDto deduccionDto = null;

            var detalleLiquidacion = await (from dl in _context.DetalleLiquidacion
                                            join pp in _context.PlanPago on dl.PlanPagoId equals pp.PlanPagoId
                                            where dl.DetalleLiquidacionId == detalleLiquidacionId
                                            select new FormatoCausacionyLiquidacionPagos()
                                            {
                                                //Plan de pago
                                                DetalleLiquidacionId = dl.DetalleLiquidacionId,
                                                PlanPagoId = dl.PlanPagoId,
                                                TerceroId = dl.TerceroId,
                                                ModalidadContrato = dl.ModalidadContrato,
                                                IdentificacionTercero = dl.NumeroIdentificacion,
                                                NombreTercero = dl.Nombre,
                                                Contrato = dl.Contrato,
                                                Viaticos = pp.Viaticos,
                                                ViaticosDescripcion = dl.Viaticos,
                                                Crp = dl.Crp.ToString(),
                                                CantidadPago = dl.CantidadPago,
                                                NumeroPago = dl.NumeroPago,

                                                ValorContrato = dl.ValorContrato,
                                                ValorAdicionReduccion = dl.ValorAdicionReduccion,
                                                ValorCancelado = dl.ValorCancelado,
                                                TotalACancelar = dl.TotalACancelar,
                                                SaldoActual = dl.SaldoActual,
                                                IdentificacionRubroPresupuestal = dl.RubroPresupuestal,
                                                IdentificacionUsoPresupuestal = dl.UsoPresupuestal,

                                                NombreSupervisor = dl.NombreSupervisor,
                                                NumeroRadicadoSupervisor = dl.NumeroRadicado,
                                                FechaRadicadoSupervisor = dl.FechaRadicado,
                                                NumeroFactura = dl.NumeroFactura,

                                                TextoComprobanteContable = dl.TextoComprobanteContable,

                                                //Formato de Liquidación
                                                Honorario = dl.Honorario,
                                                HonorarioUvt = (int)dl.HonorarioUvt,
                                                ValorIva = dl.ValorIva,
                                                ValorTotal = dl.ValorTotal,
                                                TotalRetenciones = dl.TotalRetenciones,
                                                TotalAGirar = dl.TotalAGirar,

                                                BaseSalud = dl.BaseSalud,
                                                AporteSalud = (int)dl.AporteSalud,
                                                AportePension = dl.AportePension,
                                                RiesgoLaboral = dl.RiesgoLaboral,
                                                FondoSolidaridad = dl.FondoSolidaridad,
                                                ImpuestoCovid = dl.ImpuestoCovid,
                                                SubTotal1 = dl.SubTotal1,

                                                PensionVoluntaria = dl.PensionVoluntaria,
                                                Afc = (int)dl.Afc,
                                                SubTotal2 = dl.SubTotal2,
                                                MedicinaPrepagada = dl.MedicinaPrepagada,
                                                Dependientes = dl.Dependientes,
                                                InteresVivienda = dl.InteresesVivienda,
                                                TotalDeducciones = dl.TotalDeducciones,

                                                SubTotal3 = dl.SubTotal3,
                                                RentaExenta = (int)dl.RentaExenta,
                                                LimiteRentaExenta = dl.LimiteRentaExenta,
                                                TotalRentaExenta = dl.TotalRentaExenta,
                                                DiferencialRenta = dl.DiferencialRenta,
                                                BaseGravableRenta = dl.BaseGravableRenta,
                                                BaseGravableUvt = (int)dl.BaseGravableUvt,
                                                ViaticosPagados = dl.ViaticosPagados,
                                                NumeroMesSaludActual = dl.MesSaludActual,
                                                NumeroMesSaludAnterior = dl.MesSaludAnterior,
                                                MesSaludAnterior = (dl.MesSaludAnterior > 0 && dl.MesSaludAnterior < 13) ? CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(dl.MesSaludAnterior).ToUpper() : string.Empty,
                                                MesSaludActual = (dl.MesSaludActual > 0 && dl.MesSaludActual < 13) ? CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(dl.MesSaludActual).ToUpper() : string.Empty
                                            }).FirstOrDefaultAsync();

            if (detalleLiquidacion != null)
            {
                #region Setear datos

                //Deducciones
                var deduccionesLiquidacion = await _context.LiquidacionDeducciones
                                                .Where(x => x.DetalleLiquidacionId == detalleLiquidacion.DetalleLiquidacionId)
                                                .ToListAsync();
                if (deduccionesLiquidacion != null && (deduccionesLiquidacion.Count > 0))
                {
                    detalleLiquidacion.Deducciones = new List<DeduccionDto>();
                    foreach (var deduccion in deduccionesLiquidacion)
                    {
                        deduccionDto = new DeduccionDto();
                        deduccionDto.Codigo = deduccion.Codigo;
                        deduccionDto.Nombre = deduccion.Nombre;
                        deduccionDto.Base = deduccion.Base;
                        deduccionDto.Tarifa = deduccion.Tarifa;
                        deduccionDto.Valor = deduccion.Valor;
                        detalleLiquidacion.Deducciones.Add(deduccionDto);
                    }
                }

                #endregion Setear datos
            }

            return detalleLiquidacion;
        }

        public async Task<PagedList<FormatoCausacionyLiquidacionPagos>> ObtenerListaDetalleLiquidacion(
                    int? terceroId,
                    List<int> listaEstadoId,
                    bool? procesado, UserParams userParams)
        {

            var lista = (from dl in _context.DetalleLiquidacion
                         join c in _context.PlanPago on dl.PlanPagoId equals c.PlanPagoId
                         join e in _context.Estado on c.EstadoPlanPagoId equals e.EstadoId
                         join t in _context.Tercero on c.TerceroId equals t.TerceroId
                         join p in _context.ParametroLiquidacionTercero on c.TerceroId equals p.TerceroId into parametroLiquidacion
                         from pl in parametroLiquidacion.DefaultIfEmpty()
                         where (c.TerceroId == terceroId || terceroId == null)
                         where (listaEstadoId.Contains(c.EstadoPlanPagoId.Value))
                         where (dl.Procesado == procesado || procesado == null)
                         select new FormatoCausacionyLiquidacionPagos()
                         {
                             DetalleLiquidacionId = dl.DetalleLiquidacionId,
                             PlanPagoId = dl.PlanPagoId,
                             IdentificacionTercero = dl.NumeroIdentificacion,
                             NombreTercero = dl.Nombre,
                             NumeroRadicadoSupervisor = dl.NumeroRadicado,
                             FechaRadicadoSupervisor = dl.FechaRegistro.HasValue ? dl.FechaRegistro.Value : _generalInterface.ObtenerFechaHoraActual(),
                             ValorTotal = c.ValorFacturado.HasValue ? c.ValorFacturado.Value : 0,

                         });

            if (lista != null)
            {
                lista.OrderByDescending(c => c.FechaRadicadoSupervisor);
            }

            return await PagedList<FormatoCausacionyLiquidacionPagos>.CreateAsync(lista, userParams.PageNumber, userParams.PageSize);
        }

        public async Task<DetalleLiquidacion> ObtenerDetalleLiquidacionBase(int detalleLiquidacion)
        {
            return await _context.DetalleLiquidacion.FirstOrDefaultAsync(u => u.DetalleLiquidacionId == detalleLiquidacion);
        }

        public async Task<ICollection<DetalleLiquidacion>> ObtenerListaDetalleLiquidacionViaticosAnterior(long terceroId)
        {
            int mesAnterior = _generalInterface.ObtenerFechaHoraActual().AddMonths(-1).Month;
            var detalleLiquidacionAnterior = await (from dl in _context.DetalleLiquidacion
                                                    where dl.TerceroId == terceroId
                                                    where dl.FechaOrdenPago.Value.Month == mesAnterior
                                                    where dl.Viaticos == "SI"
                                                    select dl)
                                            .ToListAsync();

            return detalleLiquidacionAnterior;
        }

        public async Task<DetalleLiquidacion> ObtenerDetalleLiquidacionAnterior(int terceroId)
        {
            DetalleLiquidacion liquidacion = null;
            int mesAnterior = _generalInterface.ObtenerFechaHoraActual().AddMonths(-1).Month;

            var lista = await (from dl in _context.DetalleLiquidacion
                               where dl.TerceroId == terceroId
                               where dl.FechaRegistro.Value.Month == mesAnterior
                               where dl.Viaticos == "NO"
                               select dl).ToListAsync();

            if (lista != null && lista.Count > 0)
            {
                liquidacion = lista.OrderBy(x => x.DetalleLiquidacionId).Last();
            }

            return liquidacion;
        }

        #region Archivo Cuenta Por Pagar
        public async Task<ICollection<DetalleLiquidacionParaArchivo>> ObtenerListaDetalleLiquidacionParaArchivo(List<int> listaLiquidacionId)
        {
            List<DetalleLiquidacionParaArchivo> listaFinal = new List<DetalleLiquidacionParaArchivo>();

            var lista = await (from dl in _context.DetalleLiquidacion
                               join t in _context.Tercero on dl.TerceroId equals t.TerceroId
                               join p in _context.ParametroLiquidacionTercero on dl.TerceroId equals p.TerceroId into parametroLiquidacion
                               from pl in parametroLiquidacion.DefaultIfEmpty()
                               where (listaLiquidacionId.Contains(dl.DetalleLiquidacionId))
                               where (dl.Procesado == false)
                               select new DetalleLiquidacionParaArchivo()
                               {
                                   PCI = "23-09-00",
                                   FechaActual = System.DateTime.Now.ToString("yyyy-MM-dd"),
                                   TipoIdentificacion = t.TipoIdentificacion,
                                   NumeroIdentificacion = t.NumeroIdentificacion,
                                   Crp = dl.Crp,
                                   TipoCuentaPagar = pl.TipoCuentaPorPagar.Value,
                                   TotalACancelar = decimal.Round(dl.TotalACancelar, 2, MidpointRounding.AwayFromZero),
                                   ValorIva = decimal.Round(dl.ValorIva, 2, MidpointRounding.AwayFromZero),
                                   TextoComprobanteContable = dl.TextoComprobanteContable,
                                   TipoDocumentoSoporte = pl.TipoDocumentoSoporte,
                                   NumeroFactura = dl.NumeroFactura,
                                   ConstanteNumero = "16",
                                   NombreSupervisor = dl.NombreSupervisor,
                                   ConstanteCargo = "CargoSupervisor",
                                   FechaRegistro = dl.FechaRegistro.Value
                               })
                    .ToListAsync();

            if (lista != null && lista.Count > 0)
            {
                listaFinal = lista.OrderBy(x => x.FechaRegistro).ToList();
            }

            return listaFinal;
        }

        public async Task<List<int>> ObtenerListaDetalleLiquidacionTotal(int? terceroId, List<int> listaEstadoId, bool? procesado)
        {
            var lista = (from dl in _context.DetalleLiquidacion
                         join c in _context.PlanPago on dl.PlanPagoId equals c.PlanPagoId
                         join e in _context.Estado on c.EstadoPlanPagoId equals e.EstadoId
                         join t in _context.Tercero on c.TerceroId equals t.TerceroId
                         join p in _context.ParametroLiquidacionTercero on c.TerceroId equals p.TerceroId into parametroLiquidacion
                         from pl in parametroLiquidacion.DefaultIfEmpty()
                         where (c.TerceroId == terceroId || terceroId == null)
                         where (listaEstadoId.Contains(c.EstadoPlanPagoId.Value))
                         where (dl.Procesado == procesado || procesado == null)
                         select dl.DetalleLiquidacionId);

            return await lista.ToListAsync();
        }

        public bool RegistrarArchivoDetalleLiquidacion(ArchivoDetalleLiquidacion archivo)
        {
            _context.ArchivoDetalleLiquidacion.Add(archivo);
            return true;
        }

        public bool RegistrarDetalleArchivoLiquidacion(List<DetalleArchivoLiquidacion> listaDetalle)
        {
            _context.BulkInsert(listaDetalle);
            return true;
        }

        public int ObtenerUltimoConsecutivoArchivoLiquidacion()
        {
            var fechaActual = _generalInterface.ObtenerFechaHoraActual();
            var listaArchivo = _context.ArchivoDetalleLiquidacion
                            .Where(x => x.FechaGeneracion.Date == fechaActual.Date).ToList();

            if (listaArchivo == null || (listaArchivo != null && listaArchivo.Count == 0))
            {
                return 0;
            }
            else
            {
                var archivo = listaArchivo.OrderBy(x => x.Consecutivo).Last();
                return archivo.Consecutivo;
            }
        }

        #endregion Archivo Cuenta Por Pagar

        #region Archivo Obligacion Presupuestal

        public async Task<PagedList<FormatoCausacionyLiquidacionPagos>> ObtenerLiquidacionesParaObligacionArchivo(
                    int? terceroId,
                    List<int> listaEstadoId,
                    bool? procesado,
                    UserParams userParams)
        {
            int modalidadContrato = (int)ModalidadContrato.ContratoPrestacionServicio;
            //int estadoPlanPago_ConLiquidacionDeducciones = (int)EstadoPlanPago.ConLiquidacionDeducciones;

            var lista = (from dl in _context.DetalleLiquidacion
                         join c in _context.PlanPago on dl.PlanPagoId equals c.PlanPagoId
                         join e in _context.Estado on c.EstadoPlanPagoId equals e.EstadoId
                         join t in _context.Tercero on c.TerceroId equals t.TerceroId
                         join p in _context.ParametroLiquidacionTercero on c.TerceroId equals p.TerceroId into parametroLiquidacion
                         from pl in parametroLiquidacion.DefaultIfEmpty()
                         where (listaEstadoId.Contains(c.EstadoPlanPagoId.Value))
                         where (pl.ModalidadContrato == modalidadContrato)
                         where (dl.TerceroId == terceroId || terceroId == null)
                         where (dl.Procesado == procesado || procesado == null)
                         select new FormatoCausacionyLiquidacionPagos()
                         {
                             DetalleLiquidacionId = dl.DetalleLiquidacionId,
                             PlanPagoId = dl.PlanPagoId,
                             IdentificacionTercero = dl.NumeroIdentificacion,
                             NombreTercero = dl.Nombre,
                             NumeroRadicadoSupervisor = dl.NumeroRadicado,
                             FechaRadicadoSupervisor = dl.FechaRadicado,
                             ValorTotal = c.ValorFacturado.Value
                         });

            if (lista != null)
            {
                lista.OrderBy(c => c.FechaRadicadoSupervisor);
            }

            return await PagedList<FormatoCausacionyLiquidacionPagos>.CreateAsync(lista, userParams.PageNumber, userParams.PageSize);
        }


        public async Task<List<int>> ObtenerLiquidacionIdsParaArchivoObligacion(int? terceroId,
                                                                                        List<int> listaEstadoId,
                                                                                        bool? procesado)
        {
            int modalidadContrato = (int)ModalidadContrato.ContratoPrestacionServicio;

            var lista = await (from dl in _context.DetalleLiquidacion
                               join c in _context.PlanPago on dl.PlanPagoId equals c.PlanPagoId
                               join e in _context.Estado on c.EstadoPlanPagoId equals e.EstadoId
                               join t in _context.Tercero on c.TerceroId equals t.TerceroId
                               join p in _context.ParametroLiquidacionTercero on c.TerceroId equals p.TerceroId into parametroLiquidacion
                               from pl in parametroLiquidacion.DefaultIfEmpty()
                               where (listaEstadoId.Contains(c.EstadoPlanPagoId.Value))
                               where (pl.ModalidadContrato == modalidadContrato)
                               where (dl.TerceroId == terceroId || terceroId == null)
                               where (dl.Procesado == procesado || procesado == null)
                               select dl.DetalleLiquidacionId).ToListAsync();

            return lista;
        }


        public async Task<ICollection<DetalleLiquidacionParaArchivo>> ObtenerCabeceraParaArchivoObligacion(List<int> listaLiquidacionId)
        {
            List<DetalleLiquidacionParaArchivo> listaFinal = new List<DetalleLiquidacionParaArchivo>();


            var lista = await (from dl in _context.DetalleLiquidacion
                               join t in _context.Tercero on dl.TerceroId equals t.TerceroId
                               join p in _context.ParametroLiquidacionTercero on dl.TerceroId equals p.TerceroId into parametroLiquidacion
                               from pl in parametroLiquidacion.DefaultIfEmpty()
                               join cp in _context.TipoCuentaXPagar on pl.TipoCuentaPorPagar equals cp.TipoCuentaXPagarId into tipoCuentaPagar
                               from tcp in tipoCuentaPagar.DefaultIfEmpty()
                               where (listaLiquidacionId.Contains(dl.DetalleLiquidacionId))

                               select new DetalleLiquidacionParaArchivo()
                               {
                                   FechaActual = System.DateTime.Now.ToString("yyyy/MM/dd"),
                                   PCI = "23-09-00",
                                   Crp = dl.Crp,
                                   Dip = "NO",
                                   TipoCuentaPagarCodigo = tcp.Codigo,
                                   ValorIva = decimal.Round(dl.ValorIva, 2, MidpointRounding.AwayFromZero),
                                   TipoDocumentoSoporte = pl.TipoDocumentoSoporte,
                                   NumeroFactura = dl.NumeroFactura,
                                   ConstanteExpedidor = "11",
                                   ConstanteCargo = "SUPERVISOR",
                                   NombreSupervisor = dl.NombreSupervisor,
                                   TextoComprobanteContable = dl.TextoComprobanteContable,
                                   ValorTotal = decimal.Round(dl.ValorTotal, 2, MidpointRounding.AwayFromZero),
                                   FechaRegistro = dl.FechaRegistro.Value
                               })
                    .ToListAsync();

            if (lista != null && lista.Count > 0)
            {
                listaFinal = lista.OrderBy(x => x.FechaRegistro).ToList();
            }
            return listaFinal;
        }

        public async Task<ICollection<ClavePresupuestalContableParaArchivo>> ObtenerItemsLiquidacionParaArchivoObligacion(List<int> listaLiquidacionId)
        {
            var lista = await (from cpc in _context.DetalleFormatoSolicitudPago
                               join rp in _context.RubroPresupuestal on cpc.RubroPresupuestalId equals rp.RubroPresupuestalId
                               join sp in _context.FormatoSolicitudPago on cpc.FormatoSolicitudPagoId equals sp.FormatoSolicitudPagoId
                               join dl in _context.DetalleLiquidacion on sp.Crp equals dl.Crp
                               join cp in _context.ClavePresupuestalContable on rp.RubroPresupuestalId equals cp.RubroPresupuestalId
                               join rc in _context.RelacionContable on cp.RelacionContableId equals rc.RelacionContableId
                               join cc in _context.CuentaContable on rc.CuentaContableId equals cc.CuentaContableId
                               join sf in _context.SituacionFondo on cp.SituacionFondoId equals sf.SituacionFondoId
                               join ff in _context.FuenteFinanciacion on cp.FuenteFinanciacionId equals ff.FuenteFinanciacionId
                               join rpr in _context.RecursoPresupuestal on cp.RecursoPresupuestalId equals rpr.RecursoPresupuestalId
                               join ac in _context.AtributoContable on rc.AtributoContableId equals ac.AtributoContableId
                               join tg in _context.TipoGasto on rc.TipoGastoId equals tg.TipoGastoId into tipoGasto
                               from tga in tipoGasto.DefaultIfEmpty()
                               where (listaLiquidacionId.Contains(dl.DetalleLiquidacionId))
                               where (sp.PlanPagoId == dl.PlanPagoId)
                               select new ClavePresupuestalContableParaArchivo()
                               {
                                   DetalleLiquidacionId = dl.DetalleLiquidacionId,
                                   Dependencia = cp.Dependencia,
                                   RubroPresupuestalIdentificacion = rp.Identificacion,
                                   RecursoPresupuestalCodigo = rpr.Codigo,
                                   FuenteFinanciacionCodigo = ff.Codigo,
                                   SituacionFondoCodigo = sf.Codigo,
                                   ValorTotal = dl.ValorTotal,
                                   AtributoContableCodigo = ac.Codigo,
                                   TipoGastoCodigo = tga.Codigo,
                                   UsoContable = rc.UsoContable.ToString(),
                                   TipoOperacion = rc.TipoOperacion.ToString(),
                                   NumeroCuenta = cc.NumeroCuenta
                               })
                               .Distinct()
                    .ToListAsync();

            return lista;
        }

        public async Task<ICollection<DeduccionDetalleLiquidacionParaArchivo>> ObtenerDeduccionesLiquidacionParaArchivoObligacion(List<int> listaLiquidacionId)
        {
            var lista = await (from ld in _context.LiquidacionDeducciones
                               join l in _context.DetalleLiquidacion on ld.DetalleLiquidacionId equals l.DetalleLiquidacionId
                               join d in _context.Deduccion on ld.DeduccionId equals d.DeduccionId
                               join td in _context.TerceroDeducciones on ld.DeduccionId equals td.DeduccionId
                               join t in _context.Tercero on td.TerceroDeDeduccionId equals t.TerceroId
                               where (l.TerceroId == td.TerceroId)
                               where (listaLiquidacionId.Contains(ld.DetalleLiquidacionId))
                               select new DeduccionDetalleLiquidacionParaArchivo()
                               {
                                   DetalleLiquidacionId = ld.DetalleLiquidacionId,
                                   DeduccionCodigo = d.Codigo,
                                   TipoIdentificacion = t.TipoIdentificacion,
                                   NumeroIdentificacion = t.NumeroIdentificacion,
                                   Base = Math.Round(ld.Base, 0, MidpointRounding.AwayFromZero),
                                   Tarifa = Math.Round(ld.Tarifa * 100, 5, MidpointRounding.AwayFromZero),
                                   Valor = ld.Valor,
                               })
                    .ToListAsync();

            return lista;
        }

        public async Task<ICollection<DetalleLiquidacionParaArchivo>> ObtenerUsosParaArchivoObligacion(List<int> listaLiquidacionId)
        {
            List<DetalleLiquidacionParaArchivo> listaFinal = new List<DetalleLiquidacionParaArchivo>();

            var lista = await (from cpc in _context.DetalleFormatoSolicitudPago
                               join rp in _context.RubroPresupuestal on cpc.RubroPresupuestalId equals rp.RubroPresupuestalId
                               join sp in _context.FormatoSolicitudPago on cpc.FormatoSolicitudPagoId equals sp.FormatoSolicitudPagoId
                               join dl in _context.DetalleLiquidacion on sp.Crp equals dl.Crp
                               join cp in _context.ClavePresupuestalContable on rp.RubroPresupuestalId equals cp.RubroPresupuestalId
                               join up in _context.UsoPresupuestal on cp.UsoPresupuestalId equals up.UsoPresupuestalId
                               where (listaLiquidacionId.Contains(dl.DetalleLiquidacionId))
                               where (sp.PlanPagoId == dl.PlanPagoId)
                               select new DetalleLiquidacionParaArchivo()
                               {
                                   DetalleLiquidacionId = dl.DetalleLiquidacionId,
                                   UsoPresupuestalCodigo = up.Identificacion,
                                   ValorTotal = cpc.ValorAPagar,
                                   FechaRegistro = dl.FechaRegistro.Value,
                               })
                               .Distinct()
                    .ToListAsync();

            if (lista != null && lista.Count > 0)
            {
                listaFinal = lista.OrderBy(x => x.FechaRegistro).ToList();
            }
            return listaFinal;
        }


        #endregion Archivo Obligacion Presupuestal

        #region Liquidación Masiva

        public bool RegistrarListaDetalleLiquidacion(IList<DetalleLiquidacion> listaDetalleLiquidacion)
        {
            try
            {
                _context.BulkInsertAsync(listaDetalleLiquidacion);
                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ICollection<DetalleLiquidacion>> ObtenerListaDetalleLiquidacionViaticosMesAnteriorXTerceroIds(List<int> listaTerceroId)
        {
            int mesAnterior = _generalInterface.ObtenerFechaHoraActual().AddMonths(-1).Month;

            var detalleLiquidacionAnterior = await (from dl in _context.DetalleLiquidacion
                                                    where listaTerceroId.Contains(dl.TerceroId)
                                                    where dl.FechaOrdenPago.Value.Month == mesAnterior
                                                    where dl.Viaticos == "SI"
                                                    select dl)
                                            .ToListAsync();
            return detalleLiquidacionAnterior;
        }

        public ICollection<DetalleLiquidacion> ObtenerListaDetalleLiquidacionMesAnteriorXTerceroIds(List<int> listaTerceroId)
        {
            int mesAnterior = _generalInterface.ObtenerFechaHoraActual().AddMonths(-1).Month;
            List<DetalleLiquidacion> lista = new List<DetalleLiquidacion>();

            var query1 = (from dl in _context.DetalleLiquidacion
                          where listaTerceroId.Contains(dl.TerceroId)
                          where dl.FechaRegistro.Value.Month == mesAnterior
                          where dl.Viaticos == "NO"
                          group dl by new { dl.TerceroId, dl.DetalleLiquidacionId }
                          into grp
                          select new
                          {
                              grp.Key.TerceroId,
                              grp.Key.DetalleLiquidacionId
                          }).ToList();

            var query2 = (from x in query1
                          group x by new { x.TerceroId, x.DetalleLiquidacionId } into grp
                          select new
                          {
                              TerceroId = grp.Key.TerceroId,
                              DetalleLiquidacionId = (from x in grp
                                                      orderby x.DetalleLiquidacionId descending
                                                      select x.DetalleLiquidacionId).FirstOrDefault()
                          });

            List<int> listaDetalleLiquidacionId = query2.Select(x => x.DetalleLiquidacionId).ToList();

            if (listaDetalleLiquidacionId != null)
            {
                lista = (from dl in _context.DetalleLiquidacion
                         where listaDetalleLiquidacionId.Contains(dl.DetalleLiquidacionId)
                         select new DetalleLiquidacion()
                         {
                             PlanPagoId = dl.PlanPagoId,
                             TerceroId = dl.TerceroId,
                             DetalleLiquidacionId = dl.DetalleLiquidacionId,
                             MesPagoActual = dl.MesPagoActual,
                             MesPagoAnterior = dl.MesPagoAnterior
                         }).ToList();
            }

            return lista;
        }

        #endregion Liquidación Masiva
    }
}