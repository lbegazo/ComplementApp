using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using ComplementApp.API.Dtos;
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
                             FechaRadicadoSupervisor = dl.FechaRadicado,
                             ValorTotal = c.ValorFacturado.Value
                         });

            if (lista != null)
            {
                lista.OrderBy(c => c.FechaRadicadoSupervisor);
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

        public async Task<ICollection<DetalleLiquidacionParaArchivo>> ObtenerListaDetalleLiquidacionParaArchivo(List<int> listaLiquidacionId)
        {
            List<DetalleLiquidacionParaArchivo> listaFinal = null;

            var lista = await (from dl in _context.DetalleLiquidacion
                               join t in _context.Tercero on dl.TerceroId equals t.TerceroId
                               join p in _context.ParametroLiquidacionTercero on dl.TerceroId equals p.TerceroId into parametroLiquidacion
                               from pl in parametroLiquidacion.DefaultIfEmpty()
                               where (listaLiquidacionId.Contains(dl.DetalleLiquidacionId))
                               where (dl.Procesado == false)
                               select new DetalleLiquidacionParaArchivo()
                               {
                                   PCI = "23-09-00",
                                   Fecha = System.DateTime.Now.ToString("yyyy-MM-dd"),
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

        public async Task<ICollection<ActividadEconomica>> ObtenerListaActividadesEconomicaXTercero(int terceroId)
        {
            var lista = await (from ae in _context.ActividadEconomica
                                                    join td in _context.TerceroDeducciones on ae.ActividadEconomicaId equals td.ActividadEconomicaId
                                                    where td.TerceroId == terceroId
                                                    select ae)
                                            .Distinct()
                                            .ToListAsync();

            return lista;
        }
    }
}