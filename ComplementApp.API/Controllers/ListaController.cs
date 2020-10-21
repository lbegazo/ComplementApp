using System.Threading.Tasks;
using ComplementApp.API.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ComplementApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ListaController : ControllerBase
    {
        private readonly IListaRepository _repo;

        public ListaController(IListaRepository repo)
        {
            _repo = repo;
        }

        //[AllowAnonymous]
        [Route("[action]")]
        [HttpGet]
        public async Task<IActionResult> ObtenerAreas()
        {
            var datos = await _repo.ObtenerAreas();
            return Ok(datos);
        }


        [Route("[action]")]
        [HttpGet]
        public async Task<IActionResult> ObtenerCargos()
        {
            var datos = await _repo.ObtenerCargos();
            return Ok(datos);
        }

        [Route("[action]")]
        [HttpGet]
        public async Task<IActionResult> ObtenerListaTipoOperacion()
        {
            var datos = await _repo.ObtenerListaTipoOperacion();
            return Ok(datos);
        }

        [Route("[action]")]
        [HttpGet]
        public async Task<IActionResult> ObtenerListaTipoDetalleModificacion()
        {
            var datos = await _repo.ObtenerListaTipoDetalleModificacion();
            return Ok(datos);
        }

        [Route("[action]")]
        [HttpGet]
        public async Task<IActionResult> ObtenerListaTercero([FromQuery(Name = "numeroIdentificacion")] string numeroIdentificacion)
        {
            var datos = await _repo.ObtenerListaTercero(numeroIdentificacion);
            return Ok(datos);
        }

        [Route("[action]")]
        [HttpGet]
        public async Task<IActionResult> ObtenerListaPerfiles()
        {
            var datos = await _repo.ObtenerListaPerfiles();
            return Ok(datos);
        }

        [Route("[action]")]
        [HttpGet]
        public async Task<IActionResult> ObtenerParametrosGenerales()
        {
            var datos = await _repo.ObtenerParametrosGenerales();
            return Ok(datos);
        }

        [Route("[action]")]
        [HttpGet]
        public async Task<IActionResult> ObtenerParametroLiquidacionXTercero([FromQuery(Name = "terceroId")] int terceroId)
        {
            var datos = await _repo.ObtenerParametroLiquidacionXTercero(terceroId);
            return Ok(datos);
        }
    }
}