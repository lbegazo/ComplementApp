using System.Collections.Generic;
using System.Threading.Tasks;
using ComplementApp.API.Dtos;
using ComplementApp.API.Helpers;
using ComplementApp.API.Models;

namespace ComplementApp.API.Interfaces.Repository
{
    public interface IUsuarioRepository
    {
        Task<Usuario> Register(Usuario user, string password);
        
        Task<PagedList<Usuario>> ObtenerUsuarios(UserParams userParams);

        Task<Usuario> ObtenerUsuarioBase(int id);       

        Task<bool> UserExists(string username);

        Task<bool> EliminarUsuario(int id);

        Task<ICollection<ValorSeleccion>> ObtenerListaUsuarioXPerfil(int perfilId, int pci);
    }
}