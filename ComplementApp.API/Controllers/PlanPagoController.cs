using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using ComplementApp.API.Data;
using ComplementApp.API.Dtos;
using ComplementApp.API.Helpers;
using ComplementApp.API.Interfaces;
using ComplementApp.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;

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

        #endregion 

        #region Dependency Injection
        private readonly DataContext _dataContext;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPlanPagoRepository _repo;
        private readonly IMapper _mapper;
        private readonly IProcesoLiquidacionPlanPago _procesoLiquidacion;
        private readonly IMailService mailService;
        private readonly IGeneralInterface _generalInterface;

        #endregion Dependency Injection

        public PlanPagoController(IUnitOfWork unitOfWork, IPlanPagoRepository repo,
                                    IMapper mapper, DataContext dataContext,
                                    IMailService mailService,
                                    IProcesoLiquidacionPlanPago procesoLiquidacion,
                                    IGeneralInterface generalInterface)
        {
            this._mapper = mapper;
            this._repo = repo;
            this._unitOfWork = unitOfWork;
            this.mailService = mailService;
            _dataContext = dataContext;
            this._procesoLiquidacion = procesoLiquidacion;
            this._generalInterface = generalInterface;
        }

        [Route("[action]")]
        [HttpGet]
        public async Task<IActionResult> ObtenerListaPlanPago([FromQuery(Name = "terceroId")] int? terceroId,
                                                              [FromQuery(Name = "listaEstadoId")] string listaEstadoId,
                                                              [FromQuery] UserParams userParams)
        {
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

            if (planPagoBD != null)
            {
                var cantidad = _repo.ObtenerCantidadMaximaPlanPago(planPagoBD.Crp);
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

            var pagedList = await _repo.ObtenerListaRadicadoPaginado(mes, terceroId, listIds, userParams);
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
                if (mes > 0 && mes < 13)
                {
                    mesDescripcion = UppercaseFirst(DateTimeFormatInfo.CurrentInfo.GetMonthName(mes));
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

                var lista = await _repo.ObtenerListaRadicado(mes, terceroIdTemp, listIds);
                if (lista != null)
                {
                    DataTable dtResultado = ObtenerTablaDeListaRadicado(lista.ToList());
                    return this.ExportExcel(dtResultado, nombreArchivo);
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
                var lista = await _repo.ObtenerLineasPlanPagoXCompromiso(crp);
                return Ok(lista);
            }
            catch (Exception)
            {
                throw;
            }

            throw new Exception($"No se pudo obtener la lista de planes de pago para el compromiso");
        }


        #endregion Forma Pago Compromiso

        public FileStreamResult ExportExcel(DataTable dt, string nombreArchivo)
        {
            var memoryStream = new MemoryStream();

            using (var package = new ExcelPackage(memoryStream))
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Radicados");
                int currentRowNo = 1;
                int totalRows = dt.Rows.Count;
                int k = 0;
                foreach (DataColumn column in dt.Columns)
                {
                    worksheet.Cells[currentRowNo, k + 1].Value = column.ColumnName;
                    k++;
                }
                currentRowNo++;
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    for (int j = 0; j < dt.Columns.Count; j++)
                    {
                        worksheet.Cells[currentRowNo, j + 1].Value = Convert.ToString(dt.Rows[i][j]);
                    }
                    currentRowNo++;
                }

                int columnCount = dt.Columns.Count;
                for (int i = 1; i <= columnCount; i++)
                {
                    worksheet.Column(i).AutoFit();
                }
                worksheet.Row(1).Height = 55;
                worksheet.Row(1).Style.Font.Bold = true;

                package.Save();
            }

            memoryStream.Position = 0;
            var contentType = "application/octet-stream";
            var fileName = nombreArchivo;
            Response.AddFileName(nombreArchivo);
            return File(memoryStream, contentType, fileName);
        }

        private DataTable ObtenerTablaDeListaRadicado(List<RadicadoDto> lista)
        {
            int consecutivo = 1;
            DataTable dt = new DataTable();
            DataRow dr = null;
            dt.Columns.Add(new DataColumn("ID", typeof(int)));
            dt.Columns.Add(new DataColumn("FECHA_RAD", typeof(string)));
            dt.Columns.Add(new DataColumn("ESTADO", typeof(string)));
            dt.Columns.Add(new DataColumn("CRP", typeof(string)));
            dt.Columns.Add(new DataColumn("NIT", typeof(string)));
            dt.Columns.Add(new DataColumn("NOMBRE_TERCERO", typeof(string)));
            dt.Columns.Add(new DataColumn("NUM_RAD_PROV", typeof(string)));
            dt.Columns.Add(new DataColumn("NUM_RAD_SUP", typeof(string)));
            dt.Columns.Add(new DataColumn("TOTAL_A_PAGAR", typeof(decimal)));
            dt.Columns.Add(new DataColumn("NUM_OBLI", typeof(string)));
            dt.Columns.Add(new DataColumn("NUM_OP", typeof(string)));
            dt.Columns.Add(new DataColumn("FECHA_PAGO", typeof(string)));
            dt.Columns.Add(new DataColumn("DETALLE", typeof(string)));

            foreach (var item in lista)
            {
                dr = dt.NewRow();
                dr["ID"] = consecutivo;
                dr["FECHA_RAD"] = item.FechaRadicadoSupervisorDescripcion;
                dr["ESTADO"] = item.Estado;
                dr["CRP"] = item.Crp;
                dr["NIT"] = item.NIT;
                dr["NOMBRE_TERCERO"] = item.NombreTercero;
                dr["NUM_RAD_PROV"] = item.NumeroRadicadoProveedor;
                dr["NUM_RAD_SUP"] = item.NumeroRadicadoSupervisor;
                dr["TOTAL_A_PAGAR"] = item.ValorAPagar;
                dr["NUM_OBLI"] = item.Obligacion;
                dr["NUM_OP"] = item.OrdenPago;
                dr["FECHA_PAGO"] = item.FechaOrdenPagoDescripcion;
                dr["DETALLE"] = item.TextoComprobanteContable;
                dt.Rows.Add(dr);
                consecutivo++;
            }
            return dt;
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

        private async Task RegistrarListaPlanPago(CDPDto cdp, List<LineaPlanPagoDto> lista)
        {
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
                var listaActualXCompromiso = await _repo.ObtenerLineasPlanPagoXCompromiso((int)cdp.Crp);

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
                        planPago.ValorPagado = 0;
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
            planPago.ValorPagado = 0;
            planPago.Viaticos = viaticos;
            planPago.TerceroId = cdp.TerceroId;
            planPago.NumeroPago = numeroPago;

            planPago.UsuarioIdRegistro = usuarioId;
            planPago.FechaRegistro = _generalInterface.ObtenerFechaHoraActual();

            return planPago;
        }
    }
}