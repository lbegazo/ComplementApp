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
    public class TerceroController : ControllerBase
    {
        #region Variable

        int usuarioId = 0;

        #endregion 


        #region Dependency Injection

        private readonly DataContext _dataContext;
        private readonly ITerceroRepository _repo;
        private readonly IGeneralInterface _generalInterface;

        private readonly IListaRepository _listaRepository;

        private readonly IMapper _mapper;

        #endregion Dependency Injection


        public TerceroController(ITerceroRepository repo, IMapper mapper,
                                    DataContext dataContext,
                                    IGeneralInterface generalInterface,
                                    IListaRepository listaRepository)
        {
            _mapper = mapper;
            _repo = repo;
            _dataContext = dataContext;
            _generalInterface = generalInterface;
            _listaRepository = listaRepository;
        }

        [Route("[action]")]
        [HttpGet]
        public async Task<ActionResult> ObtenerTercerosParaParametrizacionLiquidacion([FromQuery(Name = "tipo")] int tipo,
                                                                                        [FromQuery(Name = "terceroId")] int? terceroId,
                                                                                        [FromQuery] UserParams userParams)
        {
            try
            {
                var pagedList = await _repo.ObtenerTercerosParaParametrizacionLiquidacion(tipo, terceroId, userParams);
                var listaDto = _mapper.Map<IEnumerable<TerceroDto>>(pagedList);

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
        public async Task<ActionResult> ObtenerParametrizacionLiquidacionXTercero([FromQuery(Name = "terceroId")] int terceroId)
        {
            try
            {
                var item = await _repo.ObtenerParametrizacionLiquidacionXTercero(terceroId);
                item.TerceroDeducciones = await _repo.ObtenerDeduccionesXTercero(terceroId);
                return Ok(item);
            }
            catch (Exception)
            {
                throw;
            }

            throw new Exception($"No se pudo obtener información para la parametrización del tercero");
        }


        [Route("[action]")]
        [HttpPost]
        public async Task<IActionResult> RegistrarParametroLiquidacionTercero(ParametroLiquidacionTerceroDto parametroDto)
        {
            usuarioId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            ParametroLiquidacionTercero parametro = new ParametroLiquidacionTercero();
            var listaTerceroDeduccion = new List<TerceroDeduccion>();
            TerceroDeduccion itemTerceroDeduccion = null;

            await using var transaction = await _dataContext.Database.BeginTransactionAsync();

            try
            {
                if (parametroDto != null)
                {
                    #region Mapear datos Parametro Liquidacion Tercero

                    parametro = _mapper.Map<ParametroLiquidacionTercero>(parametroDto);

                    parametro.Subcontrata = parametroDto.subcontrataId == 1 ? true : false;
                    parametro.FacturaElectronica = parametroDto.facturaElectronicaId == 1 ? true : false;
                    parametro.UsuarioIdRegistro = usuarioId;
                    parametro.FechaRegistro = _generalInterface.ObtenerFechaHoraActual();

                    #endregion Mapear datos Parametro Liquidacion Tercero

                    //Registrar Parametro liquidación Tercero
                    _dataContext.ParametroLiquidacionTercero.Add(parametro);
                    await _dataContext.SaveChangesAsync();

                    //Eliminar Tercero deducciones
                    await _repo.EliminarTerceroDeduccionesXTercero(parametroDto.TerceroId);
                    await _dataContext.SaveChangesAsync();

                    //Registrar Tercero deducciones
                    if (parametroDto.TerceroDeducciones != null && parametroDto.TerceroDeducciones.Count > 0)
                    {
                        foreach (var item in parametroDto.TerceroDeducciones)
                        {
                            itemTerceroDeduccion = new TerceroDeduccion();
                            itemTerceroDeduccion.ActividadEconomicaId = item.ActividadEconomica.Id;
                            itemTerceroDeduccion.TerceroId = item.Tercero.Id;
                            itemTerceroDeduccion.DeduccionId = item.Deduccion.Id;
                            listaTerceroDeduccion.Add(itemTerceroDeduccion);
                        }
                        _dataContext.TerceroDeducciones.AddRange(listaTerceroDeduccion);
                        await _dataContext.SaveChangesAsync();
                    }

                    await transaction.CommitAsync();

                    return Ok(parametro.ParametroLiquidacionTerceroId);
                }
            }
            catch (Exception)
            {
                throw;
            }
            throw new Exception($"No se pudo registrar los parametros de liquidación de tercero");
        }

        [Route("[action]")]
        [HttpPut("{id}")]
        public async Task<IActionResult> ActualizarParametroLiquidacionTercero(ParametroLiquidacionTerceroDto parametroDto)
        {
            var listaTerceroDeduccion = new List<TerceroDeduccion>();
            TerceroDeduccion itemTerceroDeduccion = null;
            await using var transaction = await _dataContext.Database.BeginTransactionAsync();

            try
            {
                //Registrar Parametro liquidación Tercero
                var parametroBD = await _repo.ObtenerParametrizacionLiquidacionTerceroBase(parametroDto.ParametroLiquidacionTerceroId);
                _mapper.Map(parametroDto, parametroBD);

                parametroBD.FacturaElectronica = parametroDto.facturaElectronicaId == 1 ? true : false;
                parametroBD.Subcontrata = parametroDto.subcontrataId == 1 ? true : false;
                parametroBD.UsuarioIdModificacion = usuarioId;
                parametroBD.FechaModificacion = _generalInterface.ObtenerFechaHoraActual();

                //Eliminar Tercero deducciones
                await _repo.EliminarTerceroDeduccionesXTercero(parametroDto.TerceroId);
                await _dataContext.SaveChangesAsync();

                //Registrar nueva lista de Tercero deducciones
                if (parametroDto.TerceroDeducciones != null && parametroDto.TerceroDeducciones.Count > 0)
                {
                    foreach (var item in parametroDto.TerceroDeducciones)
                    {
                        itemTerceroDeduccion = new TerceroDeduccion();
                        itemTerceroDeduccion.ActividadEconomicaId = item.ActividadEconomica.Id;
                        itemTerceroDeduccion.TerceroId = item.Tercero.Id;
                        itemTerceroDeduccion.DeduccionId = item.Deduccion.Id;
                        itemTerceroDeduccion.TerceroDeDeduccionId = item.TerceroDeDeduccion.Id;
                        listaTerceroDeduccion.Add(itemTerceroDeduccion);
                    }
                    _dataContext.TerceroDeducciones.AddRange(listaTerceroDeduccion);
                    await _dataContext.SaveChangesAsync();
                }

                await _dataContext.SaveChangesAsync();
                await transaction.CommitAsync();

                return NoContent();
            }
            catch (Exception)
            {
                throw;
            }
        }

        [Route("[action]")]
        [HttpGet]
        public async Task<ActionResult> ObtenerDeduccionesXTercero([FromQuery(Name = "terceroId")] int terceroId)
        {
            try
            {
                var lista = await _repo.ObtenerDeduccionesXTercero(terceroId);
                return Ok(lista);
            }
            catch (Exception)
            {
                throw;
            }

            throw new Exception($"No se pudo obtener las deducciones del tercero");
        }

        [Route("[action]")]
        [HttpGet]
        public async Task<ActionResult> ObteneListaDeducciones([FromQuery] UserParams userParams)
        {
            try
            {
                var pagedList = await _repo.ObteneListaDeducciones(userParams);

                var listaDto = _mapper.Map<IEnumerable<DeduccionDto>>(pagedList);

                return Ok(listaDto);
            }
            catch (Exception)
            {
                throw;
            }

            throw new Exception($"No se pudo obtener las deducciones");
        }

    }
}