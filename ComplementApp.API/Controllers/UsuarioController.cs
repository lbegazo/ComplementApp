using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using ComplementApp.API.Data;
using ComplementApp.API.Dtos;
using ComplementApp.API.Helpers;
using ComplementApp.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ComplementApp.API.Controllers
{
    [ServiceFilter(typeof(LogActividadUsuario))]
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

        [HttpPost]
        public async Task<IActionResult> RegistrarUsuario(UsuarioParaRegistrarDto userForRegisterDto)
        {
            var idUsuarioLogueado = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var usuarioLogueado = await _repo.ObtenerUsuario(idUsuarioLogueado);

            if (!usuarioLogueado.EsAdministrador)
                return Unauthorized();

            userForRegisterDto.UserName = userForRegisterDto.UserName.ToLower();

            if (await _repo.UserExists(userForRegisterDto.UserName))
                return BadRequest("El usuario ya existe");

            var userToCreate = _mapper.Map<Usuario>(userForRegisterDto);

            var createdUser = await _repo.Register(userToCreate, userForRegisterDto.Password);
            //Esta linea es para evitar retornar User, porque contiene el password
            var userToReturn = _mapper.Map<UsuarioParaDetalleDto>(createdUser);

            return CreatedAtRoute("GetUser", new { Controller = "Users", id = createdUser.Id }, userToReturn);

        }

        [HttpPut("{id}")]
        public async Task<IActionResult> ActualizarUsuario(int id, UsuarioParaActualizar userForUpdateDto)
        {
            // Para que el usuario actualice sus propios datos
            // if (id != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            //     return Unauthorized();

            var idUsuarioLogueado = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var usuarioLogueado = await _repo.ObtenerUsuario(idUsuarioLogueado);

            if (!usuarioLogueado.EsAdministrador)
                return Unauthorized();

            var userFromRepo = await _repo.ObtenerUsuario(id);

            _mapper.Map(userForUpdateDto, userFromRepo);

            await _unitOfWork.CompleteAsync();
            return NoContent();

            //throw new Exception($"Actualizando el usuario {id} el proceso falló");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> EliminarUsuario(int id)
        {
            var idUsuarioLogueado = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var usuarioLogueado = await _repo.ObtenerUsuario(idUsuarioLogueado);

            if (!usuarioLogueado.EsAdministrador)
                return Unauthorized();

            if (idUsuarioLogueado == id)
            {
                return BadRequest("No se puede eliminar el usuario desde la misma cuenta de usuario");
            }

            if (await _repo.EliminarUsuario(id))
            {
                return NoContent();
            }

            throw new Exception($"No se pudo eliminar el usuario: {id}");
        }
    }
}