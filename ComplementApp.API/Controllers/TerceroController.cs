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
using ComplementApp.API.Interfaces.Repository;
using ComplementApp.API.Models;
using Microsoft.AspNetCore.Mvc;

namespace ComplementApp.API.Controllers
{
   
    public class TerceroController : BaseApiController
    {
        #region Variable

        int usuarioId = 0;
        int pciId = 0;
        string valorPciId = string.Empty;

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
                valorPciId = User.FindFirst(ClaimTypes.Role).Value;
                if (!string.IsNullOrEmpty(valorPciId))
                {
                    pciId = int.Parse(valorPciId);
                }
                userParams.PciId = pciId;
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
                valorPciId = User.FindFirst(ClaimTypes.Role).Value;
                if (!string.IsNullOrEmpty(valorPciId))
                {
                    pciId = int.Parse(valorPciId);
                }
                var item = await _repo.ObtenerParametrizacionLiquidacionXTercero(terceroId, pciId);
                if (item != null)
                {
                    lista = await _repo.ObtenerDeduccionesXTercero(item.ParametroLiquidacionTerceroId);
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

            ParametroLiquidacionTercero parametro = new ParametroLiquidacionTercero();
            var listaTerceroDeduccion = new List<TerceroDeduccion>();
            TerceroDeduccion itemTerceroDeduccion = null;

            await using var transaction = await _dataContext.Database.BeginTransactionAsync();

            try
            {
                usuarioId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
                valorPciId = User.FindFirst(ClaimTypes.Role).Value;
                if (!string.IsNullOrEmpty(valorPciId))
                {
                    pciId = int.Parse(valorPciId);
                }

                if (parametroDto != null)
                {
                    #region Mapear datos Parametro Liquidacion Tercero

                    parametro = _mapper.Map<ParametroLiquidacionTercero>(parametroDto);

                    parametro.Subcontrata = parametroDto.SubcontrataId == 1 ? true : false;
                    parametro.FacturaElectronica = parametroDto.FacturaElectronicaId == 1 ? true : false;
                    parametro.UsuarioIdRegistro = usuarioId;
                    parametro.PciId = pciId;
                    parametro.FechaRegistro = _generalInterface.ObtenerFechaHoraActual();

                    #endregion Mapear datos Parametro Liquidacion Tercero

                    //Registrar Parametro liquidación Tercero
                    _dataContext.ParametroLiquidacionTercero.Add(parametro);
                    await _dataContext.SaveChangesAsync();

                    //Eliminar Tercero deducciones
                    await _repo.EliminarTerceroDeduccionesXTercero(parametroDto.TerceroId, pciId);
                    await _dataContext.SaveChangesAsync();

                    //Registrar Tercero deducciones
                    if (parametroDto.TerceroDeducciones != null && parametroDto.TerceroDeducciones.Count > 0)
                    {
                        foreach (var item in parametroDto.TerceroDeducciones)
                        {
                            itemTerceroDeduccion = new TerceroDeduccion();
                            itemTerceroDeduccion.ParametroLiquidacionTerceroId = parametro.ParametroLiquidacionTerceroId;
                            itemTerceroDeduccion.TerceroId = item.Tercero.Id;
                            itemTerceroDeduccion.ActividadEconomicaId = item.ActividadEconomica.Id;

                            if (item.Deduccion.DeduccionId > 0)
                            {
                                itemTerceroDeduccion.DeduccionId = item.Deduccion.DeduccionId;
                                itemTerceroDeduccion.TerceroDeDeduccionId = item.TerceroDeDeduccion.Id;

                                if (item.Deduccion.EsValorFijo)
                                {
                                    itemTerceroDeduccion.ValorFijo = item.ValorFijo;
                                }
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
                valorPciId = User.FindFirst(ClaimTypes.Role).Value;
                if (!string.IsNullOrEmpty(valorPciId))
                {
                    pciId = int.Parse(valorPciId);
                }
                //Registrar Parametro liquidación Tercero
                var parametroBD = await _repo.ObtenerParametrizacionLiquidacionTerceroBase(parametroDto.ParametroLiquidacionTerceroId);
                _mapper.Map(parametroDto, parametroBD);

                parametroBD.FacturaElectronica = parametroDto.FacturaElectronicaId == 1 ? true : false;
                parametroBD.Subcontrata = parametroDto.SubcontrataId == 1 ? true : false;
                parametroBD.UsuarioIdModificacion = usuarioId;
                parametroBD.FechaModificacion = _generalInterface.ObtenerFechaHoraActual();

                //Registrar nueva lista de Tercero deducciones
                if (parametroDto.TerceroDeducciones != null && parametroDto.TerceroDeducciones.Count > 0)
                {
                    #region Eliminar Tercero Deducción

                    var listaTerceroDeduccionEliminar = parametroDto.TerceroDeducciones.Where(x => x.EstadoModificacion == (int)EstadoModificacion.Eliminado).ToList();

                    if (listaTerceroDeduccionEliminar != null && listaTerceroDeduccionEliminar.Count > 0)
                    {
                        foreach (var item in listaTerceroDeduccionEliminar)
                        {
                            await _repo.EliminarTerceroDeduccion(item.TerceroDeduccionId);
                        }
                    }

                    #endregion Eliminar Tercero Deducción

                    #region Insertar Tercero Deducción

                    var listaTerceroDeduccionNuevo = parametroDto.TerceroDeducciones.Where(x => x.EstadoModificacion == (int)EstadoModificacion.Insertado).ToList();

                    if (listaTerceroDeduccionNuevo != null && listaTerceroDeduccionNuevo.Count > 0)
                    {
                        foreach (var item in listaTerceroDeduccionNuevo)
                        {
                            itemTerceroDeduccion = new TerceroDeduccion();
                            itemTerceroDeduccion.TerceroId = item.Tercero.Id;
                            itemTerceroDeduccion.ParametroLiquidacionTerceroId = parametroBD.ParametroLiquidacionTerceroId;
                            itemTerceroDeduccion.ActividadEconomicaId = item.ActividadEconomica.Id;

                            if (item.Deduccion.DeduccionId > 0)
                            {
                                itemTerceroDeduccion.DeduccionId = item.Deduccion.DeduccionId;
                                itemTerceroDeduccion.TerceroDeDeduccionId = item.TerceroDeDeduccion.Id;
                                if (item.Deduccion.EsValorFijo)
                                {
                                    itemTerceroDeduccion.ValorFijo = item.ValorFijo;
                                }
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

                    #endregion Insertar Tercero Deducción

                    #region Modificar Tercero Deducción

                    var listaTerceroDeduccionModificar = parametroDto.TerceroDeducciones.Where(x => x.EstadoModificacion == (int)EstadoModificacion.Modificado).ToList();

                    if (listaTerceroDeduccionModificar != null && listaTerceroDeduccionModificar.Count > 0)
                    {
                        foreach (var item in listaTerceroDeduccionModificar)
                        {
                            var terceroDeduccionBD = await _repo.ObtenerTerceroDeduccionBase(item.TerceroDeduccionId);

                            if (terceroDeduccionBD != null)
                            {
                                terceroDeduccionBD.TerceroId = item.Tercero.Id;
                                terceroDeduccionBD.ActividadEconomicaId = item.ActividadEconomica.Id;

                                if (item.Deduccion.DeduccionId > 0)
                                {
                                    terceroDeduccionBD.DeduccionId = item.Deduccion.DeduccionId;
                                    terceroDeduccionBD.TerceroDeDeduccionId = item.TerceroDeDeduccion.Id;
                                    if (item.Deduccion.EsValorFijo)
                                    {
                                        terceroDeduccionBD.ValorFijo = item.ValorFijo;
                                    }
                                }
                                else
                                {
                                    terceroDeduccionBD.DeduccionId = null;
                                    terceroDeduccionBD.TerceroDeDeduccionId = null;
                                }
                            }
                            await _dataContext.SaveChangesAsync();
                        }
                    }

                    #endregion Modificar Tercero Deducción

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
                valorPciId = User.FindFirst(ClaimTypes.Role).Value;
                if (!string.IsNullOrEmpty(valorPciId))
                {
                    pciId = int.Parse(valorPciId);
                }
                var lista = await _repo.ObtenerDeduccionesXTercero(terceroId, pciId, null);
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

        [Route("[action]")]
        [HttpGet]
        public async Task<IActionResult> DescargarListaParametroLiquidacionTerceroTotal([FromQuery(Name = "tipoArchivo")] int tipoArchivo)
        {
            string nombreArchivo = string.Empty;
            ICollection<ParametroLiquidacionTerceroDto> listaParametroLiquidacion = null;
            ICollection<TerceroDeduccionDto> listaDeducciones = null;
            try
            {
                valorPciId = User.FindFirst(ClaimTypes.Role).Value;
                if (!string.IsNullOrEmpty(valorPciId))
                {
                    pciId = int.Parse(valorPciId);
                }
                if (tipoArchivo == 1)
                {
                    nombreArchivo = "SIGPAA_ParametroLiquidacionTercero.xlsx";
                    listaParametroLiquidacion = await _repo.ObtenerListaParametroLiquidacionTerceroTotal(pciId);

                    if (listaParametroLiquidacion != null)
                    {
                        DataTable dtResultado = _procesoCreacionExcelInterface.ObtenerTablaDeListaParametroLiquidacionTercero(listaParametroLiquidacion.ToList());
                        return _procesoCreacionExcelInterface.ExportExcel(Response, dtResultado, nombreArchivo);
                    }
                }
                else
                {
                    nombreArchivo = "SIGPAA_Deducciones.xlsx";
                    listaDeducciones = await _repo.ObtenerListaTerceroDeduccionTotal(pciId);

                    if (listaDeducciones != null)
                    {
                        DataTable dtResultado = _procesoCreacionExcelInterface.ObtenerTablaDeListaTerceroDeduccion(listaDeducciones.ToList());
                        return _procesoCreacionExcelInterface.ExportExcel(Response, dtResultado, nombreArchivo);
                    }
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
        public async Task<IActionResult> DescargarListaActividadEconomica()
        {
            string nombreArchivo = "ActividadEconomica.xlsx";
            try
            {
                var lista = await _repo.DescargarListaActividadEconomica();
                if (lista != null)
                {
                    DataTable dtResultado = _procesoCreacionExcelInterface.ObtenerTablaDeListaActividadEconomica(lista.ToList());
                    return _procesoCreacionExcelInterface.ExportExcel(Response, dtResultado, nombreArchivo);
                }
            }
            catch (Exception)
            {
                throw;
            }
            return BadRequest();
        }

        #endregion Parametrizacion Liquidacion Tercero
    }
}