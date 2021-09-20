using System.Threading.Tasks;
using ComplementApp.API.Data;
using ComplementApp.API.Dtos;
using ComplementApp.API.Models;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ComplementApp.API.Interfaces;
using ComplementApp.API.Interfaces.Repository;

namespace ComplementApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _repo;
        private readonly IMapper _mapper;
        private readonly DataContext _dataContext;
        private readonly ITokenService _token;
        public AuthController(IAuthRepository repo, IMapper mapper, DataContext dataContext, ITokenService token)
        {
            _token = token;
            _mapper = mapper;
            _repo = repo;
            _dataContext = dataContext;
        }

        [HttpPost("Register")]
        public async Task<IActionResult> register(UsuarioParaRegistrarDto userForRegisterDto)
        {
            userForRegisterDto.UserName = userForRegisterDto.UserName.ToLower();

            if (await _repo.UserExists(userForRegisterDto.UserName))
                return BadRequest("El usuario ya existe");

            //var userToCreate = new User { Username = userForRegisterDto.UserName };
            //The destination is userForRegisterDto
            var userToCreate = _mapper.Map<Usuario>(userForRegisterDto);

            var createdUser = await _repo.Register(userToCreate, userForRegisterDto.Password);
            //Esta linea es para evitar retornar User, porque contiene el password
            var userToReturn = _mapper.Map<UsuarioParaDetalleDto>(createdUser);

            return CreatedAtRoute("GetUser", new { Controller = "Users", id = createdUser.UsuarioId }, userToReturn);
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login(UserForLoginDto userForLoginDto)
        {
            var userFromRepo = await _repo.Login(userForLoginDto.Username.ToLower(), userForLoginDto.Password);

            //If the user has the token, the application does not need to go to the database

            if (userFromRepo == null)
            {
                if (await _repo.UserExists(userForLoginDto.Username.ToLower()))
                    return BadRequest("La clave no es la correcta");
                else
                    return BadRequest("El usuario no existe");
            }

            var result = await _repo.ValidateDate();

            if (!result)
                return BadRequest("El sistema no se encuentra activo, por favor comunicarse con el administrador del sistema");

            string token = _token.CreateToken(userFromRepo);

            //return Ok(new { token = tokenHandler.WriteToken(token) });
            return Ok(new { token });
        }

        [HttpPost("CargarDatosIniciales")]
        ///
        ///Este método realiza la carga inicial de datos la aplicación
        ///
        public IActionResult CargarDatosIniciales()
        {
            _dataContext.Database.Migrate();
            Seed.CargarDataInicial(_dataContext);
            return Ok();
        }
    }
}