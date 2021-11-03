using System.Collections.Generic;
using System.Threading.Tasks;
using ComplementApp.API.Dtos;
using ComplementApp.API.Models.ExcelDocumento;

namespace ComplementApp.API.Interfaces.Service
{
    public interface IDocumentoService
    {
        Task CargarInformacionCDP(List<CDPDto> listaDocumento);
         Task CargaDocumentoGestionPresupuestal(int pciId, int tipoArchivo, List<DocumentoCdp> listaCdp, List<DocumentoCompromiso> listaCompromiso, List<DocumentoObligacion> listaObligacion, List<DocumentoOrdenPago> listaOrdenPago);
    }
}