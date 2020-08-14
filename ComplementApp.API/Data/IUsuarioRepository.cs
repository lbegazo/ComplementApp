using System.Threading.Tasks;
using ComplementApp.API.Helpers;
using ComplementApp.API.Models;

namespace ComplementApp.API.Data
{
    public interface IUsuarioRepository
    {
        Task<Usuario> Register(Usuario user, string password);
        
        Task<PagedList<Usuario>> ObtenerUsuarios(UserParams userParams);

        Task<Usuario> ObtenerUsuario(int id);       

        Task<bool> UserExists(string username);

        Task<bool> EliminarUsuario(int id);
    }
}