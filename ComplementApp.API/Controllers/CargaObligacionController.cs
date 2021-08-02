using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Threading.Tasks;
using AutoMapper;
using ComplementApp.API.Data;
using ComplementApp.API.Dtos;
using ComplementApp.API.Helpers;
using ComplementApp.API.Interfaces.Repository;
using ComplementApp.API.Interfaces.Service;
using ComplementApp.API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text;
using System.Linq;
using ComplementApp.API.Interfaces;

namespace ComplementApp.API.Controllers
{
    public class CargaObligacionController : BaseApiController
    {
        #region Variable
        int usuarioId = 0;
        int pciId = 0;
        string valorPciId = string.Empty;

        #endregion 

        #region Dependency Injection

        private readonly DataContext _dataContext;
        private readonly ICargaObligacionService _cargaService;
        private readonly ICargaObligacionRepository _repo;
        private readonly IGeneralInterface _generalInterface;
        private readonly IMapper _mapper;
        private readonly IProcesoCreacionArchivo _procesoCreacionArchivo;
        #endregion Dependency Injection


        public CargaObligacionController(DataContext dataContext, ICargaObligacionService cargaService,
        ICargaObligacionRepository repo, IMapper mapper, IGeneralInterface generalInterface, IProcesoCreacionArchivo procesoCreacionArchivo)

        {
            _generalInterface = generalInterface;
            _repo = repo;
            _cargaService = cargaService;
            _dataContext = dataContext;
            _mapper = mapper;
            _procesoCreacionArchivo = procesoCreacionArchivo;

        }

        [HttpPost]
        [Route("upload")]
        public async Task<IActionResult> RegistrarCargaObligacion()
        {
            if (Request.Form.Files.Count > 0)
            {
                valorPciId = User.FindFirst(ClaimTypes.Role).Value;
                if (!string.IsNullOrEmpty(valorPciId))
                {
                    pciId = int.Parse(valorPciId);
                }

                using var transaction = _dataContext.Database.BeginTransaction();

                try
                {
                    IFormFile file = Request.Form.Files[0];

                    if (file == null || file.Length <= 0)
                        return BadRequest("El archivo se encuentra vacío");

                    if (!Path.GetExtension(file.FileName).Equals(".xlsx", StringComparison.OrdinalIgnoreCase))
                        return BadRequest("El archivo no es soportado, el archivo debe tener la extensión: xlsx");

                    #region Obtener información del archivo excel

                    DataTable dtCabecera = _cargaService.ObtenerInformacionDeExcel(file);

                    #endregion Obtener información del archivo excel

                    #region Mapear datos en la lista de Dtos

                    List<CargaObligacion> listaDocumento = await _cargaService.ObtenerListaCargaObligacion(pciId, dtCabecera);
                    #endregion

                    var result = _repo.EliminarCargaObligacion();

                    #region Insertar lista en la base de datos

                    var EsCabeceraCorrecto = _repo.InsertarListaCargaObligacion(listaDocumento);

                    #endregion Insertar lista en la base de datos

                    if (!EsCabeceraCorrecto)
                        throw new ArgumentException("No se pudo registrar la carga de la obligación");

                    transaction.Commit();
                }
                catch (Exception)
                {
                    throw;
                }
            }
            else
            {
                return BadRequest("El archivo no pudo ser enviado al servidor web");
            }

            return Ok();
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> ObtenerListaCargaObligacion([FromQuery] string estado,
                                                                     [FromQuery] UserParams userParams)
        {
            valorPciId = User.FindFirst(ClaimTypes.Role).Value;
            if (!string.IsNullOrEmpty(valorPciId))
            {
                pciId = int.Parse(valorPciId);
            }
            userParams.PciId = pciId;

            var pagedList = await _repo.ObtenerListaCargaObligacion(estado, userParams);
            var listaDto = _mapper.Map<IEnumerable<CDPDto>>(pagedList);

            Response.AddPagination(pagedList.CurrentPage, pagedList.PageSize,
                                pagedList.TotalCount, pagedList.TotalPages);

            return base.Ok(listaDto);
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> DescargarArchivoCargaObligacion([FromQuery] int? tipoArchivoId,
                                                                         [FromQuery] string estado
                                                                                )
        {
            #region Variables

            List<int> listaIds = new List<int>();
            string cadena = string.Empty;
            string nombreArchivo = string.Empty;
            DateTime fecha = _generalInterface.ObtenerFechaHoraActual();

            #endregion Variables

            try
            {
                usuarioId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
                valorPciId = User.FindFirst(ClaimTypes.Role).Value;
                if (!string.IsNullOrEmpty(valorPciId))
                {
                    pciId = int.Parse(valorPciId);
                }

                switch (tipoArchivoId)
                {
                    case (int)TipoArchivoObligacion.Cabecera:
                        {
                            #region Cabecera

                            nombreArchivo = "SIGPAA Cabecera";

                            var lista = (await _repo.ObtenerListaCargaObligacionArchivoCabecera(usuarioId, estado, pciId));

                            List<int> listIds = lista.Select(x => x.Obligacion).ToList();
                            List<int> listDistinct = listIds.Distinct().ToList();

                            //Obtener información para el archivo
                            cadena = _cargaService.ObtenerInformacionOrdenPagoArchivoCabecera(lista.ToList());


                            //Registrar archivo y sus detalles
                            ArchivoDetalleLiquidacion archivo = _procesoCreacionArchivo.RegistrarArchivoDetalleLiquidacion(usuarioId, pciId, listDistinct,
                                                                                                    nombreArchivo, 0,
                                                                                                    (int)TipoDocumentoArchivo.OrdenPago);
                            _dataContext.SaveChanges();

                            _procesoCreacionArchivo.RegistrarDetalleArchivoLiquidacion(archivo.ArchivoDetalleLiquidacionId, listDistinct);
                            _dataContext.SaveChanges();

                            //Encoding.UTF8: Respeta las tildes en las palabras
                            byte[] byteArray = Encoding.UTF8.GetBytes(cadena);
                            MemoryStream stream = new MemoryStream(byteArray);

                            if (stream == null)
                                return new NoContentResult();
                            
                            Response.AddFileName(nombreArchivo);
                            return File(stream, "application/octet-stream", nombreArchivo);

                            #endregion Cabecera
                        };
                    case (int)TipoArchivoObligacion.Item:
                        {
                            #region Items

                            var lista = await _repo.ObtenerListaCargaObligacionArchivoDetalle(estado, pciId);

                            if (lista != null && lista.Count > 0)
                            {
                                //Obtener información para el archivo
                                cadena = _cargaService.ObtenerInformacionOrdenPagoArchivoDetalle(lista.ToList());

                                //Encoding.UTF8: Respeta las tildes en las palabras
                                byte[] byteArray = Encoding.UTF8.GetBytes(cadena);
                                MemoryStream stream = new MemoryStream(byteArray);

                                if (stream == null)
                                    return new NoContentResult();

                                nombreArchivo = "SIGPAA Detalle";
                                Response.AddFileName(nombreArchivo);
                                return File(stream, "application/octet-stream", nombreArchivo);
                            }
                            else
                            {
                                return new NoContentResult();
                            }

                            #endregion Items
                        };
                    default:
                        break;
                }

                throw new Exception("No se pudo crear el archivo de las órdenes de pago");

            }
            catch (Exception)
            {
                throw;
            }
        }



    }
}