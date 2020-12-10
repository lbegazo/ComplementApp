using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using ComplementApp.API.Dtos;
using ComplementApp.API.Interfaces;
using ComplementApp.API.Models;
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
        public async Task<IActionResult> ObtenerParametroGeneralXNombre([FromQuery(Name = "nombre")] string nombre)
        {
            var datos = await _repo.ObtenerParametroGeneralXNombre(nombre);
            return Ok(datos);
        }

        [Route("[action]")]
        [HttpGet]
        public async Task<IActionResult> ObtenerParametrosGeneralesXTipo([FromQuery(Name = "tipo")] string tipo)
        {
            var datos = await _repo.ObtenerParametrosGeneralesXTipo(tipo);
            return Ok(datos);
        }


        [Route("[action]")]
        [HttpGet]
        public async Task<IActionResult> ObtenerParametroLiquidacionXTercero([FromQuery(Name = "terceroId")] int terceroId)
        {
            var datos = await _repo.ObtenerParametroLiquidacionXTercero(terceroId);
            return Ok(datos);
        }

        [Route("[action]")]
        [HttpGet]
        public async Task<IActionResult> ObtenerListaUsuarioxFiltro([FromQuery(Name = "nombres")] string nombres)
        {
            var datos = await _repo.ObtenerListaUsuarioxFiltro(nombres);
            return Ok(datos);
        }

        [Route("[action]")]
        [HttpGet]
        public async Task<IActionResult> ObtenerListaEstadoSolicitudCDP([FromQuery(Name = "tipoDocumento")] string tipoDocumento)
        {
            var lista = await _repo.ObtenerListaEstado(tipoDocumento);
            return Ok(lista);
        }

        [Route("[action]")]
        [HttpGet]
        public IActionResult ObtenerListaMeses()
        {
            var months = Enumerable.Range(1, 12).Select(i => new { Id = i, Nombre = UppercaseFirst(DateTimeFormatInfo.CurrentInfo.GetMonthName(i)) });
            return Ok(months);
        }

        [Route("[action]")]
        [HttpGet]
        public async Task<IActionResult> ObtenerListaUsoPresupuestalXRubro([FromQuery(Name = "rubroPresupuestalId")] int rubroPresupuestalId)
        {
            var lista = await _repo.ObtenerListaUsoPresupuestalXRubro(rubroPresupuestalId);
            return Ok(lista);
        }

        [Route("[action]")]
        [HttpGet]
        public async Task<IActionResult> ObtenerListaXTipo([FromQuery(Name = "listaId")] int listaId)
        {
            TipoLista tipoLista = (TipoLista)listaId;
            var lista = await _repo.ObtenerListaXTipo(tipoLista);
            return Ok(lista);
        }

        static string UppercaseFirst(string s)
        {
            // Check for empty string.
            if (string.IsNullOrEmpty(s))
            {
                return string.Empty;
            }
            // Return char and concat substring.
            return char.ToUpper(s[0]) + s.Substring(1);
        }
    }
}