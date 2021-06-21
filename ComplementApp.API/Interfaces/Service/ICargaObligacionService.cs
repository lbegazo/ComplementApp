using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using ComplementApp.API.Dtos;
using ComplementApp.API.Models;
using Microsoft.AspNetCore.Http;

namespace ComplementApp.API.Interfaces.Service
{
    public interface ICargaObligacionService
    {
        bool EliminarCargaObligacion();

        DataTable ObtenerInformacionDeExcel(IFormFile file);

        Task<List<CargaObligacion>> ObtenerListaCargaObligacion(int pciId, DataTable dt);
        string ObtenerInformacionOrdenPagoArchivoCabecera(List<CargaObligacionDto> lista);
        string ObtenerInformacionOrdenPagoArchivoDetalle(List<CargaObligacionDto> listaTotal);
    }
}