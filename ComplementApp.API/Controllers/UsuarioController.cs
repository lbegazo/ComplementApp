using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using ComplementApp.API.Data;
using ComplementApp.API.Dtos;
using ComplementApp.API.Helpers;
using ComplementApp.API.Interfaces;
using ComplementApp.API.Interfaces.Repository;
using ComplementApp.API.Models;
using Microsoft.AspNetCore.Mvc;

namespace ComplementApp.API.Controllers
{
    
    public class UsuarioController : BaseApiController
    {
        #region Variable
        int pciId = 0;
        string valorPciId = string.Empty;
        
        #endregion 

        #region Dependency injection

        private readonly IUsuarioRepository _repo;
        private readonly ITransaccionRepository _transaccionRepository;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly DataContext _dataContext;

        #endregion Dependency injection
        public UsuarioController(IUnitOfWork unitOfWork, IUsuarioRepository repo,
                                ITransaccionRepository transaccionRepository,
                                IMapper mapper, DataContext dataContext)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _repo = repo;
            _dataContext = dataContext;
            _transaccionRepository = transaccionRepository;
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
            var user = await _repo.ObtenerUsuarioBase(id);
            var perfiles = await _transaccionRepository.ObtenerPerfilesxUsuario(id);
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
            var transacciones = await _transaccionRepository.ObtenerListaTransaccionXUsuario(idUsuarioLogueado);
            return Ok(transacciones);
        }

        [Route("[action]/{codigoTransaccion}")]
        [HttpGet]
        public async Task<IActionResult> ObtenerTransaccionXCodigo(string codigoTransaccion)
        {
            var transacciones = await _transaccionRepository.ObtenerTransaccionXCodigo(codigoTransaccion);
            return Ok(transacciones);
        }

        [HttpPost]
        public async Task<IActionResult> RegistrarUsuario(UsuarioParaRegistrarDto userForRegisterDto)
        {
            await using var transaction = await _dataContext.Database.BeginTransactionAsync();
            // var idUsuarioLogueado = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            try
            {
                userForRegisterDto.UserName = userForRegisterDto.UserName.ToLower();

                if (await _repo.UserExists(userForRegisterDto.UserName))
                    return BadRequest("El usuario ya existe");

                var userToCreate = _mapper.Map<Usuario>(userForRegisterDto);

                var createdUser = await _repo.Register(userToCreate, userForRegisterDto.Password);
                await _dataContext.SaveChangesAsync();

                _transaccionRepository.RegistrarPerfilesAUsuario(createdUser.UsuarioId, userForRegisterDto.Perfiles);
                await _dataContext.SaveChangesAsync();

                await transaction.CommitAsync();

                //Esta linea es para evitar retornar User, porque contiene el password
                var userToReturn = _mapper.Map<UsuarioParaDetalleDto>(createdUser);

                return CreatedAtRoute("GetUser", new { Controller = "Users", id = createdUser.UsuarioId }, userToReturn);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> ActualizarUsuario(int id, UsuarioParaActualizar userForUpdateDto)
        {
            await using var transaction = await _dataContext.Database.BeginTransactionAsync();

            try
            {
                _transaccionRepository.EliminarPerfilesUsuario(id);
                await _dataContext.SaveChangesAsync();

                _transaccionRepository.RegistrarPerfilesAUsuario(id, userForUpdateDto.Perfiles);
                await _dataContext.SaveChangesAsync();

                var userFromRepo = await _repo.ObtenerUsuarioBase(id);
                _mapper.Map(userForUpdateDto, userFromRepo);
                await _dataContext.SaveChangesAsync();

                await transaction.CommitAsync();

                return NoContent();
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> EliminarUsuario(int id)
        {
            var idUsuarioLogueado = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var usuarioLogueado = await _repo.ObtenerUsuarioBase(idUsuarioLogueado);

            await using var transaction = await _dataContext.Database.BeginTransactionAsync();

            if (idUsuarioLogueado == id)
            {
                return BadRequest("No se puede eliminar el usuario desde la misma cuenta de usuario");
            }

            try
            {
                _transaccionRepository.EliminarPerfilesUsuario(id);
                _dataContext.SaveChanges();

                await _repo.EliminarUsuario(id);
                await _dataContext.SaveChangesAsync();

                await transaction.CommitAsync();

                return NoContent();
            }
            catch (Exception)
            {
                throw;
            }
        }

        [Route("[action]/{perfilId}")]
        [HttpGet]
        public async Task<IActionResult> ObtenerListaUsuarioXPerfil(int perfilId)
        {
            valorPciId = User.FindFirst(ClaimTypes.Role).Value;
            if (!string.IsNullOrEmpty(valorPciId))
            {
                pciId = int.Parse(valorPciId);
            }
            var lista = await _repo.ObtenerListaUsuarioXPerfil(perfilId, pciId);
            return Ok(lista);
        }

    }
}