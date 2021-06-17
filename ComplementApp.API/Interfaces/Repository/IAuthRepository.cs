using System.Threading.Tasks;
using ComplementApp.API.Models;

namespace ComplementApp.API.Interfaces.Repository
{
    public interface IAuthRepository
    {
         Task<Usuario> Register(Usuario user, string password);
         Task<Usuario> Login(string username, string password);
         Task<bool> UserExists(string username);
         Task<bool> ValidateDate();
    }
}