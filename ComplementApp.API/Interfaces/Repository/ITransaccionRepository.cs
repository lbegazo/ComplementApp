using System.Collections.Generic;
using System.Threading.Tasks;
using ComplementApp.API.Models;

namespace ComplementApp.API.Interfaces.Repository
{
    public interface ITransaccionRepository
    {
         Task<ICollection<Transaccion>> ObtenerListaTransaccionXUsuario(int usuarioId);

        Task<ICollection<Perfil>> ObtenerPerfilesxUsuario(int usuarioId);

        bool RegistrarPerfilesAUsuario(int usuarioId, ICollection<Perfil> listaPerfiles);

        bool EliminarPerfilesUsuario(int usuarioId);

        bool EliminarTransaccionesXPerfil(int perfilId);

        bool InsertarTransaccionesXPerfil(int perfilId, List<int> listaTransaccion);

        Task<Transaccion> ObtenerTransaccionXCodigo(string codigoTransaccion);

    }
}