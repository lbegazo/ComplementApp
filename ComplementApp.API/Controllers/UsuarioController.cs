using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using ComplementApp.API.Data;
using ComplementApp.API.Dtos;
using ComplementApp.API.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ComplementApp.API.Controllers
{
    //[ServiceFilter(typeof(LogActividadUsuario))]
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UsuarioController : ControllerBase
    {
        private readonly IUsuarioRepository _repo;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        public UsuarioController(IUnitOfWork unitOfWork, IUsuarioRepository repo, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _repo = repo;
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerUsuarios()
        {
            var usuarios = await _repo.ObtenerUsuarios();
            var usersForList = _mapper.Map<IEnumerable<UsuarioParaDetalleDto>>(usuarios);
            return Ok(usersForList);
        }

        [HttpGet("{id}", Name = "ObtenerUsuario")]
        public async Task<IActionResult> ObtenerUsuario(int id)
        {
            var user = await _repo.ObtenerUsuario(id);
            var userForDetailed = _mapper.Map<UsuarioParaDetalleDto>(user);
            return Ok(userForDetailed);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> ActualizarUsuario(int id, UsuarioParaActualizar userForUpdateDto)
        {

            if (id != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

            var userFromRepo = await _repo.ObtenerUsuario(id);

            _mapper.Map(userForUpdateDto, userFromRepo);

            if (await _unitOfWork.CompleteAsync())
                return NoContent();

            throw new Exception($"Falló al actualizar el usuario {id} ");

        }

    }
}