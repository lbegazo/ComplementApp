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
        public async Task<IActionResult> ObtenerUsuarios([FromQuery] UserParams userParams)
        {
            var pagedList = await _repo.ObtenerUsuarios(userParams);
            var usersForList = _mapper.Map<IEnumerable<UsuarioParaDetalleDto>>(pagedList);

            Response.AddPagination(pagedList.CurrentPage, pagedList.PageSize,
                                pagedList.TotalCount, pagedList.TotalPages);

            return Ok(usersForList);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> ObtenerUsuario(int id)
        {
            var user = await _repo.ObtenerUsuario(id);
            var perfiles = await _repo.ObtenerPerfilesxUsuario(id);
            var userForDetailed = _mapper.Map<UsuarioParaDetalleDto>(user);

            userForDetailed.Perfiles = new List<Perfil>();
            foreach (var item in perfiles)
            {
                userForDetailed.Perfiles.Add(item);
            }

            return Ok(userForDetailed);
        }

        [Route("[action]/{idUsuarioLogueado}")]
        [HttpGet]
        public async Task<IActionResult> ObtenerListaTransaccionXUsuario(int idUsuarioLogueado)
        {
            var transacciones = await _repo.ObtenerListaTransaccionXUsuario(idUsuarioLogueado);
            //var listaDto = _mapper.Map<IEnumerable<TransaccionDto>>(transacciones);
            return Ok(transacciones);
        }

        [Route("[action]/{codigoTransaccion}")]
        [HttpGet]
        public async Task<IActionResult> ObtenerTransaccionXCodigo(string codigoTransaccion)
        {
            var transacciones = await _repo.ObtenerTransaccionXCodigo(codigoTransaccion);
            return Ok(transacciones);
        }

        [HttpPost]
        public async Task<IActionResult> RegistrarUsuario(UsuarioParaRegistrarDto userForRegisterDto)
        {
            var idUsuarioLogueado = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var usuarioLogueado = await _repo.ObtenerUsuario(idUsuarioLogueado);

            userForRegisterDto.UserName = userForRegisterDto.UserName.ToLower();

            if (await _repo.UserExists(userForRegisterDto.UserName))
                return BadRequest("El usuario ya existe");

            var userToCreate = _mapper.Map<Usuario>(userForRegisterDto);

            var createdUser = await _repo.Register(userToCreate, userForRegisterDto.Password);

            _repo.RegistrarPerfilesAUsuario(createdUser.UsuarioId, userForRegisterDto.Perfiles);

            //Esta linea es para evitar retornar User, porque contiene el password
            var userToReturn = _mapper.Map<UsuarioParaDetalleDto>(createdUser);

            return CreatedAtRoute("GetUser", new { Controller = "Users", id = createdUser.UsuarioId }, userToReturn);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> ActualizarUsuario(int id, UsuarioParaActualizar userForUpdateDto)
        {
            // Para que el usuario actualice sus propios datos
            // if (id != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            //     return Unauthorized();

            _repo.RegistrarPerfilesAUsuario(id, userForUpdateDto.Perfiles);

            // var idUsuarioLogueado = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            // var usuarioLogueado = await _repo.ObtenerUsuario(idUsuarioLogueado);

            var userFromRepo = await _repo.ObtenerUsuario(id);

            _mapper.Map(userForUpdateDto, userFromRepo);

            await _unitOfWork.CompleteAsync();
            return NoContent();

            throw new Exception($"Actualizando el usuario {id} el proceso falló");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> EliminarUsuario(int id)
        {
            var idUsuarioLogueado = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var usuarioLogueado = await _repo.ObtenerUsuario(idUsuarioLogueado);

            // if (!usuarioLogueado.EsAdministrador)
            //     return Unauthorized();

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