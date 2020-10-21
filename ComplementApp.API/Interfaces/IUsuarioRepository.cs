using System.Collections.Generic;
using System.Threading.Tasks;
using ComplementApp.API.Helpers;
using ComplementApp.API.Models;

namespace ComplementApp.API.Interfaces
{
    public interface IUsuarioRepository
    {
        Task<Usuario> Register(Usuario user, string password);
        
        Task<PagedList<Usuario>> ObtenerUsuarios(UserParams userParams);

        Task<Usuario> ObtenerUsuarioBase(int id);       

        Task<bool> UserExists(string username);

        Task<bool> EliminarUsuario(int id);

        Task<ICollection<Transaccion>> ObtenerListaTransaccionXUsuario(int usuarioId);

        Task<ICollection<Perfil>> ObtenerPerfilesxUsuario(int usuarioId);

        bool RegistrarPerfilesAUsuario(int usuarioId, ICollection<Perfil> listaPerfiles);

        Task<Transaccion> ObtenerTransaccionXCodigo(string codigoTransaccion);

        bool EliminarPerfilesUsuario(int usuarioId);
    }
}