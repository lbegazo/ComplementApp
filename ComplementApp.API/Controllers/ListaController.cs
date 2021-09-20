using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using ComplementApp.API.Dtos;
using ComplementApp.API.Helpers;
using ComplementApp.API.Interfaces.Repository;
using ComplementApp.API.Models;
using Microsoft.AspNetCore.Mvc;

namespace ComplementApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ListaController : ControllerBase
    {
        #region Variable
        int pciId = 0;
        string valorPciId = string.Empty;

        #endregion 

        private readonly IListaRepository _repo;
        private readonly IMapper _mapper;

        public ListaController(IListaRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
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
        public async Task<IActionResult> ObtenerListaTercero([FromQuery(Name = "numeroIdentificacion")] string numeroIdentificacion,
                                                             [FromQuery(Name = "nombre")] string nombre)
        {
            var datos = await _repo.ObtenerListaTercero(numeroIdentificacion, nombre);
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
        public async Task<IActionResult> ObtenerListaUsuarioxFiltro([FromQuery(Name = "nombres")] string nombres,
                                                                    [FromQuery(Name = "apellidos")] string apellidos)
        {
            valorPciId = User.FindFirst(ClaimTypes.Role).Value;
            if (!string.IsNullOrEmpty(valorPciId))
            {
                pciId = int.Parse(valorPciId);
            }

            var datos = await _repo.ObtenerListaUsuarioxFiltro(pciId, nombres, apellidos);
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

        [Route("[action]")]
        [HttpGet]
        public async Task<IActionResult> ObtenerListaXTipoyPci([FromQuery(Name = "listaId")] int listaId)
        {
            valorPciId = User.FindFirst(ClaimTypes.Role).Value;
            if (!string.IsNullOrEmpty(valorPciId))
            {
                pciId = int.Parse(valorPciId);
            }

            TipoLista tipoLista = (TipoLista)listaId;
            var lista = await _repo.ObtenerListaXTipoyPci(pciId, tipoLista);
            return Ok(lista);
        }

        [Route("[action]")]
        [HttpGet]
        public async Task<IActionResult> ObtenerListaDeducciones([FromQuery(Name = "codigo")] string codigo, [FromQuery(Name = "nombre")] string nombre)
        {
            var datos = await _repo.ObtenerListaDeducciones(codigo, nombre);
            return Ok(datos);
        }

        [Route("[action]")]
        [HttpGet]
        public async Task<IActionResult> ObtenerListaActividadesEconomicas([FromQuery(Name = "codigo")] string codigo)
        {
            var datos = await _repo.ObtenerListaActividadesEconomicas(codigo);
            return Ok(datos);
        }

        [Route("[action]")]
        [HttpGet]
        public IActionResult ObtenerListaSIoNO()
        {
            List<ValorSeleccion> lista = new List<ValorSeleccion>();

            lista.Add(new ValorSeleccion() { Id = 0, Nombre = "NO" });
            lista.Add(new ValorSeleccion() { Id = 1, Nombre = "SI" });
            return Ok(lista);
        }

        [Route("[action]")]
        [HttpGet]
        public IActionResult ObtenerListaTipoArchivo()
        {
            List<ValorSeleccion> lista = new List<ValorSeleccion>();

            lista.Add(new ValorSeleccion() { Id = 1, Nombre = "Cuenta por Pagar" });
            lista.Add(new ValorSeleccion() { Id = 2, Nombre = "Obligación" });
            return Ok(lista);
        }

        [Route("[action]")]
        [HttpGet]
        public async Task<IActionResult> ObtenerListaRubroPresupuestalPorPapa([FromQuery] int rubroPresupuestalId, [FromQuery] UserParams userParams)
        {
            var pagedList = await _repo.ObtenerListaRubroPresupuestalPorPapa(rubroPresupuestalId, userParams);
            var listaDto = _mapper.Map<IEnumerable<RubroPresupuestal>>(pagedList);

            Response.AddPagination(pagedList.CurrentPage, pagedList.PageSize,
                                   pagedList.TotalCount, pagedList.TotalPages);

            return base.Ok(listaDto);
        }

        [Route("[action]")]
        [HttpGet]
        public async Task<IActionResult> ObtenerListaRubroPresupuestal([FromQuery(Name = "identificacion")] string identificacion,
                                                                       [FromQuery(Name = "nombre")] string nombre)
        {
            var datos = await _repo.ObtenerListaRubroPresupuestal(identificacion, nombre);
            return Ok(datos);
        }

        [Route("[action]")]
        [HttpGet]
        public async Task<IActionResult> ObtenerListaSolicitudCDP([FromQuery(Name = "numeroSolicitud")] string numeroSolicitud)
        {
            var datos = await _repo.ObtenerListaSolicitudCDP(numeroSolicitud);
            return Ok(datos);
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