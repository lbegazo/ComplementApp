using ComplementApp.API.Dtos;
using ComplementApp.API.Models;

namespace ComplementApp.API.Interfaces
{
    public interface ITokenService
    {
        string CreateToken(Usuario userFromRepo);
    }
}