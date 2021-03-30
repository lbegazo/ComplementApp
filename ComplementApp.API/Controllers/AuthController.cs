using System.Threading.Tasks;
using ComplementApp.API.Data;
using ComplementApp.API.Dtos;
using ComplementApp.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System;
using System.IdentityModel.Tokens.Jwt;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ComplementApp.API.Interfaces;

namespace ComplementApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _repo;
        private readonly IConfiguration _config;
        private readonly IMapper _mapper;

        private readonly DataContext _dataContext;
        public AuthController(IAuthRepository repo, IConfiguration config, IMapper mapper, DataContext dataContext)
        {
            _mapper = mapper;
            _config = config;
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


            /**************************Create the token************************************/
            //var role = userFromRepo.EsAdministrador ? 1 : 0;

            //The token contains 3 claims: userId and username, Administrator
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userFromRepo.UsuarioId.ToString()),
                new Claim(ClaimTypes.Name, userFromRepo.Username),
                new Claim(ClaimTypes.Role, userFromRepo.PciId.ToString()),
                new Claim(ClaimTypes.Surname, userFromRepo.Nombres+ ' '+ userFromRepo.Apellidos)
            };

            //The server has to sign the token(To have a valid token)
            var key = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(
                                _config.GetSection("AppSettings:Token").Value));

            //we are encryting the key(in bytes) using HmacSha512Signature           
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            //Create the token's features
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(60),
                SigningCredentials = creds
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            //Create the token using the previous variables
            var token = tokenHandler.CreateToken(tokenDescriptor);
            //Return the token to the clients
            return Ok(new { token = tokenHandler.WriteToken(token) });

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