using System;
using System.Collections.Generic;
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

namespace ComplementApp.API.Controllers
{
    [ServiceFilter(typeof(LogActividadUsuario))]
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ObligacionController : ControllerBase
    {
        #region Variable

        int usuarioId = 0;

        #endregion

        #region Dependency Injection

        private readonly DataContext _dataContext;
        private readonly IObligacionRepository _repo;
        private readonly IMapper _mapper;
        private readonly IGeneralInterface _generalInterface;
        private readonly IListaRepository _listaRepository;
        private readonly IPlanPagoRepository _planPagoRepository;

        private readonly IProcesoLiquidacionSolicitudPago _procesoLiquidacion;

        #endregion Dependency Injection


        public ObligacionController(IObligacionRepository repo, IMapper mapper,
                            DataContext dataContext, IGeneralInterface generalInterface,
                            IPlanPagoRepository planPagoRepository, IListaRepository listaRepository,
                            IProcesoLiquidacionSolicitudPago procesoLiquidacion)
        {
            _mapper = mapper;
            _repo = repo;
            _dataContext = dataContext;
            _generalInterface = generalInterface;
            _listaRepository = listaRepository;
            _planPagoRepository = planPagoRepository;
            _procesoLiquidacion = procesoLiquidacion;
        }

        #region Registro de Solicitud de Pago

        [Route("[action]")]
        [HttpGet]
        public async Task<ActionResult> ObtenerCompromisosParaSolicitudRegistroPago([FromQuery(Name = "usuarioId")] int usuarioId,
                                                                                    [FromQuery(Name = "perfilId")] int perfilId,
                                                                                    [FromQuery(Name = "terceroId")] int? terceroId,
                                                                                    [FromQuery] UserParams userParams)
        {
            try
            {
                var pagedList = await _repo.ObtenerCompromisosParaSolicitudRegistroPago(usuarioId, perfilId, terceroId, userParams);
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



        #endregion Registro de Solicitud de Pago

        [Route("[action]")]
        [HttpGet]
        public async Task<ActionResult> ObtenerCompromisosParaClavePresupuestalContable([FromQuery(Name = "terceroId")] int? terceroId,
                                                                                        [FromQuery(Name = "numeroCrp")] int? numeroCrp,
                                                                                        [FromQuery] UserParams userParams)
        {
            try
            {
                var pagedList = await _repo.ObtenerCompromisosParaClavePresupuestalContable(terceroId, numeroCrp, userParams);
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
        public async Task<ActionResult> ObtenerRubrosParaClavePresupuestalContable([FromQuery(Name = "cdpId")] int cdpId)
        {
            try
            {
                string identificacionPCI = string.Empty;

                ValorSeleccion parametroPCI = await _listaRepository.ObtenerParametroGeneralXNombre("Pci-ANE");
                if (identificacionPCI != null)
                {
                    identificacionPCI = parametroPCI.Valor;
                }

                var lista = await _repo.ObtenerRubrosParaClavePresupuestalContable(cdpId);

                foreach (var item in lista)
                {
                    item.Pci = identificacionPCI;
                }

                return Ok(lista);
            }
            catch (Exception)
            {
                throw;
            }

            throw new Exception($"No se pudo obtener los rubros presupuestales");
        }

        [Route("[action]")]
        [HttpGet]
        public async Task<ActionResult> ObtenerRelacionesContableXRubro([FromQuery(Name = "rubroPresupuestalId")] int rubroPresupuestalId)
        {
            try
            {
                var lista = await _repo.ObtenerRelacionesContableXRubro(rubroPresupuestalId);
                return Ok(lista);
            }
            catch (Exception)
            {
                throw;
            }

            throw new Exception($"No se pudo obtener la lista de relaciones contables");
        }


        [HttpPost]
        public async Task<IActionResult> RegistrarRelacionContable(RelacionContableDto relacionDto)
        {
            await using var transaction = await _dataContext.Database.BeginTransactionAsync();
            try
            {
                var relacion = _mapper.Map<RelacionContable>(relacionDto);

                await _repo.RegistrarRelacionContable(relacion);
                await _dataContext.SaveChangesAsync();

                await transaction.CommitAsync();

                return Ok();
            }
            catch (Exception)
            {
                throw;
            }
        }

        [Route("[action]")]
        [HttpGet]
        public async Task<IActionResult> ObtenerFormatoSolicitudPago(int cdpId)
        {
            FormatoSolicitudPagoDto formato = null;

            try
            {
                formato = await _repo.ObtenerFormatoSolicitudPago(cdpId);
                if (formato != null)
                {
                    var CantidadMaxima = _planPagoRepository.ObtenerCantidadMaximaPlanPago(formato.Cdp.Crp);

                    formato.CantidadMaxima = CantidadMaxima;
                    formato.ValorPagadoFechaActual = formato.Cdp.ValorTotal - formato.Cdp.SaldoActual;

                    var pagosRealizados = await _repo.ObtenerPagosRealizadosXCompromiso(formato.Cdp.Crp);
                    if (pagosRealizados != null)
                    {
                        formato.NumeroPagoFechaActual = pagosRealizados.Count;
                        formato.PagosRealizados = pagosRealizados;
                    }
                }
                return Ok(formato);
            }
            catch (Exception)
            {
                throw;
            }

            throw new Exception($"No se pudo obtener la lista de relaciones contables");
        }

        [Route("[action]")]
        [HttpPost]
        public async Task<IActionResult> RegistrarFormatoSolicitudPago(FormatoSolicitudPagoParaGuardarDto formatoDto)
        {
            usuarioId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            await using var transaction = await _dataContext.Database.BeginTransactionAsync();
            FormatoSolicitudPago formato = null;

            try
            {
                if (formatoDto != null)
                {
                    #region Mapear datos 

                    formato = _mapper.Map<FormatoSolicitudPago>(formatoDto);
                    formato.ActividadEconomicaId = formatoDto.ActividadEconomicaId;
                    formato.EstadoId = (int)EstadoSolicitudPago.Generado;
                    formato.UsuarioIdRegistro = usuarioId;
                    formato.FechaRegistro = _generalInterface.ObtenerFechaHoraActual();

                    #endregion Mapear datos 

                    //Registrar detalle de liquidación
                    _dataContext.FormatoSolicitudPago.Add(formato);
                    await _dataContext.SaveChangesAsync();

                    await transaction.CommitAsync();

                    return Ok(formato.FormatoSolicitudPagoId);
                }
            }
            catch (Exception)
            {
                throw;
            }

            throw new Exception($"No se pudo registrar el formato de liquidación");
        }

        [Route("[action]")]
        [HttpGet]
        public async Task<IActionResult> ObtenerSeguridadSocialParaSolicitudPago([FromQuery(Name = "planPagoId")] int planPagoId,
                                                                                            [FromQuery(Name = "valorBaseCotizacion")] decimal valorBaseCotizacion,
                                                                                            [FromQuery(Name = "actividadEconomicaId")] int? actividadEconomicaId)
        {
            FormatoCausacionyLiquidacionPagos formato = null;
            try
            {

                formato = await _procesoLiquidacion.ObtenerFormatoSolicitudPago(planPagoId, valorBaseCotizacion, actividadEconomicaId);
            }
            catch (Exception)
            {
                throw;
            }
            return base.Ok(formato);
        }
    }
}