using System.Threading.Tasks;
using ComplementApp.API.Models;

namespace ComplementApp.API.Data
{
    public interface IAuthRepository
    {
         Task<Usuario> Register(Usuario user, string password);
         Task<Usuario> Login(string username, string password);
         Task<bool> UserExists(string username);
    }
}