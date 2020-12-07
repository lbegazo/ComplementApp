using System;
using System.Collections.Generic;
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
        #region Dependency Injection

        private readonly DataContext _dataContext;
        private readonly IObligacionRepository _repo;
        private readonly IMapper _mapper;
        private readonly IGeneralInterface _generalInterface;

        private readonly IListaRepository _listaRepository;

        #endregion Dependency Injection


        public ObligacionController(IObligacionRepository repo, IMapper mapper,
                            DataContext dataContext, IGeneralInterface generalInterface,
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
    }
}