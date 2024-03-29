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
using ComplementApp.API.Interfaces.Repository;
using ComplementApp.API.Models;
using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;

namespace ComplementApp.API.Data
{
    public class DetalleLiquidacionRepository : IDetalleLiquidacionRepository
    {

        #region Dependency Injection
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        private readonly IGeneralInterface _generalInterface;
        private readonly IListaRepository _listaRepository;

        #endregion Dependency Injection

        public DetalleLiquidacionRepository(DataContext context, IMapper mapper,
                                            IGeneralInterface generalInterface, IListaRepository listaRepository)
        {
            _mapper = mapper;
            _context = context;
            this._generalInterface = generalInterface;
            this._listaRepository = listaRepository;
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
                                            where dl.PciId == pp.PciId
                                            select new FormatoCausacionyLiquidacionPagos()
                                            {
                                                //Plan de pago
                                                DetalleLiquidacionId = dl.DetalleLiquidacionId,
                                                FormatoSolicitudPagoId = dl.FormatoSolicitudPagoId.Value,
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

        public async Task<PagedList<FormatoCausacionyLiquidacionPagos>> ObtenerLiquidacionesParaCuentaPorPagarArchivo(
                    int? terceroId,
                    List<int> listaEstadoId,
                    bool? procesado, UserParams userParams)
        {
            var listaCompromisoConClave = (from pp in _context.ClavePresupuestalContable
                                           where pp.PciId == userParams.PciId
                                           select pp.Crp).ToHashSet();

            var lista = (from dl in _context.DetalleLiquidacion
                         join c in _context.PlanPago on dl.PlanPagoId equals c.PlanPagoId
                         join t in _context.Tercero on c.TerceroId equals t.TerceroId
                         join p in _context.ParametroLiquidacionTercero on c.TerceroId equals p.TerceroId into parametroLiquidacion
                         from pl in parametroLiquidacion.DefaultIfEmpty()
                         where dl.EstadoId != (int)EstadoDetalleLiquidacion.Rechazado
                         where (dl.Crp == c.Crp)
                         where dl.PciId == c.PciId
                         where dl.PciId == pl.PciId
                         where dl.PciId == userParams.PciId
                         where (c.TerceroId == terceroId || terceroId == null)
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
                             TieneClavePresupuestalContable = listaCompromisoConClave.Contains(c.Crp)
                         }).
                         Distinct();

            if (lista != null)
            {
                lista.OrderByDescending(c => c.DetalleLiquidacionId);
            }

            return await PagedList<FormatoCausacionyLiquidacionPagos>.CreateAsync(lista, userParams.PageNumber, userParams.PageSize);
        }

        public async Task<DetalleLiquidacion> ObtenerDetalleLiquidacionBase(int detalleLiquidacion)
        {
            return await _context.DetalleLiquidacion.FirstOrDefaultAsync(u => u.DetalleLiquidacionId == detalleLiquidacion);
        }

        public async Task<ICollection<DetalleLiquidacion>> ObtenerListaDetalleLiquidacionViaticosAnterior(long terceroId, int pciId)
        {
            int mesAnterior = _generalInterface.ObtenerFechaHoraActual().AddMonths(-1).Month;
            var detalleLiquidacionAnterior = await (from dl in _context.DetalleLiquidacion
                                                    where dl.EstadoId != (int)EstadoDetalleLiquidacion.Rechazado
                                                    where dl.PciId == pciId
                                                    where dl.TerceroId == terceroId
                                                    where dl.FechaOrdenPago.Value.Month == mesAnterior
                                                    where dl.Viaticos == "SI"
                                                    select dl)
                                            .ToListAsync();

            return detalleLiquidacionAnterior;
        }

        public async Task<bool> ExisteLiquidacionAnterior(long crp, long terceroId, int pciId)
        {
            var detalleLiquidacion = await (from dl in _context.DetalleLiquidacion
                                            where dl.EstadoId == (int)EstadoDetalleLiquidacion.Generado
                                            where dl.PciId == pciId
                                            where dl.Crp == crp
                                            where dl.TerceroId == terceroId
                                            select dl)
                                            .ToListAsync();

            var resultado = detalleLiquidacion?.Count > 0 ? true : false;
            return resultado;
        }


        public async Task<DetalleLiquidacion> ObtenerDetalleLiquidacionAnterior(int terceroId, int pciId)
        {
            DetalleLiquidacion liquidacion = null;
            int mesAnterior = _generalInterface.ObtenerFechaHoraActual().AddMonths(-1).Month;

            var lista = await (from dl in _context.DetalleLiquidacion
                               where dl.EstadoId != (int)EstadoDetalleLiquidacion.Rechazado
                               where dl.TerceroId == terceroId
                               where dl.PciId == pciId
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
        public async Task<ICollection<DetalleLiquidacionParaArchivo>> ObtenerListaDetalleLiquidacionParaArchivo(List<int> listaLiquidacionId, int pciId)
        {
            List<DetalleLiquidacionParaArchivo> listaFinal = new List<DetalleLiquidacionParaArchivo>();
            string identificacionPCI = string.Empty;
            var pci = await _listaRepository.ObtenerPci(pciId);

            if (pci != null)
            {
                identificacionPCI = pci.Identificacion;
            }

            var lista = await (from dl in _context.DetalleLiquidacion
                               join t in _context.Tercero on dl.TerceroId equals t.TerceroId
                               join p in _context.ParametroLiquidacionTercero on dl.TerceroId equals p.TerceroId into parametroLiquidacion
                               from pl in parametroLiquidacion.DefaultIfEmpty()
                               join tc in _context.TipoCuentaXPagar on pl.TipoCuentaXPagarId equals tc.TipoCuentaXPagarId into CuentaPorPagar
                               from tiCu in CuentaPorPagar.DefaultIfEmpty()
                               join tds in _context.TipoDocumentoSoporte on pl.TipoDocumentoSoporteId equals
                                                                                  tds.TipoDocumentoSoporteId into DocumentoSoporte
                               from tipoDocu in DocumentoSoporte.DefaultIfEmpty()
                               where dl.PciId == pl.PciId
                               where (listaLiquidacionId.Contains(dl.DetalleLiquidacionId))
                               where dl.PciId == pciId
                               where (dl.Procesado == false)
                               select new DetalleLiquidacionParaArchivo()
                               {
                                   PCI = identificacionPCI,
                                   FechaActual = System.DateTime.Now.ToString("yyyy-MM-dd"),
                                   TipoIdentificacion = t.TipoIdentificacion,
                                   NumeroIdentificacion = t.NumeroIdentificacion,
                                   Crp = dl.Crp,
                                   TipoCuentaXPagarCodigo = pl.TipoCuentaXPagarId > 0 ? tiCu.Codigo : string.Empty,
                                   TotalACancelar = decimal.Round(dl.TotalACancelar, 2, MidpointRounding.AwayFromZero),
                                   ValorIva = decimal.Round(dl.ValorIva, 2, MidpointRounding.AwayFromZero),
                                   TextoComprobanteContable = dl.TextoComprobanteContable.Length > 220 ? dl.TextoComprobanteContable.Substring(0, 220) : dl.TextoComprobanteContable,
                                   TipoDocumentoSoporteCodigo = pl.TipoDocumentoSoporteId > 0 ? tipoDocu.Codigo : string.Empty,
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

        public async Task<List<int>> ObtenerListaDetalleLiquidacionTotal(int pciId, int? terceroId, List<int> listaEstadoId, bool? procesado)
        {
            var lista = (from dl in _context.DetalleLiquidacion
                         join c in _context.PlanPago on dl.PlanPagoId equals c.PlanPagoId
                         join t in _context.Tercero on c.TerceroId equals t.TerceroId
                         join p in _context.ParametroLiquidacionTercero on c.TerceroId equals p.TerceroId into parametroLiquidacion
                         from pl in parametroLiquidacion.DefaultIfEmpty()
                         where dl.EstadoId != (int)EstadoDetalleLiquidacion.Rechazado
                         where dl.PciId == pciId
                         where dl.PciId == c.PciId
                         where dl.PciId == pl.PciId
                         where (c.TerceroId == terceroId || terceroId == null)
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

        public int ObtenerUltimoConsecutivoArchivoLiquidacion(int pciId)
        {
            var fechaActual = _generalInterface.ObtenerFechaHoraActual();
            var listaArchivo = (from adl in _context.ArchivoDetalleLiquidacion
                                where adl.FechaGeneracion.Date == fechaActual.Date
                                where adl.PciId == pciId
                                select adl)
                               .ToList();

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
            var listaCompromisoConClave = (from pp in _context.ClavePresupuestalContable
                                           where pp.PciId == userParams.PciId
                                           select pp.Crp).ToHashSet();

            var lista = (from dl in _context.DetalleLiquidacion
                         join sp in _context.FormatoSolicitudPago on new { FormatoSolicitudPagoId = dl.FormatoSolicitudPagoId.Value } equals new { sp.FormatoSolicitudPagoId }
                         join dsp in _context.DetalleFormatoSolicitudPago on new { sp.FormatoSolicitudPagoId } equals new { dsp.FormatoSolicitudPagoId }
                         join cpc in _context.ClavePresupuestalContable on new { ClavePresupuestalContableId = dsp.ClavePresupuestalContableId.Value } equals new { cpc.ClavePresupuestalContableId }
                         join rp in _context.RubroPresupuestal on new { dsp.RubroPresupuestalId } equals new { rp.RubroPresupuestalId }
                         join ff in _context.FuenteFinanciacion on new { cpc.FuenteFinanciacionId } equals new { ff.FuenteFinanciacionId }
                         join c in _context.PlanPago on new { dl.PlanPagoId, dl.PciId } equals new { c.PlanPagoId, c.PciId }
                         join t in _context.Tercero on c.TerceroId equals t.TerceroId
                         where dl.EstadoId != (int)EstadoDetalleLiquidacion.Rechazado
                         where dl.PciId == userParams.PciId
                         where (dl.TerceroId == terceroId || terceroId == null)
                         where (dl.Procesado == false)
                         select new FormatoCausacionyLiquidacionPagos()
                         {
                             DetalleLiquidacionId = dl.DetalleLiquidacionId,
                             PlanPagoId = dl.PlanPagoId,
                             Crp = dl.Crp.ToString(),
                             FormatoSolicitudPagoId = dl.FormatoSolicitudPagoId.Value,
                             IdentificacionTercero = dl.NumeroIdentificacion,
                             NombreTercero = dl.Nombre,
                             NumeroRadicadoSupervisor = dl.NumeroRadicado,
                             FechaRadicadoSupervisor = dl.FechaRadicado,
                             ValorTotal = dsp.ValorAPagar,
                             TieneClavePresupuestalContable = listaCompromisoConClave.Contains(c.Crp),
                             IdentificacionRubroPresupuestal = rp.Identificacion,
                             FuenteFinanciacion = ff.Nombre
                         });

            if (lista != null)
            {
                lista.OrderBy(c => c.FechaRadicadoSupervisor);
            }

            return await PagedList<FormatoCausacionyLiquidacionPagos>.CreateAsync(lista, userParams.PageNumber, userParams.PageSize);
        }


 public async Task<PagedList<FormatoCausacionyLiquidacionPagos>> ObtenerLiquidacionesObligacionExportarExcel(
                    int? terceroId,
                    List<int> listaEstadoId,
                    bool? procesado,
                    UserParams userParams)
        {
            var listaCompromisoConClave = (from pp in _context.ClavePresupuestalContable
                                           where pp.PciId == userParams.PciId
                                           select pp.Crp).ToHashSet();

            var lista = (from dl in _context.DetalleLiquidacion
                         join sp in _context.FormatoSolicitudPago on new { FormatoSolicitudPagoId = dl.FormatoSolicitudPagoId.Value } equals new { sp.FormatoSolicitudPagoId }
                         join dsp in _context.DetalleFormatoSolicitudPago on new { sp.FormatoSolicitudPagoId } equals new { dsp.FormatoSolicitudPagoId }                         
                         join rp in _context.RubroPresupuestal on new { dsp.RubroPresupuestalId } equals new { rp.RubroPresupuestalId }
                         join c in _context.PlanPago on new { dl.PlanPagoId, dl.PciId } equals new { c.PlanPagoId, c.PciId }
                         join t in _context.Tercero on c.TerceroId equals t.TerceroId
                         
                         join cpc in _context.ClavePresupuestalContable on  new { ClavePresupuestalContableId = dsp.ClavePresupuestalContableId.Value } equals 
                                                                            new { cpc.ClavePresupuestalContableId } into ClavePresupuestal
                         from cp in ClavePresupuestal.DefaultIfEmpty()
                         join ff in _context.FuenteFinanciacion on new { cp.FuenteFinanciacionId } equals new { ff.FuenteFinanciacionId } into FuenteFinanciacion
                         from ff in FuenteFinanciacion.DefaultIfEmpty()                        
                         
                         where dl.EstadoId != (int)EstadoDetalleLiquidacion.Rechazado
                         where dl.PciId == userParams.PciId
                         where (dl.TerceroId == terceroId || terceroId == null)
                         where (dl.Procesado == false)
                         select new FormatoCausacionyLiquidacionPagos()
                         {
                             DetalleLiquidacionId = dl.DetalleLiquidacionId,
                             PlanPagoId = dl.PlanPagoId,
                             Crp = dl.Crp.ToString(),
                             FormatoSolicitudPagoId = dl.FormatoSolicitudPagoId.Value,
                             IdentificacionTercero = dl.NumeroIdentificacion,
                             NombreTercero = dl.Nombre,
                             NumeroRadicadoSupervisor = dl.NumeroRadicado,
                             FechaRadicadoSupervisor = dl.FechaRadicado,
                             ValorTotal = dsp.ValorAPagar,
                             TieneClavePresupuestalContable = listaCompromisoConClave.Contains(c.Crp),
                             IdentificacionRubroPresupuestal = rp.Identificacion,
                             FuenteFinanciacion = ff.Nombre
                         });

            if (lista != null)
            {
                lista.OrderBy(c => c.FechaRadicadoSupervisor);
            }

            return await PagedList<FormatoCausacionyLiquidacionPagos>.CreateAsync(lista, userParams.PageNumber, userParams.PageSize);
        }


        public async Task<List<int>> ObtenerLiquidacionIdsParaObligacionArchivo(int? terceroId,
                    List<int> listaEstadoId,
                    bool? procesado,
                    UserParams userParams)
        {
            var lista = await (from dl in _context.DetalleLiquidacion
                               join sp in _context.FormatoSolicitudPago on new { FormatoSolicitudPagoId = dl.FormatoSolicitudPagoId.Value } equals new { sp.FormatoSolicitudPagoId }
                               join dsp in _context.DetalleFormatoSolicitudPago on new { sp.FormatoSolicitudPagoId } equals new { dsp.FormatoSolicitudPagoId }
                               join cpc in _context.ClavePresupuestalContable on new { ClavePresupuestalContableId = dsp.ClavePresupuestalContableId.Value } equals new { cpc.ClavePresupuestalContableId }
                               join rp in _context.RubroPresupuestal on new { dsp.RubroPresupuestalId } equals new { rp.RubroPresupuestalId }
                               join ff in _context.FuenteFinanciacion on new { cpc.FuenteFinanciacionId } equals new { ff.FuenteFinanciacionId }
                               join c in _context.PlanPago on new { dl.PlanPagoId, dl.PciId } equals new { c.PlanPagoId, c.PciId }
                               join t in _context.Tercero on c.TerceroId equals t.TerceroId
                               where dl.EstadoId != (int)EstadoDetalleLiquidacion.Rechazado
                               where dl.PciId == userParams.PciId
                               where (dl.TerceroId == terceroId || terceroId == null)
                               where (dl.Procesado == false)
                               select dl.DetalleLiquidacionId
                         )
                         .Distinct()
                         .ToListAsync();

            return lista;
        }


        public async Task<bool> ValidarLiquidacionSinClavePresupuestal(
                           int? terceroId,
                           List<int> listaEstadoId,
                           int pciId)
        {
            bool respuesta = false;
            var listaCompromisoConClave = (from pp in _context.ClavePresupuestalContable
                                           where pp.PciId == pciId
                                           select pp.Crp).ToHashSet();

            var lista = await (from dl in _context.DetalleLiquidacion
                               join sp in _context.FormatoSolicitudPago on new { FormatoSolicitudPagoId = dl.FormatoSolicitudPagoId.Value } equals new { sp.FormatoSolicitudPagoId }
                               join dsp in _context.DetalleFormatoSolicitudPago on new { sp.FormatoSolicitudPagoId } equals new { dsp.FormatoSolicitudPagoId }
                               join cpc in _context.ClavePresupuestalContable on new { ClavePresupuestalContableId = dsp.ClavePresupuestalContableId.Value } equals new { cpc.ClavePresupuestalContableId }
                               join rp in _context.RubroPresupuestal on new { dsp.RubroPresupuestalId } equals new { rp.RubroPresupuestalId }
                               join ff in _context.FuenteFinanciacion on new { cpc.FuenteFinanciacionId } equals new { ff.FuenteFinanciacionId }
                               join c in _context.PlanPago on new { dl.PlanPagoId, dl.PciId } equals new { c.PlanPagoId, c.PciId }
                               join t in _context.Tercero on c.TerceroId equals t.TerceroId
                               where dl.EstadoId != (int)EstadoDetalleLiquidacion.Rechazado
                               where dl.PciId == pciId
                               where (dl.TerceroId == terceroId || terceroId == null)
                               where !listaCompromisoConClave.Contains(dl.Crp)
                               where (dl.Procesado == false)
                               select dl.DetalleLiquidacionId
                         )
                         .Distinct()
                         .ToListAsync();

            if (lista != null && lista.Count > 0)
            {
                respuesta = true;
            }

            return respuesta;
        }

        public async Task<List<int>> ObtenerLiquidacionesConClaveParaArchivoObligacion(int pciId, List<int> listaLiquidacionId)
        {
            var listaCompromisoConClave = (from pp in _context.ClavePresupuestalContable
                                           where pp.PciId == pciId
                                           select pp.Crp).ToHashSet();

            var lista = await (from dl in _context.DetalleLiquidacion
                               join c in _context.PlanPago on dl.PlanPagoId equals c.PlanPagoId
                               where dl.PciId == c.PciId
                               where listaLiquidacionId.Contains(dl.DetalleLiquidacionId)
                               where listaCompromisoConClave.Contains(c.Crp)
                               select dl.DetalleLiquidacionId

                         )
                         .Distinct()
                         .ToListAsync();

            return lista;
        }


        public async Task<List<int>> ObtenerLiquidacionIdsParaArchivoObligacion(int pciId, int? terceroId, List<int> listaEstadoId, bool? procesado)
        {
            var lista = await (from dl in _context.DetalleLiquidacion
                               join c in _context.PlanPago on dl.PlanPagoId equals c.PlanPagoId
                               join t in _context.Tercero on c.TerceroId equals t.TerceroId
                               join p in _context.ParametroLiquidacionTercero on c.TerceroId equals p.TerceroId into parametroLiquidacion
                               from pl in parametroLiquidacion.DefaultIfEmpty()
                               where dl.EstadoId != (int)EstadoDetalleLiquidacion.Rechazado
                               where dl.PciId == c.PciId
                               where dl.PciId == pl.PciId
                               where dl.PciId == pciId
                               where (dl.TerceroId == terceroId || terceroId == null)
                               where (dl.Procesado == procesado || procesado == null)

                               select dl.DetalleLiquidacionId).ToListAsync();
            return lista;
        }


        public async Task<List<int>> ObtenerLiquidacionIdsConUsosPresupuestales(List<int> listaLiquidacionId)
        {
            var lista = await (from cpc in _context.DetalleFormatoSolicitudPago
                               join sp in _context.FormatoSolicitudPago on cpc.FormatoSolicitudPagoId equals sp.FormatoSolicitudPagoId
                               join dl in _context.DetalleLiquidacion on new { sp.Crp, FormatoSolicitudPagoId = sp.FormatoSolicitudPagoId } equals
                                                                         new { dl.Crp, FormatoSolicitudPagoId = dl.FormatoSolicitudPagoId.Value }
                               join cp in _context.ClavePresupuestalContable on cpc.ClavePresupuestalContableId equals cp.ClavePresupuestalContableId
                               join up in _context.UsoPresupuestal on cp.UsoPresupuestalId equals up.UsoPresupuestalId
                               where sp.PciId == dl.PciId
                               where sp.PciId == cp.PciId
                               where sp.PciId == up.PciId
                               where listaLiquidacionId.Contains(dl.DetalleLiquidacionId)
                               where sp.PlanPagoId == dl.PlanPagoId
                               where sp.Crp == cp.Crp
                               where sp.EstadoId == (int)EstadoSolicitudPago.ConLiquidacionDeducciones
                               select dl.DetalleLiquidacionId)

                               .Distinct()
                               .ToListAsync();

            return lista;
        }

        public async Task<List<int>> ObtenerLiquidacionIdsConRubroFuncionamiento(List<int> listaLiquidacionId)
        {
            string codigoFuncionamiento = "A-";

            var lista = await (from cpc in _context.DetalleFormatoSolicitudPago
                               join sp in _context.FormatoSolicitudPago on cpc.FormatoSolicitudPagoId equals sp.FormatoSolicitudPagoId
                               join dl in _context.DetalleLiquidacion on new { sp.Crp, FormatoSolicitudPagoId = sp.FormatoSolicitudPagoId } equals
                                                                            new { dl.Crp, FormatoSolicitudPagoId = dl.FormatoSolicitudPagoId.Value }
                               join cp in _context.ClavePresupuestalContable on cpc.ClavePresupuestalContableId equals cp.ClavePresupuestalContableId
                               join up in _context.UsoPresupuestal on cp.UsoPresupuestalId equals up.UsoPresupuestalId
                               join rp in _context.RubroPresupuestal on cpc.RubroPresupuestalId equals rp.RubroPresupuestalId
                               where sp.PciId == dl.PciId
                               where sp.PciId == cp.PciId
                               where sp.PciId == up.PciId
                               where listaLiquidacionId.Contains(dl.DetalleLiquidacionId)
                               where sp.PlanPagoId == dl.PlanPagoId
                               where sp.Crp == cp.Crp
                               where sp.EstadoId == (int)EstadoSolicitudPago.ConLiquidacionDeducciones
                               where rp.Identificacion.StartsWith(codigoFuncionamiento)
                               //where EF.Functions.Like()
                               select dl.DetalleLiquidacionId)

                               .Distinct()
                               .ToListAsync();

            return lista;
        }

        public async Task<ICollection<DetalleLiquidacionParaArchivo>> ObtenerCabeceraParaArchivoObligacion(List<int> listaLiquidacionId, int pciId)
        {
            List<DetalleLiquidacionParaArchivo> listaFinal = new List<DetalleLiquidacionParaArchivo>();
            string identificacionPCI = string.Empty;
            var pci = await _listaRepository.ObtenerPci(pciId);

            if (pci != null)
            {
                identificacionPCI = pci.Identificacion;
            }

            var lista = await (from dl in _context.DetalleLiquidacion
                               join t in _context.Tercero on dl.TerceroId equals t.TerceroId
                               join p in _context.ParametroLiquidacionTercero on dl.TerceroId equals p.TerceroId into parametroLiquidacion
                               from pl in parametroLiquidacion.DefaultIfEmpty()
                               join tc in _context.TipoCuentaXPagar on pl.TipoCuentaXPagarId equals tc.TipoCuentaXPagarId into CuentaPorPagar
                               from tiCu in CuentaPorPagar.DefaultIfEmpty()
                               join td in _context.TipoDocumentoSoporte on pl.TipoDocumentoSoporteId equals td.TipoDocumentoSoporteId into TipoDocumento
                               from tiDo in TipoDocumento.DefaultIfEmpty()
                               where dl.PciId == pl.PciId
                               where (listaLiquidacionId.Contains(dl.DetalleLiquidacionId))
                               select new DetalleLiquidacionParaArchivo()
                               {
                                   FechaActual = System.DateTime.Now.ToString("yyyy/MM/dd"),
                                   PCI = identificacionPCI,
                                   Crp = dl.Crp,
                                   Dip = "NO",
                                   TipoCuentaXPagarCodigo = pl.TipoCuentaXPagarId > 0 ? tiCu.Codigo : string.Empty,
                                   ValorIva = decimal.Round(dl.ValorIva, 2, MidpointRounding.AwayFromZero),
                                   TipoDocumentoSoporteCodigo = pl.TipoDocumentoSoporteId > 0 ? tiDo.Codigo : string.Empty,
                                   NumeroFactura = dl.NumeroFactura,
                                   ConstanteExpedidor = "11",
                                   ConstanteCargo = "SUPERVISOR",
                                   NombreSupervisor = dl.NombreSupervisor,
                                   TextoComprobanteContable =
                                   dl.TextoComprobanteContable.Length > 220 ? dl.TextoComprobanteContable.Substring(0, 220) : dl.TextoComprobanteContable,
                                   ValorTotal = decimal.Round(dl.ValorTotal, 2, MidpointRounding.AwayFromZero),
                                   FechaRegistro = dl.FechaRegistro.Value,
                               })
                    .ToListAsync();

            if (lista != null && lista.Count > 0)
            {
                listaFinal = lista.OrderBy(x => x.FechaRegistro).ToList();
            }
            return listaFinal;
        }

        public async Task<ICollection<DetalleLiquidacion>> ObtenerListaDetalleLiquidacionSinClavePresupuestalXIds(List<int> listaLiquidacionId)
        {
            var lista = await (from dl in _context.DetalleLiquidacion
                               join sp in _context.FormatoSolicitudPago on new { Crp = dl.Crp, FormatoSolicitudPagoId = dl.FormatoSolicitudPagoId.Value, dl.PciId } equals
                                                                           new { Crp = sp.Crp, FormatoSolicitudPagoId = sp.FormatoSolicitudPagoId, sp.PciId }
                               join dsp in _context.DetalleFormatoSolicitudPago on new { sp.FormatoSolicitudPagoId } equals new { dsp.FormatoSolicitudPagoId }
                               where (listaLiquidacionId.Contains(dl.DetalleLiquidacionId))
                               where dsp.ClavePresupuestalContableId == null
                               select new DetalleLiquidacion()
                               {
                                   DetalleLiquidacionId = dl.DetalleLiquidacionId,
                                   Crp = dl.Crp,
                                   PciId = dl.PciId,
                                   FormatoSolicitudPagoId = dl.FormatoSolicitudPagoId,
                               })
                    .ToListAsync();

            return lista;
        }

        public async Task<ICollection<ClavePresupuestalContableParaArchivo>> ObtenerItemsLiquidacionParaArchivoObligacion(List<int> listaLiquidacionId)
        {
            var lista = await (from cpc in _context.DetalleFormatoSolicitudPago
                               join rp in _context.RubroPresupuestal on cpc.RubroPresupuestalId equals rp.RubroPresupuestalId
                               join sp in _context.FormatoSolicitudPago on cpc.FormatoSolicitudPagoId equals sp.FormatoSolicitudPagoId
                               join dl in _context.DetalleLiquidacion on new { Crp = sp.Crp, FormatoSolicitudPagoId = sp.FormatoSolicitudPagoId } equals
                                                                         new { Crp = dl.Crp, FormatoSolicitudPagoId = dl.FormatoSolicitudPagoId.Value }
                               join cp in _context.ClavePresupuestalContable on cpc.ClavePresupuestalContableId equals cp.ClavePresupuestalContableId
                               join rc in _context.RelacionContable on cp.RelacionContableId equals rc.RelacionContableId
                               join cc in _context.CuentaContable on rc.CuentaContableId equals cc.CuentaContableId into CuentaContable
                               from cuco in CuentaContable.DefaultIfEmpty()
                               join sf in _context.SituacionFondo on cp.SituacionFondoId equals sf.SituacionFondoId
                               join ff in _context.FuenteFinanciacion on cp.FuenteFinanciacionId equals ff.FuenteFinanciacionId
                               join rpr in _context.RecursoPresupuestal on cp.RecursoPresupuestalId equals rpr.RecursoPresupuestalId
                               join ac in _context.AtributoContable on rc.AtributoContableId equals ac.AtributoContableId
                               join tg in _context.TipoGasto on rc.TipoGastoId equals tg.TipoGastoId into tipoGasto
                               from tga in tipoGasto.DefaultIfEmpty()
                               where sp.PciId == dl.PciId
                               where sp.PciId == cp.PciId
                               where sp.PciId == rc.PciId
                               where (listaLiquidacionId.Contains(dl.DetalleLiquidacionId))
                               where (sp.PlanPagoId == dl.PlanPagoId)
                               where (sp.Crp == cp.Crp)
                               select new ClavePresupuestalContableParaArchivo()
                               {
                                   DetalleLiquidacionId = dl.DetalleLiquidacionId,
                                   RubroPresupuestalId = rp.RubroPresupuestalId,
                                   UsoPresupuestalId = cp.UsoPresupuestalId.HasValue ? cp.UsoPresupuestalId.Value : null,
                                   Dependencia = cp.Dependencia,
                                   RubroPresupuestalIdentificacion = rp.Identificacion,
                                   RecursoPresupuestalCodigo = rpr.Codigo,
                                   FuenteFinanciacionCodigo = ff.Codigo,
                                   SituacionFondoCodigo = sf.Codigo,
                                   ValorTotal = cpc.ValorAPagar,
                                   AtributoContableCodigo = ac.Codigo,
                                   TipoGastoCodigo = tga.Codigo,
                                   UsoContable = rc.UsoContable.ToString(),
                                   TipoOperacion = rc.TipoOperacion.ToString(),
                                   NumeroCuenta = (!string.IsNullOrWhiteSpace(ac.Codigo) &&
                                                    !string.IsNullOrWhiteSpace(tga.Codigo) &&
                                                    !string.IsNullOrWhiteSpace(rc.TipoOperacion.ToString()))
                                                    ? string.Empty : (!string.IsNullOrWhiteSpace(rc.UsoContable.ToString())) ? cuco.NumeroCuenta : string.Empty,
                                   ClavePresupuestalContableId = cpc.ClavePresupuestalContableId.Value,
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
                               where (listaLiquidacionId.Contains(ld.DetalleLiquidacionId))
                               where l.TerceroId == td.TerceroId
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
                    .Distinct()
                    .ToListAsync();

            return lista;
        }

        public async Task<ICollection<DetalleLiquidacionParaArchivo>> ObtenerUsosParaArchivoObligacion(List<int> listaLiquidacionId)
        {
            List<DetalleLiquidacionParaArchivo> listaFinal = new List<DetalleLiquidacionParaArchivo>();

            var lista = await (from cpc in _context.DetalleFormatoSolicitudPago
                               join rp in _context.RubroPresupuestal on cpc.RubroPresupuestalId equals rp.RubroPresupuestalId
                               join sp in _context.FormatoSolicitudPago on cpc.FormatoSolicitudPagoId equals sp.FormatoSolicitudPagoId
                               join dl in _context.DetalleLiquidacion on new { sp.Crp, FormatoSolicitudPagoId = sp.FormatoSolicitudPagoId } equals
                                                                            new { dl.Crp, FormatoSolicitudPagoId = dl.FormatoSolicitudPagoId.Value }
                               join cp in _context.ClavePresupuestalContable on cpc.ClavePresupuestalContableId equals cp.ClavePresupuestalContableId
                               join up in _context.UsoPresupuestal on cp.UsoPresupuestalId equals up.UsoPresupuestalId
                               where (sp.PciId == dl.PciId)
                               where (sp.PciId == cp.PciId)
                               where (sp.PciId == up.PciId)
                               where (listaLiquidacionId.Contains(dl.DetalleLiquidacionId))
                               where (sp.PlanPagoId == dl.PlanPagoId)
                               where (sp.Crp == cp.Crp)
                               select new DetalleLiquidacionParaArchivo()
                               {
                                   DetalleLiquidacionId = dl.DetalleLiquidacionId,
                                   UsoPresupuestalId = cp.UsoPresupuestalId,
                                   RubroPresupuestalId = rp.RubroPresupuestalId,
                                   UsoPresupuestalCodigo = up.UsoPresupuestalId > 0 ? up.Identificacion : string.Empty,
                                   ValorTotal = cpc.ValorAPagar,
                                   FechaRegistro = dl.FechaRegistro.Value,
                                   ClavePresupuestalContableId = cpc.ClavePresupuestalContableId.Value,
                               })
                               .Distinct()
                               .ToListAsync();


            if (lista != null && lista.Count > 0)
            {
                listaFinal = lista.OrderBy(x => x.FechaRegistro).ToList();
            }
            return listaFinal;
        }

        public async Task<ICollection<DetalleLiquidacionParaArchivo>> ObtenerFacturaParaArchivoObligacion(List<int> listaLiquidacionId)
        {
            List<DetalleLiquidacionParaArchivo> listaFinal = new List<DetalleLiquidacionParaArchivo>();

            var lista = await (from dl in _context.DetalleLiquidacion
                               join t in _context.Tercero on dl.TerceroId equals t.TerceroId
                               join sp in _context.FormatoSolicitudPago on new { dl.Crp, FormatoSolicitudPagoId = dl.FormatoSolicitudPagoId.Value } equals
                                                                            new { sp.Crp, FormatoSolicitudPagoId = sp.FormatoSolicitudPagoId }
                               join p in _context.ParametroLiquidacionTercero on dl.TerceroId equals p.TerceroId into parametroLiquidacion
                               from pl in parametroLiquidacion.DefaultIfEmpty()
                               where dl.EstadoId != (int)EstadoDetalleLiquidacion.Rechazado
                               where dl.PciId == pl.PciId
                               where sp.PciId == dl.PciId
                               where sp.PlanPagoId == dl.PlanPagoId
                               where (listaLiquidacionId.Contains(dl.DetalleLiquidacionId))
                               select new DetalleLiquidacionParaArchivo()
                               {
                                   DetalleLiquidacionId = dl.DetalleLiquidacionId,
                                   NumeroFactura = sp.NumeroFactura,
                                   EsFacturaElectronica = pl.FacturaElectronica ? true : false,
                               })
                    .ToListAsync();

            if (lista != null && lista.Count > 0)
            {
                listaFinal = lista.OrderBy(x => x.FechaRegistro).ToList();
            }
            return listaFinal;
        }

        #endregion Archivo Obligacion Presupuestal

        #region Archivo General

        public async Task<ICollection<ValorSeleccion>> ObtenerListaArchivoCreados(DateTime fechaGeneracion, int tipoDocumentoArchivo, int pcidId)
        {
            List<ValorSeleccion> listaFinal = new List<ValorSeleccion>();

            var lista = await (from adl in _context.ArchivoDetalleLiquidacion
                               where adl.FechaGeneracion.Date == fechaGeneracion.Date
                               where adl.TipoDocumentoArchivo == tipoDocumentoArchivo
                               where adl.PciId == pcidId
                               select new ValorSeleccion()
                               {
                                   Id = adl.ArchivoDetalleLiquidacionId,
                                   Nombre = adl.Nombre,
                               })
                    .ToListAsync();

            return lista;
        }

        public async Task<PagedList<FormatoCausacionyLiquidacionPagos>> ObtenerDocumentosParaAdministracionArchivo(
             int archivoId,
             UserParams userParams)
        {
            var listaCompromisoConClave = (from pp in _context.ClavePresupuestalContable
                                           where pp.PciId == userParams.PciId
                                           select pp.Crp).ToHashSet();

            var lista = (from dal in _context.DetalleArchivoLiquidacion
                         join dl in _context.DetalleLiquidacion on new { dal.DetalleLiquidacionId } equals new { dl.DetalleLiquidacionId }
                         join c in _context.PlanPago on new { dl.PlanPagoId, dl.PciId } equals new { c.PlanPagoId, c.PciId }
                         join t in _context.Tercero on c.TerceroId equals t.TerceroId
                         join p in _context.ParametroLiquidacionTercero on new { c.TerceroId, dl.PciId } equals new { p.TerceroId, p.PciId }
                         where dal.ArchivoDetalleLiquidacionId == archivoId
                         where dl.PciId == userParams.PciId
                         select new FormatoCausacionyLiquidacionPagos()
                         {
                             DetalleLiquidacionId = dl.DetalleLiquidacionId,
                             PlanPagoId = dl.PlanPagoId,
                             Crp = dl.Crp.ToString(),
                             FormatoSolicitudPagoId = dl.FormatoSolicitudPagoId.Value,
                             IdentificacionTercero = dl.NumeroIdentificacion,
                             NombreTercero = dl.Nombre,
                             NumeroRadicadoSupervisor = dl.NumeroRadicado,
                             FechaRadicadoSupervisor = dl.FechaRadicado,
                             ValorTotal = c.ValorFacturado.Value,
                             TieneClavePresupuestalContable = listaCompromisoConClave.Contains(c.Crp)
                         });

            if (lista != null)
            {
                lista.OrderBy(c => c.FechaRadicadoSupervisor);
            }

            return await PagedList<FormatoCausacionyLiquidacionPagos>.CreateAsync(lista, userParams.PageNumber, userParams.PageSize);
        }

        public async Task<bool> EliminarListaDetalleArchivo(List<int> liquidacionIds)
        {
            var listaExistente = await (from plt in _context.DetalleArchivoLiquidacion
                                        where liquidacionIds.Contains(plt.DetalleLiquidacionId)
                                        select plt).ToListAsync();

            _context.DetalleArchivoLiquidacion.RemoveRange(listaExistente);
            return true;
        }

        public async Task<bool> EliminarArchivoDetalleLiquidacion(int archivoId)
        {
            var archivo = await (from plt in _context.ArchivoDetalleLiquidacion
                                 where plt.ArchivoDetalleLiquidacionId == archivoId
                                 select plt).FirstOrDefaultAsync();

            _context.ArchivoDetalleLiquidacion.Remove(archivo);
            return true;
        }

        #endregion Archivo General

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

        public async Task<ICollection<DetalleLiquidacion>> ObtenerListaDetalleLiquidacionViaticosMesAnteriorXTerceroIds(List<int> listaTerceroId, int pcidId)
        {
            int mesAnterior = _generalInterface.ObtenerFechaHoraActual().AddMonths(-1).Month;

            var detalleLiquidacionAnterior = await (from dl in _context.DetalleLiquidacion
                                                    where dl.EstadoId != (int)EstadoDetalleLiquidacion.Rechazado
                                                    where dl.PciId == pcidId
                                                    where listaTerceroId.Contains(dl.TerceroId)
                                                    where dl.FechaOrdenPago.Value.Month == mesAnterior
                                                    where dl.Viaticos == "SI"
                                                    select dl)
                                            .ToListAsync();
            return detalleLiquidacionAnterior;
        }

        public ICollection<DetalleLiquidacion> ObtenerListaDetalleLiquidacionMesAnteriorXTerceroIds(List<int> listaTerceroId, int pciId)
        {
            int mesAnterior = _generalInterface.ObtenerFechaHoraActual().AddMonths(-1).Month;
            List<DetalleLiquidacion> lista = new List<DetalleLiquidacion>();

            var query1 = (from dl in _context.DetalleLiquidacion
                          where dl.EstadoId != (int)EstadoDetalleLiquidacion.Rechazado
                          where dl.PciId == pciId
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