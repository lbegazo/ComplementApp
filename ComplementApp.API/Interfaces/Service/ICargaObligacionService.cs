using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using ComplementApp.API.Models;
using Microsoft.AspNetCore.Http;

namespace ComplementApp.API.Interfaces.Service
{
    public interface ICargaObligacionService
    {
        bool EliminarCargaObligacion();

        DataTable ObtenerInformacionDeExcel(IFormFile file);

        List<CargaObligacion> ObtenerListaCargaObligacion(DataTable dt);
    }
}