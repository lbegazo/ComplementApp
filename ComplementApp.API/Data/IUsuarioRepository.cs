using System.Collections.Generic;
using System.Threading.Tasks;
using ComplementApp.API.Models;

namespace ComplementApp.API.Data
{
    public interface IUsuarioRepository
    {
        Task<IEnumerable<Usuario>> ObtenerUsuarios();

        Task<Usuario> ObtenerUsuario(int id);
    }
}