using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ComplementApp.API.Data;
using ComplementApp.API.Dtos;
using ComplementApp.API.Interfaces.Repository;
using ComplementApp.API.Interfaces.Service;
using ComplementApp.API.Models.ExcelDocumento;

namespace ComplementApp.API.Services
{
    public class DocumentoService : IDocumentoService
    {
        #region Variable
        int constanteLote = 7500;

        #endregion 

        #region Dependency Injection

        private readonly DataContext _dataContext;
        private readonly IDocumentoRepository _repo;
        private readonly ICDPRepository _cdpRepo;
        private readonly IProcesoDocumentoExcel _excelDocumento;

        #endregion Dependency Injection

        public DocumentoService(DataContext dataContext, IDocumentoRepository repo, ICDPRepository cdpRepo, IProcesoDocumentoExcel excelDocumento)
        {
            _dataContext = dataContext;
            _repo = repo;
            _cdpRepo = cdpRepo;
            _excelDocumento = excelDocumento;
        }

        public async Task CargarInformacionCDP(List<CDPDto> listaDocumento)
        {
            using var transaction = await _dataContext.Database.BeginTransactionAsync();

            try
            {
                _excelDocumento.EliminarInformacionCDP();
                await _dataContext.SaveChangesAsync();

                #region Insertar lista en la base de datos

                if (listaDocumento.Count > constanteLote)
                {
                    for (int i = 0; i < listaDocumento.Count; i += constanteLote)
                    {
                        List<CDPDto> lista = (List<CDPDto>)listaDocumento.Skip(i).Take(constanteLote).ToList();
                        _repo.InsertaCabeceraCDP(lista);
                        await _dataContext.SaveChangesAsync();
                    }
                }
                else
                {
                    _repo.InsertaCabeceraCDP(listaDocumento);
                    await _dataContext.SaveChangesAsync();
                }

                #endregion Insertar lista en la base de datos

                transaction.Commit();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task CargaDocumentoGestionPresupuestal(int pciId, int tipoArchivo, List<DocumentoCdp> listaCdp, List<DocumentoCompromiso> listaCompromiso, List<DocumentoObligacion> listaObligacion, List<DocumentoOrdenPago> listaOrdenPago)
        {
            try
            {
                using var transaction = await _dataContext.Database.BeginTransactionAsync();

                #region Eliminar Documento CDP, Compromiso, Obligacion, OrdenPago

                if (tipoArchivo == 1)
                {
                    _repo.EliminarDocumentoCDP();
                    await _dataContext.SaveChangesAsync();
                }

                if (tipoArchivo == 2)
                {
                    _repo.EliminarDocumentoCompromiso();
                    await _dataContext.SaveChangesAsync();
                }

                if (tipoArchivo == 3)
                {
                    _repo.EliminarDocumentoObligacion();
                    await _dataContext.SaveChangesAsync();
                }

                if (tipoArchivo == 4)
                {
                    _repo.EliminarDocumentoOrdenPago();
                    await _dataContext.SaveChangesAsync();
                }

                #endregion Eliminar Documento CDP, Compromiso, Obligacion, OrdenPago

                #region Insertar Documento CDP, Compromiso, Obligacion, OrdenPago

                if (listaCdp != null && listaCdp.Count > 0)
                {
                    listaCdp.Select(c => { c.PciId = pciId; return c; }).ToList();
                    _repo.InsertarListaDocumentoCDP(listaCdp);
                    await _dataContext.SaveChangesAsync();
                }

                if (listaCompromiso != null && listaCompromiso.Count > 0)
                {
                    listaCompromiso.Select(c => { c.PciId = pciId; return c; }).ToList();
                    _repo.InsertarListaDocumentoCompromiso(listaCompromiso);
                    await _dataContext.SaveChangesAsync();
                }

                if (listaObligacion != null && listaObligacion.Count > 0)
                {
                    listaObligacion.Select(c => { c.PciId = pciId; return c; }).ToList();
                    _repo.InsertarListaDocumentoObligacion(listaObligacion);
                    await _dataContext.SaveChangesAsync();
                }

                if (listaOrdenPago != null && listaOrdenPago.Count > 0)
                {
                    listaOrdenPago.Select(c => { c.PciId = pciId; return c; }).ToList();
                    _repo.InsertarListaDocumentoOrdenPago(listaOrdenPago);
                    await _dataContext.SaveChangesAsync();
                }

                #endregion Insertar Documento CDP, Compromiso, Obligacion, OrdenPago

                #region Insertar CDP

                if (listaCdp != null && listaCdp.Count > 0)
                {
                    await _cdpRepo.InsertarDataCDPDeReporte(1);
                    await _dataContext.SaveChangesAsync();
                }

                if (listaCompromiso != null && listaCompromiso.Count > 0)
                {
                    await _cdpRepo.InsertarDataCDPDeReporte(2);
                    await _dataContext.SaveChangesAsync();
                }

                if (listaObligacion != null && listaObligacion.Count > 0)
                {
                    await _cdpRepo.InsertarDataCDPDeReporte(3);
                    await _dataContext.SaveChangesAsync();
                }

                if (listaOrdenPago != null && listaOrdenPago.Count > 0)
                {
                    await _cdpRepo.InsertarDataCDPDeReporte(4);
                    await _dataContext.SaveChangesAsync();
                }

                #endregion Insertar CDP

                transaction.Commit();
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}