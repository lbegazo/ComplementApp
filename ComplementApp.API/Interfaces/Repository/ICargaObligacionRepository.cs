using System.Collections.Generic;
using ComplementApp.API.Models;

namespace ComplementApp.API.Interfaces.Repository
{
    public interface ICargaObligacionRepository
    {
        bool InsertarListaCargaObligacion(IList<CargaObligacion> listaCdp);

        bool EliminarCargaObligacion();
    }
}