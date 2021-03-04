using System;
using System.Collections.Generic;
using System.Data;
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
        private readonly IProcesoCreacionArchivoExcel _procesoCreacionExcelInterface;

        #endregion Dependency Injection


        public TerceroController(ITerceroRepository repo, IMapper mapper,
                                    DataContext dataContext,
                                    IGeneralInterface generalInterface,
                                    IListaRepository listaRepository,
                                    IProcesoCreacionArchivoExcel procesoCreacionExcelInterface)
        {
            _mapper = mapper;
            _repo = repo;
            _dataContext = dataContext;
            _generalInterface = generalInterface;
            _listaRepository = listaRepository;
            _procesoCreacionExcelInterface = procesoCreacionExcelInterface;
        }

        #region Tercero

        [Route("[action]")]
        [HttpPost]
        public async Task<IActionResult> RegistrarTercero(TerceroDto terceroDto)
        {
            usuarioId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            Tercero tercero = new Tercero();

            await using var transaction = await _dataContext.Database.BeginTransactionAsync();

            bool existe = await _repo.ValidarExistenciaTercero(terceroDto.TipoDocumentoIdentidadId, terceroDto.NumeroIdentificacion);

            try
            {
                if (!existe)
                {
                    if (terceroDto != null)
                    {
                        #region Mapear datos Tercero

                        tercero.TipoIdentificacion = terceroDto.TipoDocumentoIdentidadId;
                        tercero.NumeroIdentificacion = terceroDto.NumeroIdentificacion;
                        tercero.Nombre = terceroDto.Nombre;
                        tercero.FechaExpedicionDocumento = terceroDto.FechaExpedicionDocumento;
                        tercero.Telefono = terceroDto.Telefono;
                        tercero.Email = terceroDto.Email;
                        tercero.Direccion = terceroDto.Direccion;
                        tercero.DeclaranteRenta = terceroDto.DeclaranteRenta;

                        if (terceroDto.FacturadorElectronico)
                        {
                            tercero.RegimenTributario = "RESPONSABLE DE IVA";
                        }
                        else
                        {
                            tercero.RegimenTributario = "NO RESPONSABLE DE IVA";
                        }

                        tercero.UsuarioIdRegistro = usuarioId;
                        tercero.FechaRegistro = _generalInterface.ObtenerFechaHoraActual();

                        #endregion Mapear datos Tercero

                        //Registrar Parametro liquidación Tercero
                        _dataContext.Tercero.Add(tercero);
                        await _dataContext.SaveChangesAsync();

                        await transaction.CommitAsync();

                        return Ok(tercero.TerceroId);
                    }
                }
                else
                {
                    return Ok(0);
                }
            }
            catch (Exception)
            {
                throw;
            }
            throw new Exception($"No se pudo registrar el tercero");
        }

        [Route("[action]")]
        [HttpPut]
        public async Task<IActionResult> ActualizarTercero(TerceroDto terceroDto)
        {
            await using var transaction = await _dataContext.Database.BeginTransactionAsync();

            try
            {
                //Registrar Parametro liquidación Tercero
                var terceroBD = await _repo.ObtenerTerceroBase(terceroDto.TerceroId);

                #region Mapear datos Tercero

                terceroBD.Nombre = terceroDto.Nombre;
                terceroBD.FechaExpedicionDocumento = terceroDto.FechaExpedicionDocumento;
                terceroBD.Telefono = terceroDto.Telefono;
                terceroBD.Email = terceroDto.Email;
                terceroBD.Direccion = terceroDto.Direccion;
                terceroBD.DeclaranteRenta = terceroDto.DeclaranteRenta;

                if (terceroDto.FacturadorElectronico)
                {
                    terceroBD.RegimenTributario = "RESPONSABLE DE IVA";
                }
                else
                {
                    terceroBD.RegimenTributario = "NO RESPONSABLE DE IVA";
                }

                terceroBD.UsuarioIdModificacion = usuarioId;
                terceroBD.FechaModificacion = _generalInterface.ObtenerFechaHoraActual();

                #endregion Mapear datos Tercero

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
        public async Task<ActionResult> ObtenerTercero([FromQuery(Name = "terceroId")] int terceroId)
        {
            try
            {
                var item = await _repo.ObtenerTercero(terceroId);
                return Ok(item);
            }
            catch (Exception)
            {
                throw;
            }

            throw new Exception($"No se pudo obtener información para la parametrización del tercero");
        }

        [Route("[action]")]
        [HttpGet]
        public async Task<ActionResult> ObtenerTerceros([FromQuery(Name = "tipo")] int tipo,
                                                        [FromQuery(Name = "terceroId")] int? terceroId,
                                                        [FromQuery] UserParams userParams)
        {
            try
            {
                var pagedList = await _repo.ObtenerTerceros(terceroId, userParams);
                var listaDto = _mapper.Map<IEnumerable<TerceroDto>>(pagedList);

                Response.AddPagination(pagedList.CurrentPage, pagedList.PageSize,
                                    pagedList.TotalCount, pagedList.TotalPages);

                return Ok(listaDto);
            }
            catch (Exception)
            {
                throw;
            }

            throw new Exception($"No se pudo obtener la lista de terceros");
        }


        [Route("[action]")]
        [HttpGet]
        public async Task<IActionResult> DescargarListaTercero()
        {
            string nombreArchivo = "Tercero.xlsx";
            try
            {
                var lista = await _repo.ObtenerListaTercero();
                if (lista != null)
                {
                    DataTable dtResultado = _procesoCreacionExcelInterface.ObtenerTablaDeListaTercero(lista.ToList());
                    return _procesoCreacionExcelInterface.ExportExcel(Response, dtResultado, nombreArchivo);
                }
            }
            catch (Exception)
            {
                throw;
            }
            return BadRequest();
        }

        #endregion Tercero

        #region Parametrizacion Liquidacion Tercero

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
            ICollection<TerceroDeduccionDto> lista = null;
            try
            {
                var item = await _repo.ObtenerParametrizacionLiquidacionXTercero(terceroId);
                if (item != null)
                {
                    lista = await _repo.ObtenerDeduccionesXTercero(terceroId);
                    item.TerceroDeducciones = lista;
                }
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

                    parametro.Subcontrata = parametroDto.SubcontrataId == 1 ? true : false;
                    parametro.FacturaElectronica = parametroDto.FacturaElectronicaId == 1 ? true : false;
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
                            itemTerceroDeduccion.TerceroId = item.Tercero.Id;
                            itemTerceroDeduccion.ActividadEconomicaId = item.ActividadEconomica.Id;

                            if (item.Deduccion.DeduccionId > 0)
                            {
                                itemTerceroDeduccion.DeduccionId = item.Deduccion.DeduccionId;
                                itemTerceroDeduccion.TerceroDeDeduccionId = item.TerceroDeDeduccion.Id;
                            }
                            else
                            {
                                itemTerceroDeduccion.DeduccionId = null;
                                itemTerceroDeduccion.TerceroDeDeduccionId = null;
                            }
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

                parametroBD.FacturaElectronica = parametroDto.FacturaElectronicaId == 1 ? true : false;
                parametroBD.Subcontrata = parametroDto.SubcontrataId == 1 ? true : false;
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
                        itemTerceroDeduccion.TerceroId = item.Tercero.Id;
                        itemTerceroDeduccion.ActividadEconomicaId = item.ActividadEconomica.Id;

                        if (item.Deduccion.DeduccionId > 0)
                        {
                            itemTerceroDeduccion.DeduccionId = item.Deduccion.DeduccionId;
                            itemTerceroDeduccion.TerceroDeDeduccionId = item.TerceroDeDeduccion.Id;
                        }
                        else
                        {
                            itemTerceroDeduccion.DeduccionId = null;
                            itemTerceroDeduccion.TerceroDeDeduccionId = null;
                        }
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

        #endregion Parametrizacion Liquidacion Tercero
    }
}