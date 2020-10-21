using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using ComplementApp.API.Data;
using ComplementApp.API.Helpers;
using ComplementApp.API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ComplementApp.API.Controllers
{
    [ServiceFilter(typeof(LogActividadUsuario))]
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class CDPController : ControllerBase
    {
        private readonly ICDPRepository _repo;
        private readonly IMapper _mapper;
        private readonly IUsuarioRepository _usuarioRepo;

        public IUsuarioRepository UsuarioRepo => _usuarioRepo;

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
            var usuarioId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);            

            var cdps = await _repo.ObtenerListaCDP(usuarioId);
            return base.Ok(cdps);
        }

        [Route("[action]/{numeroCDP}")]
        [HttpGet]
        public async Task<IActionResult> ObtenerCDP(int numeroCDP)
        {
            var usuarioId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);     

            var cdps = await _repo.ObtenerCDP(usuarioId,numeroCDP);
            return base.Ok(cdps);
        }

        [Route("[action]/{numeroCDP}")]
        [HttpGet]
        public async Task<IActionResult> ObtenerDetalleDeCDP(int numeroCDP)
        {
            var usuarioId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);  

            var CDPs = await _repo.ObtenerDetalleDeCDP(usuarioId, numeroCDP);
            return Ok(CDPs);
        }        

    }
}