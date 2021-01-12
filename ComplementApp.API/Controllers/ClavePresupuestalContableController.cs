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
    public class ClavePresupuestalContableController : ControllerBase
    {
        #region Dependency Injection
        private readonly DataContext _dataContext;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IClavePresupuestalContableRepository _repo;
        private readonly IMapper _mapper;
        private readonly IGeneralInterface _generalInterface;

        private readonly IListaRepository _listaRepository;

        #endregion Dependency Injection

        public ClavePresupuestalContableController(IUnitOfWork unitOfWork, IClavePresupuestalContableRepository repo,
                                    IMapper mapper, DataContext dataContext, IListaRepository listaRepository,
                                    IGeneralInterface generalInterface)
        {
            this._mapper = mapper;
            this._repo = repo;
            this._unitOfWork = unitOfWork;
            _dataContext = dataContext;
            this._generalInterface = generalInterface;
            this._listaRepository = listaRepository;
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
        public async Task<ActionResult> ObtenerRubrosPresupuestalesXCompromiso([FromQuery(Name = "crp")] int crp)
        {
            try
            {
                string identificacionPCI = string.Empty;

                ValorSeleccion parametroPCI = await _listaRepository.ObtenerParametroGeneralXNombre("Pci-ANE");
                if (identificacionPCI != null)
                {
                    identificacionPCI = parametroPCI.Valor;
                }

                var lista = await _repo.ObtenerRubrosPresupuestalesXCompromiso(crp);

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
        public async Task<ActionResult> ObtenerRelacionesContableXRubroPresupuestal([FromQuery(Name = "rubroPresupuestalId")] int rubroPresupuestalId)
        {
            try
            {
                var lista = await _repo.ObtenerRelacionesContableXRubroPresupuestal(rubroPresupuestalId);
                return Ok(lista);
            }
            catch (Exception)
            {
                throw;
            }

            throw new Exception($"No se pudo obtener la lista de relaciones contables");
        }

        [Route("[action]")]
        [HttpGet]
        public async Task<ActionResult> ObtenerUsosPresupuestalesXRubroPresupuestal([FromQuery(Name = "rubroPresupuestalId")] int rubroPresupuestalId)
        {
            try
            {
                var lista = await _repo.ObtenerUsosPresupuestalesXRubroPresupuestal(rubroPresupuestalId);
                return Ok(lista);
            }
            catch (Exception)
            {
                throw;
            }

            throw new Exception($"No se pudo obtener la lista de usos presupuestales");
        }


        [Route("[action]")]
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
        [HttpPost]
        public async Task<IActionResult> RegistrarClavePresupuestalContable(ClavePresupuestalContableDto[] lista)
        {
            await using var transaction = await _dataContext.Database.BeginTransactionAsync();
            List<ClavePresupuestalContable> listaClavePresupuestal = new List<ClavePresupuestalContable>();
            ClavePresupuestalContable clavePresupuestalContable = null;
            try
            {
                foreach (var item in lista)
                {
                    clavePresupuestalContable = new ClavePresupuestalContable();
                    clavePresupuestalContable.Pci = item.Pci;
                    clavePresupuestalContable.Crp = item.Crp;
                    clavePresupuestalContable.Dependencia = item.Dependencia;
                    clavePresupuestalContable.RubroPresupuestalId = item.RubroPresupuestal.Id;
                    clavePresupuestalContable.TerceroId = item.Tercero.Id;
                    clavePresupuestalContable.SituacionFondoId = item.SituacionFondo.Id;
                    clavePresupuestalContable.FuenteFinanciacionId = item.FuenteFinanciacion.Id;
                    clavePresupuestalContable.RecursoPresupuestalId = item.RecursoPresupuestal.Id;
                    clavePresupuestalContable.UsoPresupuestalId = item.UsoPresupuestal.Id;
                    clavePresupuestalContable.RelacionContableId = item.RelacionContable.Id;

                    listaClavePresupuestal.Add(clavePresupuestalContable);
                }

                await _dataContext.ClavePresupuestalContable.AddRangeAsync(listaClavePresupuestal);
                await _dataContext.SaveChangesAsync();

                await transaction.CommitAsync();

                return Ok(1);
            }
            catch (Exception)
            {
                throw;
            }
        }

    }
}