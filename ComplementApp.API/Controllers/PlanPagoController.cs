using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using ComplementApp.API.Data;
using ComplementApp.API.Dtos;
using ComplementApp.API.Helpers;
using ComplementApp.API.Interfaces;
using ComplementApp.API.Interfaces.Repository;
using ComplementApp.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace ComplementApp.API.Controllers
{
    [ServiceFilter(typeof(LogActividadUsuario))]
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class PlanPagoController : ControllerBase
    {
        #region Variable
        int usuarioId = 0;
        int pciId = 0;
        string valorPciId = string.Empty;

        #endregion 

        #region Dependency Injection
        private readonly DataContext _dataContext;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPlanPagoRepository _repo;
        private readonly IMapper _mapper;
        private readonly IProcesoLiquidacionPlanPago _procesoLiquidacion;
        private readonly IMailService mailService;
        private readonly IGeneralInterface _generalInterface;
        private readonly IProcesoCreacionArchivoExcel _procesoCreacionExcelInterface;

        #endregion Dependency Injection

        public PlanPagoController(IUnitOfWork unitOfWork, IPlanPagoRepository repo,
                                    IMapper mapper, DataContext dataContext,
                                    IMailService mailService,
                                    IProcesoLiquidacionPlanPago procesoLiquidacion,
                                    IGeneralInterface generalInterface,
                                    IProcesoCreacionArchivoExcel procesoCreacionExcelInterface)
        {
            this._mapper = mapper;
            this._repo = repo;
            this._unitOfWork = unitOfWork;
            this.mailService = mailService;
            _dataContext = dataContext;
            this._procesoLiquidacion = procesoLiquidacion;
            this._generalInterface = generalInterface;
            this._procesoCreacionExcelInterface = procesoCreacionExcelInterface;
        }

        [Route("[action]")]
        [HttpGet]
        public async Task<IActionResult> ObtenerListaPlanPago([FromQuery(Name = "terceroId")] int? terceroId,
                                                              [FromQuery(Name = "listaEstadoId")] string listaEstadoId,
                                                              [FromQuery] UserParams userParams)
        {
            valorPciId = User.FindFirst(ClaimTypes.Role).Value;
            if (!string.IsNullOrEmpty(valorPciId))
            {
                pciId = int.Parse(valorPciId);
            }
            userParams.PciId = pciId;

            List<int> listIds = listaEstadoId.Split(',').Select(int.Parse).ToList();

            var pagedList = await _repo.ObtenerListaPlanPago(terceroId, listIds, userParams);
            var listaDto = _mapper.Map<IEnumerable<PlanPagoDto>>(pagedList);

            Response.AddPagination(pagedList.CurrentPage, pagedList.PageSize,
                                pagedList.TotalCount, pagedList.TotalPages);

            return base.Ok(listaDto);
        }

        [Route("[action]")]
        [HttpGet]
        public async Task<IActionResult> ObtenerListaPlanPagoXCompromiso([FromQuery(Name = "crp")] long crp,
                                                                [FromQuery(Name = "listaEstadoId")] string listaEstadoId,
                                                                [FromQuery] UserParams userParams)
        {
            valorPciId = User.FindFirst(ClaimTypes.Role).Value;
            if (!string.IsNullOrEmpty(valorPciId))
            {
                pciId = int.Parse(valorPciId);
            }
            userParams.PciId = pciId;
            List<int> listIds = listaEstadoId.Split(',').Select(int.Parse).ToList();

            var pagedList = await _repo.ObtenerListaPlanPagoXCompromiso(crp, listIds, userParams);
            var listaDto = _mapper.Map<IEnumerable<PlanPagoDto>>(pagedList);

            Response.AddPagination(pagedList.CurrentPage, pagedList.PageSize,
                                pagedList.TotalCount, pagedList.TotalPages);

            return base.Ok(listaDto);
        }


        [Route("[action]")]
        [HttpGet]
        public async Task<IActionResult> ObtenerPlanPago([FromQuery(Name = "planPagoId")] int planPagoId)
        {
            var planPagoBD = await _repo.ObtenerPlanPagoBase(planPagoId);
            var planPagoDto = _mapper.Map<PlanPagoDto>(planPagoBD);
            return base.Ok(planPagoDto);
        }

        [Route("[action]")]
        [HttpGet]
        public async Task<IActionResult> ObtenerPlanPagoDetallado([FromQuery(Name = "planPagoId")] int planPagoId)
        {
            var planPagoBD = await _repo.ObtenerPlanPagoDetallado(planPagoId);
            return base.Ok(planPagoBD);
        }

        [Route("[action]")]
        [HttpGet]
        public async Task<IActionResult> ObtenerDetallePlanPago([FromQuery(Name = "planPagoId")] int planPagoId)
        {
            var planPagoBD = await _repo.ObtenerDetallePlanPago(planPagoId);
            valorPciId = User.FindFirst(ClaimTypes.Role).Value;
            if (!string.IsNullOrEmpty(valorPciId))
            {
                pciId = int.Parse(valorPciId);
            }

            if (planPagoBD != null)
            {
                var cantidad = _repo.ObtenerCantidadMaximaPlanPago(planPagoBD.Crp, pciId);
                planPagoBD.CantidadPago = cantidad;
            }
            return base.Ok(planPagoBD);
        }

        [Route("[action]")]
        [HttpGet]
        public async Task<IActionResult> ObtenerDetallePlanPagoParaSolicitudPago([FromQuery(Name = "planPagoId")] int planPagoId)
        {
            var planPagoBD = await _repo.ObtenerDetallePlanPagoParaSolicitudPago(planPagoId);

            if (planPagoBD != null)
            {
                valorPciId = User.FindFirst(ClaimTypes.Role).Value;
                if (!string.IsNullOrEmpty(valorPciId))
                {
                    pciId = int.Parse(valorPciId);
                }
                var cantidad = _repo.ObtenerCantidadMaximaPlanPago(planPagoBD.Crp, pciId);
                planPagoBD.CantidadPago = cantidad;
            }
            return base.Ok(planPagoBD);
        }


        [HttpPut]
        public async Task<IActionResult> ActualizarPlanPago(PlanPagoDto planPagoDto)
        {
            usuarioId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            if (planPagoDto != null)
            {
                //Obtener todos los campos del plan de pago para una correcta actualización
                var planPagoBD = await _repo.ObtenerPlanPagoBase(planPagoDto.PlanPagoId);

                _mapper.Map(planPagoDto, planPagoBD);

                if (planPagoDto.esRadicarFactura)
                {
                    planPagoBD.EstadoPlanPagoId = (int)EstadoPlanPago.PorObligar;
                    planPagoBD.FechaFactura = _generalInterface.ObtenerFechaHoraActual();
                    planPagoBD.FechaRegistro = _generalInterface.ObtenerFechaHoraActual();
                    planPagoBD.UsuarioIdRegistro = usuarioId;
                }
                else
                {
                    planPagoBD.EstadoPlanPagoId = (int)EstadoPlanPago.PorObligar;
                    planPagoBD.FechaFactura = _generalInterface.ObtenerFechaHoraActual();
                    planPagoBD.FechaModificacion = _generalInterface.ObtenerFechaHoraActual();
                    planPagoBD.UsuarioIdModificacion = usuarioId;
                }

                //Para apagar manualmente la columna que no deseo modificar
                // _dataContext.Entry(planPagoBD).Property("RubroPresupuestalId").IsModified = false;

                //await _unitOfWork.CompleteAsync();
                //Para solicitar la actualización de una entidad
                //_dataContext.Update(planPagoBD);
                await _unitOfWork.CompleteAsync();
                return NoContent();
            }

            throw new Exception($"No se pudo actualizar la factura");
        }


        [Route("[action]")]
        [HttpGet]
        public async Task<IActionResult> ObtenerListaRadicadoPaginado([FromQuery(Name = "mes")] int mes,
                                                                        [FromQuery(Name = "terceroId")] int? terceroId,
                                                                        [FromQuery(Name = "listaEstadoId")] string listaEstadoId,
                                                                        [FromQuery] UserParams userParams)
        {
            int i = (userParams.PageNumber - 1) * userParams.PageSize + 1;
            List<int> listIds = listaEstadoId.Split(',').Select(int.Parse).ToList();
            valorPciId = User.FindFirst(ClaimTypes.Role).Value;
            if (!string.IsNullOrEmpty(valorPciId))
            {
                pciId = int.Parse(valorPciId);
            }

            var pagedList = await _repo.ObtenerListaRadicadoPaginado(pciId, mes, terceroId, listIds, userParams);
            var listaDto = _mapper.Map<IEnumerable<RadicadoDto>>(pagedList);

            foreach (var item in listaDto)
            {
                item.Consecutivo = i;
                i++;
            }

            Response.AddPagination(pagedList.CurrentPage, pagedList.PageSize,
                                    pagedList.TotalCount, pagedList.TotalPages);

            return base.Ok(listaDto);
        }

        [Route("[action]")]
        [HttpGet]
        public async Task<IActionResult> DescargarListaRadicado([FromQuery(Name = "mes")] int mes,
                                                                [FromQuery(Name = "listaEstadoId")] string listaEstadoId,
                                                                [FromQuery(Name = "terceroId")] int terceroId
                                                                        )
        {
            List<int> listIds = new List<int>();
            int? terceroIdTemp = null;
            string mesDescripcion = string.Empty;
            string nombreArchivo = "Radicados.xlsx";

            try
            {
                valorPciId = User.FindFirst(ClaimTypes.Role).Value;
                if (!string.IsNullOrEmpty(valorPciId))
                {
                    pciId = int.Parse(valorPciId);
                }
                if (mes > 0 && mes < 13)
                {
                    mesDescripcion = _generalInterface.UppercaseFirst(DateTimeFormatInfo.CurrentInfo.GetMonthName(mes));
                    nombreArchivo = "Radicados_" + mesDescripcion + ".xlsx";
                }
                if (terceroId > 0)
                {
                    terceroIdTemp = terceroId;
                }

                if (listaEstadoId.Length > 0)
                {
                    listIds = listaEstadoId.Split(',').Select(int.Parse).ToList();
                }

                var lista = await _repo.ObtenerListaRadicado(pciId, mes, terceroIdTemp, listIds);
                if (lista != null)
                {
                    DataTable dtResultado = _procesoCreacionExcelInterface.ObtenerTablaDeListaRadicado(lista.ToList());
                    return _procesoCreacionExcelInterface.ExportExcel(Response, dtResultado, nombreArchivo);
                }
            }
            catch (Exception)
            {
                throw;
            }
            return BadRequest();
        }

        [Route("[action]")]
        [HttpGet]
        public async Task<IActionResult> DescargarListaPlanPago()
        {
            string nombreArchivo = "ListaPlanPago.xlsx";
            try
            {
                valorPciId = User.FindFirst(ClaimTypes.Role).Value;
                if (!string.IsNullOrEmpty(valorPciId))
                {
                    pciId = int.Parse(valorPciId);
                }
                var lista = await _repo.ObtenerListaPlanPagoTotal(pciId);
                if (lista != null)
                {
                    DataTable dtResultado = _procesoCreacionExcelInterface.ObtenerTablaDeListaPlanPago(lista.ToList());
                    return _procesoCreacionExcelInterface.ExportExcel(Response, dtResultado, nombreArchivo);
                }
            }
            catch (Exception)
            {
                throw;
            }
            return BadRequest();
        }

        #region Forma Pago Compromiso

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> RegistrarFormaPagoCompromiso([FromQuery(Name = "tipo")] int tipo, FormaPagoCompromisoDto forma)
        {
            usuarioId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            valorPciId = User.FindFirst(ClaimTypes.Role).Value;
            if (!string.IsNullOrEmpty(valorPciId))
            {
                pciId = int.Parse(valorPciId);
            }
            await using var transaction = await _dataContext.Database.BeginTransactionAsync();

            if (forma != null)
            {
                if (forma.ListaLineaPlanPago != null && forma.ListaLineaPlanPago.Count > 0)
                {
                    var lista = forma.ListaLineaPlanPago.OrderBy(x => x.MesId).ToList();

                    if (tipo == 1)
                    {
                        await RegistrarListaPlanPago(forma.Cdp, lista);
                    }
                    else
                    {
                        await ActualizarListaPlanPago(forma.Cdp, lista);
                    }

                    await transaction.CommitAsync();
                    return Ok(1);
                }
            }
            return NoContent();
        }


        [Route("[action]")]
        [HttpGet]
        public async Task<ActionResult> ObtenerCompromisosParaPlanPago([FromQuery(Name = "tipo")] int tipo,
                                                                        [FromQuery(Name = "terceroId")] int? terceroId,
                                                                        [FromQuery(Name = "numeroCrp")] int? numeroCrp,
                                                                        [FromQuery] UserParams userParams)
        {

            try
            {
                PagedList<CDPDto> pagedList = null;
                valorPciId = User.FindFirst(ClaimTypes.Role).Value;
                if (!string.IsNullOrEmpty(valorPciId))
                {
                    pciId = int.Parse(valorPciId);
                }
                userParams.PciId = pciId;

                if (tipo == 1)
                {
                    pagedList = await _repo.ObtenerCompromisosSinPlanPago(terceroId, numeroCrp, userParams);
                }
                else
                {
                    pagedList = await _repo.ObtenerCompromisosConPlanPago(terceroId, numeroCrp, userParams);
                }
                var listaDto = _mapper.Map<IEnumerable<CDPDto>>(pagedList);

                Response.AddPagination(pagedList.CurrentPage, pagedList.PageSize,
                                    pagedList.TotalCount, pagedList.TotalPages);

                return Ok(listaDto);
            }
            catch (Exception)
            {
                throw;
            }

            throw new Exception($"No se pudo obtener la lista de compromisos");
        }

        [Route("[action]")]
        [HttpGet]
        public async Task<ActionResult> ObtenerLineasPlanPagoXCompromiso([FromQuery(Name = "crp")] int crp)
        {
            try
            {
                valorPciId = User.FindFirst(ClaimTypes.Role).Value;
                if (!string.IsNullOrEmpty(valorPciId))
                {
                    pciId = int.Parse(valorPciId);
                }
                var lista = await _repo.ObtenerLineasPlanPagoXCompromiso(crp, pciId);
                return Ok(lista);
            }
            catch (Exception)
            {
                throw;
            }

            throw new Exception($"No se pudo obtener la lista de planes de pago para el compromiso");
        }


        #endregion Forma Pago Compromiso
        private async Task RegistrarListaPlanPago(CDPDto cdp, List<LineaPlanPagoDto> lista)
        {
            valorPciId = User.FindFirst(ClaimTypes.Role).Value;
            if (!string.IsNullOrEmpty(valorPciId))
            {
                pciId = int.Parse(valorPciId);
            }
            List<PlanPago> listaPlanPago = new List<PlanPago>();
            PlanPago planPago = null;
            int numeroPagos = 1;
            DateTime fechaActual = _generalInterface.ObtenerFechaHoraActual();
            int mesAnterior = 0;

            if (lista != null && lista.Count > 0)
            {
                foreach (var item in lista)
                {
                    planPago = MapearPlanPago(cdp, fechaActual.Year, item.MesId, item.Valor, numeroPagos, item.Viaticos);
                    planPago.PciId = pciId;
                    listaPlanPago.Add(planPago);
                    mesAnterior = item.MesId;
                    numeroPagos++;
                }

                await _dataContext.PlanPago.AddRangeAsync(listaPlanPago);
                await _dataContext.SaveChangesAsync();
            }
        }

        private async Task ActualizarListaPlanPago(CDPDto cdp, List<LineaPlanPagoDto> listaTotal)
        {
            valorPciId = User.FindFirst(ClaimTypes.Role).Value;
            if (!string.IsNullOrEmpty(valorPciId))
            {
                pciId = int.Parse(valorPciId);
            }
            int numeroPagos = 1;
            List<PlanPago> listaPlanPago = new List<PlanPago>();
            PlanPago planPago = null;
            DateTime fechaActual = _generalInterface.ObtenerFechaHoraActual();

            #region Registrar nuevos 

            List<LineaPlanPagoDto> listaNueva = listaTotal
                                                .Where(x => x.EstadoModificacion == (int)EstadoModificacion.Insertado)
                                                .OrderBy(x => x.MesId)
                                                .ToList();

            if (listaNueva != null && listaNueva.Count > 0)
            {
                var listaActualXCompromiso = await _repo.ObtenerLineasPlanPagoXCompromiso((int)cdp.Crp, pciId);

                foreach (var item in listaNueva)
                {
                    var planesPagoBD = listaActualXCompromiso.ToList();

                    if (planesPagoBD == null)
                    {
                        numeroPagos = 1;
                    }
                    else
                    {
                        if (numeroPagos == 1)
                        {
                            numeroPagos = planesPagoBD.Count() + 1;
                        }
                    }

                    planPago = MapearPlanPago(cdp, fechaActual.Year, item.MesId, item.Valor, numeroPagos, item.Viaticos);

                    listaPlanPago.Add(planPago);
                    numeroPagos++;
                }

                await _dataContext.PlanPago.AddRangeAsync(listaPlanPago);
                await _dataContext.SaveChangesAsync();
            }

            #endregion Registrar nuevos 

            #region Actualizar registros

            List<LineaPlanPagoDto> listaModificada = listaTotal
            .Where(x => x.EstadoModificacion == (int)EstadoModificacion.Modificado)
            .ToList();

            if (listaModificada != null && listaModificada.Count > 0)
            {
                foreach (var item in listaModificada)
                {
                    planPago = await _repo.ObtenerPlanPagoBase(item.PlanPagoId);

                    if (planPago != null)
                    {
                        planPago.ValorInicial = item.Valor;
                        planPago.ValorAdicion = 0;
                        planPago.ValorAPagar = item.Valor;
                        planPago.SaldoDisponible = 0;
                        planPago.UsuarioIdModificacion = usuarioId;
                        planPago.Viaticos = item.Viaticos;
                        planPago.FechaModificacion = _generalInterface.ObtenerFechaHoraActual();
                        await _dataContext.SaveChangesAsync();
                    }
                }
            }

            #endregion Actualizar registros
        }

        private PlanPago MapearPlanPago(CDPDto cdp, int anio, int mesPago, decimal valor, int numeroPago, bool viaticos)
        {
            var planPago = new PlanPago();
            planPago.Crp = cdp.Crp;
            planPago.EstadoPlanPagoId = (int)EstadoPlanPago.PorPagar;
            planPago.Cdp = cdp.Cdp;
            planPago.AnioPago = anio;
            planPago.MesPago = mesPago;
            planPago.ValorInicial = valor;
            planPago.ValorAdicion = 0;
            planPago.ValorAPagar = valor;
            planPago.SaldoDisponible = 0;
            planPago.Viaticos = viaticos;
            planPago.TerceroId = cdp.TerceroId;
            planPago.NumeroPago = numeroPago;
            planPago.PciId = pciId;

            planPago.UsuarioIdRegistro = usuarioId;
            planPago.FechaRegistro = _generalInterface.ObtenerFechaHoraActual();

            return planPago;
        }
    }
}