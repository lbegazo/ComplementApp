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

namespace ComplementApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _repo;
        private readonly IConfiguration _config;
        public AuthController(IAuthRepository repo, IConfiguration config)
        {
            _config = config;
            _repo = repo;
        }

        [HttpPost("Register")]
        public async Task<IActionResult> register(UserForRegisterDto userForRegisterDto)
        {
            userForRegisterDto.UserName= userForRegisterDto.UserName.ToLower();

            if(await _repo.UserExists(userForRegisterDto.UserName))
                return BadRequest("Username already exists");
            
            var userToCreate = new User{ Username=userForRegisterDto.UserName};

            var createdUser= await _repo.Register(userToCreate, userForRegisterDto.Password);

            return StatusCode(201);            
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login(UserForLoginDto userForLoginDto)
        {
            var userFromRepo =await _repo.Login(userForLoginDto.Username.ToLower(), userForLoginDto.Password);

            //If the user has the token, the application does not need to go to the database

            if(userFromRepo==null)
                return BadRequest("Username does not exists");

            /**************************Create the token************************************/

            //The token contains 2 claims: userId and username
            var claims=new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userFromRepo.Id.ToString()),
                new Claim(ClaimTypes.Name, userFromRepo.Username)
            };

            //The server has to sign the token(To have a valid token)
            var key = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(
                                _config.GetSection("AppSettings:Token").Value));

            //we are encryting the key(in bytes) using HmacSha512Signature           
            var creds= new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            //Create the token's features
            var tokenDescriptor= new SecurityTokenDescriptor
            {
                Subject=new ClaimsIdentity(claims),
                Expires= DateTime.Now.AddDays(1),
                SigningCredentials=creds
            };

            var tokenHandler= new JwtSecurityTokenHandler();
            //Create the token using the previous variables
            var token= tokenHandler.CreateToken(tokenDescriptor);
            //Return the token to the clients
            return Ok(new { token= tokenHandler.WriteToken(token)});
        }
       
    }
}