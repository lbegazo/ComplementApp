using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ComplementApp.API.Interfaces;
using ComplementApp.API.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace ComplementApp.API.Services
{
    public class TokenService : ITokenService
    {
        private readonly SymmetricSecurityKey _key;
        //private readonly string _environment;

        public TokenService(IConfiguration config)
        {
            //The server has to sign the token(To have a valid token)
            _key = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(
                                config["AppSettings:Token"]));
            //_environment = config["AppSettings:environment"];
        }

        public string CreateToken(Usuario userFromRepo)
        {
            /**************************Create the token************************************/
            //The token contains claims
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userFromRepo.UsuarioId.ToString()),
                new Claim(ClaimTypes.Name, userFromRepo.Username),
                new Claim(ClaimTypes.Role, userFromRepo.PciId.ToString()),
                new Claim(ClaimTypes.Surname, userFromRepo.Nombres+ ' '+ userFromRepo.Apellidos),
                new Claim(ClaimTypes.GivenName,  userFromRepo.Nombres)
                //new Claim(ClaimTypes.Email,  _environment),
            };

            //we are encryting the key(in bytes) using HmacSha512Signature           
            var creds = new SigningCredentials(_key, SecurityAlgorithms.HmacSha512Signature);

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

            return tokenHandler.WriteToken(token);
        }
    }
}