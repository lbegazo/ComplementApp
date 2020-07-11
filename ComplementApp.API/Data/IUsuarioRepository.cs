using System.Collections.Generic;
using System.Threading.Tasks;
using ComplementApp.API.Models;

namespace ComplementApp.API.Data
{
    public interface IUsuarioRepository
    {
        Task<Usuario> Register(Usuario user, string password);
        Task<IEnumerable<Usuario>> ObtenerUsuarios();

        Task<Usuario> ObtenerUsuario(int id);

        Task<IEnumerable<Cargo>> ObtenerCargos();

        Task<IEnumerable<Area>> ObtenerAreas();

        Task<bool> UserExists(string username);
    }
}