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

namespace ComplementApp.API.Controllers
{
    public class CargaObligacionController : BaseApiController
    {
        #region Variable

        int pciId = 0;
        string valorPciId = string.Empty;

        #endregion 

        #region Dependency Injection

        private readonly DataContext _dataContext;
        private readonly ICargaObligacionService _cargaService;
        private readonly ICargaObligacionRepository _repo;

        private readonly IMapper _mapper;
        #endregion Dependency Injection

        public CargaObligacionController(DataContext dataContext, ICargaObligacionService cargaService,
        ICargaObligacionRepository repo, IMapper mapper)

        {
            _repo = repo;
            _cargaService = cargaService;
            _dataContext = dataContext;
            _mapper = mapper;

        }

        [HttpPost]
        [Route("upload")]
        public IActionResult RegistrarCargaObligacion()
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

                    List<CargaObligacion> listaDocumento = _cargaService.ObtenerListaCargaObligacion(pciId, dtCabecera);
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

        [Route("[action]")]
        [HttpGet]
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
    }
}