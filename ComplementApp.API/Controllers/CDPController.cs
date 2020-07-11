using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using ComplementApp.API.Data;
using ComplementApp.API.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ComplementApp.API.Controllers
{
    // [ServiceFilter(typeof(LogUserActivity))]
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class CDPController : ControllerBase
    {
        private readonly ICDPRepository _repo;
        private readonly IMapper _mapper;
        private readonly IUsuarioRepository _usuarioRepo;
        public CDPController(ICDPRepository repo, IUsuarioRepository usuarioRepo, IMapper mapper)
        {
            _usuarioRepo = usuarioRepo;
            _mapper = mapper;
            _repo = repo;
        }



        [Route("[action]")]
        [HttpGet]
        public async Task<IActionResult> ObtenerListaCDP()
        {
            string nombreCompleto = string.Empty;
            nombreCompleto = await ObtenerNombreCompletoUsuario();

            var cdps = await _repo.ObtenerListaCDP(nombreCompleto);
            return base.Ok(cdps);
        }

        [Route("[action]/{numeroCDP}")]
        [HttpGet]
        public async Task<IActionResult> ObtenerCDP(int numeroCDP)
        {
            string nombreCompleto = string.Empty;
            nombreCompleto = await ObtenerNombreCompletoUsuario();

            var cdps = await _repo.ObtenerCDP(nombreCompleto,numeroCDP);
            return base.Ok(cdps);
        }



        [Route("[action]/{numeroCDP}")]
        [HttpGet]
        public async Task<IActionResult> ObtenerDetalleDeCDP(int numeroCDP)
        {
            string nombreCompleto = string.Empty;
            nombreCompleto = await ObtenerNombreCompletoUsuario();

            if (string.IsNullOrEmpty(nombreCompleto))
            {
                return NotFound();
            }
            var CDPs = await _repo.ObtenerDetalleDeCDP(nombreCompleto, numeroCDP);
            return Ok(CDPs);
        }


        private async Task<string> ObtenerNombreCompletoUsuario()
        {
            string nombreCompleto = string.Empty;
            var idUsuario = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var usuario = await _usuarioRepo.ObtenerUsuario(idUsuario);

            if (usuario != null)
            {
                nombreCompleto = usuario.Nombres + " " + usuario.Apellidos;
            }
            return nombreCompleto;
        }

    }
}