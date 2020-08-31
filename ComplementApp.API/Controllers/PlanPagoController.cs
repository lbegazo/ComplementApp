using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using ComplementApp.API.Data;
using ComplementApp.API.Dtos;
using ComplementApp.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace ComplementApp.API.Controllers
{
    //[Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class PlanPagoController : ControllerBase
    {
        private readonly DataContext _dataContext;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPlanPagoRepository _repo;
        private readonly IMapper _mapper;
        public PlanPagoController(IUnitOfWork unitOfWork, IPlanPagoRepository repo, IMapper mapper, DataContext dataContext)
        {
            this._mapper = mapper;
            this._repo = repo;
            this._unitOfWork = unitOfWork;
             _dataContext = dataContext;
        }

        [Route("[action]")]
        [HttpGet]
        public async Task<IActionResult> ObtenerListaPlanPago([FromQuery(Name = "terceroId")] int terceroId,
                                                                [FromQuery(Name = "listaEstadoId")] string listaEstadoId)
        {


            if (terceroId > 0 && !string.IsNullOrEmpty(listaEstadoId))
            {
                List<int> listIds = listaEstadoId.Split(',').Select(int.Parse).ToList();
                var lista = await _repo.ObtenerListaPlanPago(terceroId, listIds);
                var listaDto = _mapper.Map<IEnumerable<PlanPagoDto>>(lista);
                return base.Ok(listaDto);
            }
            return base.Ok();
        }

        [Route("[action]")]
        [HttpGet]
        public async Task<IActionResult> ObtenerPlanPago([FromQuery(Name = "planPagoId")] int planPagoId)
        {
            var planPagoBD = await _repo.ObtenerPlanPago(planPagoId);
            var planPagoDto = _mapper.Map<PlanPagoDto>(planPagoBD);
            return base.Ok(planPagoDto);
        }

        [HttpPut]
        public async Task<IActionResult> ActualizarPlanPago(PlanPagoDto planPagoDto)
        {
            //PlanPagoDto planPagoDto = null;
            if (planPagoDto != null)
            {
                //Obtener todos los campos del plan de pago para una correcta actualización
                var planPagoBD = await _repo.ObtenerPlanPago(planPagoDto.PlanPagoId);

                _mapper.Map(planPagoDto, planPagoBD);

                if (planPagoDto.esRadicarFactura)
                {
                    planPagoBD.EstadoPlanPagoId = (int)EstadoPlanPago.PorObligar;
                    planPagoBD.FechaFactura = System.DateTime.Now;
                }
                else
                {
                    planPagoBD.EstadoPlanPagoId = (int)EstadoPlanPago.PorObligar;
                    planPagoBD.FechaFactura = System.DateTime.Now;
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
    }
}