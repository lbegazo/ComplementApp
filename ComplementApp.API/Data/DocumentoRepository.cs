using System;
using System.Collections.Generic;
using System.Linq;
using ComplementApp.API.Dtos;
using ComplementApp.API.Interfaces.Repository;
using ComplementApp.API.Models;
using ComplementApp.API.Models.ExcelDocumento;
using EFCore.BulkExtensions;

namespace ComplementApp.API.Data
{
    public class DocumentoRepository : IDocumentoRepository
    {
        private readonly DataContext _context;

        public DocumentoRepository(DataContext context)
        {
            _context = context;
        }

        #region Carga Masiva PAA
        public bool InsertaCabeceraCDP(IList<CDPDto> listaCdp)
        {
            try
            {
                #region Setear datos

                List<CDP> listaCDP = obtenerListaCdp(listaCdp);

                #endregion Setear datos

                _context.BulkInsert(listaCDP);
                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool InsertaCabeceraCDPConTercero(IList<CDPDto> lista)
        {
            try
            {
                #region Setear datos

                List<CDP> listaCDP = obtenerListaCdpConTercero(lista);

                #endregion Setear datos

                _context.BulkInsert(listaCDP);
                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool InsertaPlanDePago(IList<PlanPagoDto> lista)
        {
            try
            {
                #region Setear datos

                List<PlanPago> listaPlanPago = obtenerListaPlanPago(lista);

                #endregion Setear datos

                _context.BulkInsert(listaPlanPago);
                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool InsertaDetalleCDP(IList<DetalleCDPDto> lista)
        {
            try
            {
                #region Setear datos

                List<PlanAdquisicion> listaCDP = obtenerListaDetalleCdp(lista);

                #endregion Setear datos

                _context.BulkInsert(listaCDP);
                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool EliminarCabeceraCDP()
        {
            try
            {
                if (!_context.CDP.Any())
                    return true;

                if (_context.CDP.Any())
                    return _context.CDP.BatchDelete() > 0;
            }
            catch (Exception)
            {
                throw;
            }
            return false;
        }

        public bool EliminarCabeceraCDPXInstancia(int instancia)
        {
            try
            {
                if (!_context.CDP.Any(x => x.Instancia == instancia))
                    return true;

                return _context.CDP.Where(x => x.Instancia == instancia).BatchDelete() > 0;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool EliminarDetalleCDP()
        {
            try
            {
                if (!_context.PlanAdquisicion.Any())
                    return true;

                if (_context.PlanAdquisicion.Any())
                    return _context.PlanAdquisicion.BatchDelete() > 0;
            }
            catch (Exception)
            {
                throw;
            }
            return false;
        }

        private bool EliminarParametroLiquidacionTercero()
        {
            try
            {
                if (!_context.ParametroLiquidacionTercero.Any())
                    return true;

                if (_context.ParametroLiquidacionTercero.Any())
                    return _context.ParametroLiquidacionTercero.BatchDelete() > 0;
            }
            catch (Exception)
            {
                throw;
            }
            return false;
        }

        private List<CDP> obtenerListaCdp(IList<CDPDto> listaCdp)
        {

            List<CDP> listaCDP = new List<CDP>();
            CDP cdp = null;
            Tercero tercero = null;

            var listaRubrosPresupuestales = _context.RubroPresupuestal.ToList();
            var listaPci = _context.Pci.ToList();
            var listaTerceros = _context.Tercero.ToList();

            foreach (var item in listaCdp)
            {
                cdp = new CDP();

                cdp.Instancia = item.Instancia;
                cdp.Cdp = item.Cdp;
                cdp.Crp = item.Crp;
                cdp.Obligacion = item.Obligacion;
                cdp.OrdenPago = item.OrdenPago;
                cdp.Fecha = item.Fecha;

                cdp.ValorInicial = item.ValorInicial;
                cdp.Operacion = item.Operacion;
                cdp.ValorTotal = item.ValorTotal;
                cdp.SaldoActual = item.SaldoActual;

                cdp.Detalle1 = item.Detalle1;
                cdp.Detalle2 = item.Detalle2;
                cdp.Detalle3 = item.Detalle3;
                cdp.Detalle4 = item.Detalle4;
                cdp.Detalle5 = string.Empty;
                cdp.Detalle6 = item.Detalle6;
                cdp.Detalle7 = item.Detalle7;
                cdp.Detalle8 = item.Detalle8;
                cdp.Detalle9 = item.Detalle9;
                cdp.Detalle10 = item.Detalle10;

                #region PCI

                if (!string.IsNullOrEmpty(item.IdentificacionRubro))
                {
                    var pci = listaPci
                                    .Where(c => c.Identificacion.ToUpper() == item.Pci.ToUpper())
                                    .FirstOrDefault();
                    if (pci != null)
                    {
                        cdp.Pci = pci;
                        cdp.PciId = pci.PciId;
                    }
                }

                #endregion PCI

                #region Rubro Presupuestal

                if (!string.IsNullOrEmpty(item.IdentificacionRubro))
                {
                    var rubro = listaRubrosPresupuestales
                                    .Where(c => c.Identificacion.ToUpper() == item.IdentificacionRubro.ToUpper())
                                    .FirstOrDefault();
                    if (rubro != null)
                    {
                        cdp.RubroPresupuestal = rubro;
                        cdp.RubroPresupuestalId = rubro.RubroPresupuestalId;
                    }
                }

                #endregion Rubro Presupuestal

                #region Tercero

                if (!string.IsNullOrEmpty(item.NumeroIdentificacionTercero))
                {
                    tercero = listaTerceros
                                .Where(c => c.NumeroIdentificacion == item.NumeroIdentificacionTercero
                                            && c.TipoIdentificacion == item.TipoIdentificacionTercero
                                            ).FirstOrDefault();

                    if (tercero != null)
                    {
                        cdp.Tercero = tercero;
                        cdp.TerceroId = tercero.TerceroId;
                    }
                }

                #endregion Tercero

                listaCDP.Add(cdp);
            }

            return listaCDP;
        }

        private List<CDP> obtenerListaCdpConTercero(IList<CDPDto> lista)
        {

            List<CDP> listaCDP = new List<CDP>();
            CDP cdp = null;
            Tercero tercero = null;

            var listaRubrosPresupuestales = _context.RubroPresupuestal.ToList();
            //var listaTerceros = _context.Tercero.ToList();

            foreach (var item in lista)
            {
                cdp = new CDP();

                cdp.Instancia = item.Instancia;
                cdp.Cdp = item.Cdp;
                cdp.Crp = item.Crp;
                cdp.Obligacion = item.Obligacion;
                cdp.OrdenPago = item.OrdenPago;
                cdp.Fecha = item.Fecha;

                cdp.ValorInicial = item.ValorInicial;
                cdp.Operacion = item.Operacion;
                cdp.ValorTotal = item.ValorTotal;
                cdp.SaldoActual = item.SaldoActual;

                cdp.Detalle1 = item.Detalle1;
                cdp.Detalle2 = item.Detalle2;
                cdp.Detalle3 = item.Detalle3;
                cdp.Detalle4 = item.Detalle4;
                cdp.Detalle5 = item.Detalle5;
                cdp.Detalle6 = item.Detalle6;
                cdp.Detalle7 = item.Detalle7;
                cdp.Detalle8 = item.Detalle8;
                cdp.Detalle9 = item.Detalle9;

                //Rubro Presupuestal
                if (!string.IsNullOrEmpty(item.IdentificacionRubro))
                {
                    var rubro = listaRubrosPresupuestales
                                    .Where(c => c.Identificacion.ToUpper() == item.IdentificacionRubro.ToUpper())
                                    .FirstOrDefault();
                    if (rubro != null)
                    {
                        cdp.RubroPresupuestal = rubro;
                        cdp.RubroPresupuestalId = rubro.RubroPresupuestalId;
                    }
                }

                #region Tercero

                if (!string.IsNullOrEmpty(item.NumeroIdentificacionTercero))
                {
                    tercero = _context.Tercero
                                .Where(c => c.NumeroIdentificacion == item.NumeroIdentificacionTercero
                                            && c.TipoIdentificacion == item.TipoIdentificacionTercero
                                            ).FirstOrDefault();

                    if (tercero == null)
                    {
                        this.InsertarTercero(item);

                        tercero = _context.Tercero
                                .Where(c => c.NumeroIdentificacion == item.NumeroIdentificacionTercero
                                            && c.TipoIdentificacion == item.TipoIdentificacionTercero
                                            ).FirstOrDefault();
                    }

                    if (tercero != null)
                    {
                        cdp.Tercero = tercero;
                        cdp.TerceroId = tercero.TerceroId;
                    }
                }

                #endregion Tercero

                listaCDP.Add(cdp);
            }

            return listaCDP;
        }

        private List<PlanAdquisicion> obtenerListaDetalleCdp(IList<DetalleCDPDto> lista)
        {
            List<PlanAdquisicion> listaCDP = new List<PlanAdquisicion>();
            PlanAdquisicion cdp = null;

            var listaActividadGeneral = _context.ActividadGeneral.ToList();
            var listaActividadEspecifica = _context.ActividadEspecifica.ToList();
            var listaDependencia = _context.Dependencia.ToList();
            var listaRubrosPresupuestales = _context.RubroPresupuestal.ToList();
            var listaUsuarios = _context.Usuario.ToList();
            var listaArea = _context.Area.ToList();

            foreach (var item in lista)
            {
                cdp = new PlanAdquisicion();

                cdp.PcpId = item.PcpId;
                cdp.IdArchivo = item.IdArchivo;
                cdp.Cdp = item.Cdp;
                cdp.Proy = item.Proy;
                cdp.Prod = item.Prod;
                cdp.PlanDeCompras = item.PlanDeCompras;
                cdp.AplicaContrato = item.AplicaContrato;
                cdp.Convenio = item.Convenio;
                cdp.PlanDeCompras = item.PlanDeCompras;

                cdp.ValorAct = item.ValorAct;
                cdp.SaldoAct = item.SaldoAct;
                cdp.ValorCDP = item.ValorCDP;
                cdp.ValorRP = item.ValorRP;
                cdp.ValorOB = item.ValorOB;
                cdp.ValorOP = item.ValorOP;
                cdp.SaldoTotal = item.SaldoTotal;
                cdp.Valor_Convenio = item.Valor_Convenio;

                //Actividad General
                if (!string.IsNullOrEmpty(item.Proyecto))
                {
                    var actividad = (from ag in listaActividadGeneral
                                     join rp in listaRubrosPresupuestales on ag.RubroPresupuestalId equals rp.RubroPresupuestalId
                                     where (rp.Nombre.Trim().ToUpper() == item.Proyecto.Trim().ToUpper())
                                     select ag).FirstOrDefault();
                    if (actividad != null)
                    {
                        cdp.ActividadGeneralId = actividad.ActividadGeneralId;
                    }
                }

                //Actividad Especifica
                if (!string.IsNullOrEmpty(item.ActividadBpin))
                {
                    var actividad = listaActividadEspecifica
                                        .Where(c => c.Nombre.ToUpper() == item.ActividadBpin.ToUpper())
                                        .FirstOrDefault();
                    if (actividad != null)
                        cdp.ActividadEspecificaId = actividad.ActividadEspecificaId;
                }

                //Dependencia
                if (!string.IsNullOrEmpty(item.Dependencia))
                {
                    var dependencia = listaDependencia
                                        .Where(c => c.Nombre.ToUpper() == item.Dependencia.ToUpper())
                                        .FirstOrDefault();
                    if (dependencia != null)
                        cdp.DependenciaId = dependencia.DependenciaId;
                }

                //Rubro Presupuestal
                if (!string.IsNullOrEmpty(item.RubroPresupuestal.Identificacion))
                {
                    var rubro = listaRubrosPresupuestales
                                .Where(c => c.Identificacion.ToUpper() == item.RubroPresupuestal.Identificacion.ToUpper())
                                .FirstOrDefault();
                    if (rubro != null)
                    {
                        cdp.RubroPresupuestalId = rubro.RubroPresupuestalId;
                    }
                }

                //Decreto
                if (!string.IsNullOrEmpty(item.Decreto))
                {
                    var decreto = listaRubrosPresupuestales
                                .Where(c => c.Identificacion.ToUpper() == item.Decreto.ToUpper())
                                .FirstOrDefault();

                    if (decreto != null)
                        cdp.DecretoId = decreto.RubroPresupuestalId;
                }

                //Area
                if (!string.IsNullOrEmpty(item.Area))
                {
                    var area = listaArea
                                .Where(c => c.Nombre.ToUpper() == item.Area.ToUpper())
                                .FirstOrDefault();

                    if (area != null)
                        cdp.AreaId = area.AreaId;
                }

                //Usuario
                if (!string.IsNullOrEmpty(item.Responsable))
                {
                    var usuario = listaUsuarios
                                .Where(c => c.Nombres.ToUpper() + " " + c.Apellidos.ToUpper() == item.Responsable.ToUpper())
                                .FirstOrDefault();
                    if (usuario != null)
                        cdp.UsuarioId = usuario.UsuarioId;
                }
                listaCDP.Add(cdp);
            }

            return listaCDP;
        }

        private List<PlanPago> obtenerListaPlanPago(IList<PlanPagoDto> lista)
        {
            List<PlanPago> listaCDP = new List<PlanPago>();
            PlanPago cdp = null;

            var listaTercero = _context.Tercero.ToList();
            var listaRubrosPresupuestales = _context.RubroPresupuestal.ToList();
            var listaUsoPresupuestal = _context.UsoPresupuestal.ToList();
            var listaEstadoPlanPago = _context.Estado
                                            .Where(x => x.TipoDocumento.ToUpper() == "PLANDEPAGOS")
                                            .ToList();
            var listaEstadoOrdenPago = _context.Estado
                                            .Where(x => x.TipoDocumento.ToUpper() == "ORDENPAGO")
                                            .ToList();

            foreach (var item in lista)
            {
                cdp = new PlanPago();

                cdp.Cdp = item.Cdp;
                cdp.Crp = item.Crp;
                cdp.AnioPago = item.AnioPago;
                cdp.MesPago = item.MesPago;

                if (item.ValorInicial > 0)
                    cdp.ValorInicial = item.ValorInicial;
                if (item.ValorAPagar > 0)
                    cdp.ValorAPagar = item.ValorAPagar;
                if (item.SaldoDisponible > 0)
                    cdp.SaldoDisponible = item.SaldoDisponible;

                cdp.Viaticos = item.ViaticosDescripcion == "NO" ? false : true;

                if (item.NumeroPago > 0)
                    cdp.NumeroPago = item.NumeroPago;
                if (!string.IsNullOrEmpty(item.NumeroRadicadoProveedor))
                    cdp.NumeroRadicadoProveedor = item.NumeroRadicadoProveedor;
                if (item.FechaRadicadoProveedor != DateTime.MinValue)
                    cdp.FechaRadicadoProveedor = item.FechaRadicadoProveedor;
                if (!string.IsNullOrEmpty(item.NumeroRadicadoSupervisor))
                    cdp.NumeroRadicadoSupervisor = item.NumeroRadicadoSupervisor;
                if (item.FechaRadicadoSupervisor != DateTime.MinValue)
                    cdp.FechaRadicadoSupervisor = item.FechaRadicadoSupervisor;
                if (!string.IsNullOrEmpty(item.NumeroFactura))
                    cdp.NumeroFactura = item.NumeroFactura;
                if (item.ValorFacturado > 0)
                    cdp.ValorFacturado = item.ValorFacturado;
                if (!string.IsNullOrEmpty(item.Observaciones))
                    cdp.Observaciones = item.Observaciones;
                if (item.FechaFactura != DateTime.MinValue)
                    cdp.FechaFactura = item.FechaFactura;

                //Tercero
                if (item.TipoIdentificacionTercero > 0 &&
                    !string.IsNullOrEmpty(item.IdentificacionTercero))
                {
                    var tercero = listaTercero
                                        .Where(c => c.TipoIdentificacion == item.TipoIdentificacionTercero)
                                        .Where(c => c.NumeroIdentificacion == item.IdentificacionTercero)
                                        .FirstOrDefault();
                    if (tercero != null)
                        cdp.TerceroId = tercero.TerceroId;
                }

                //Estado PlanPago
                if (!string.IsNullOrEmpty(item.EstadoPlanPago))
                {
                    var estado = listaEstadoPlanPago
                                        .Where(c => c.Nombre.ToUpper() == item.EstadoPlanPago.ToUpper())
                                        .FirstOrDefault();
                    if (estado != null)
                        cdp.EstadoPlanPagoId = estado.EstadoId;
                }

                //Estado OrdenPago
                if (!string.IsNullOrEmpty(item.EstadoOrdenPago))
                {
                    var estado = listaEstadoOrdenPago
                                        .Where(c => c.Nombre.ToUpper() == item.EstadoOrdenPago.ToUpper())
                                        .FirstOrDefault();
                    if (estado != null)
                        cdp.EstadoOrdenPagoId = estado.EstadoId;
                }

                //Rubro Presupuestal
                if (!string.IsNullOrEmpty(item.IdentificacionRubroPresupuestal))
                {
                    var rubro = listaRubrosPresupuestales
                                .Where(c => c.Identificacion.ToUpper() == item.IdentificacionRubroPresupuestal.ToUpper())
                                .FirstOrDefault();
                    if (rubro != null)
                        cdp.RubroPresupuestalId = rubro.RubroPresupuestalId;
                }

                //Uso Presupuestal
                if (!string.IsNullOrEmpty(item.IdentificacionUsoPresupuestal))
                {
                    var uso = listaUsoPresupuestal
                                .Where(c => c.Identificacion.ToUpper() == item.IdentificacionUsoPresupuestal.ToUpper())
                                .FirstOrDefault();
                    if (uso != null)
                        cdp.UsoPresupuestalId = uso.UsoPresupuestalId;
                }

                listaCDP.Add(cdp);
            }

            return listaCDP;
        }

        private List<ParametroLiquidacionTercero> obtenerListaParametroLiquidacionTercero(IList<ParametroLiquidacionTerceroDto> lista)
        {
            List<ParametroLiquidacionTercero> listaCDP = new List<ParametroLiquidacionTercero>();
            ParametroLiquidacionTercero nuevoItem = null;

            var listaTercero = _context.Tercero.ToList();

            foreach (var item in lista)
            {
                nuevoItem = new ParametroLiquidacionTercero();
                nuevoItem.Afc = item.Afc;
                nuevoItem.AportePension = item.AportePension;
                nuevoItem.AporteSalud = item.AporteSalud;
                nuevoItem.BaseAporteSalud = item.BaseAporteSalud;
                nuevoItem.Dependiente = item.Dependiente;
                nuevoItem.FondoSolidaridad = item.FondoSolidaridad;
                nuevoItem.HonorarioSinIva = item.HonorarioSinIva;
                nuevoItem.InteresVivienda = item.InteresVivienda;
                nuevoItem.MedicinaPrepagada = item.MedicinaPrepagada;
                nuevoItem.ModalidadContrato = item.ModalidadContrato;
                nuevoItem.PensionVoluntaria = item.PensionVoluntaria;
                nuevoItem.RiesgoLaboral = item.RiesgoLaboral;
                nuevoItem.TarifaIva = item.TarifaIva;
                nuevoItem.TipoCuentaXPagarId = item.TipoCuentaXPagarId;
                nuevoItem.TipoDocumentoSoporteId = item.TipoDocumentoSoporteId;
                nuevoItem.TipoIva = item.TipoIva;
                nuevoItem.TipoPago = item.TipoPago;

                //Tercero
                if (item.TipoDocumentoIdentidadId > 0 &&
                    !string.IsNullOrEmpty(item.IdentificacionTercero))
                {
                    var tercero = listaTercero
                                        .Where(c => c.TipoIdentificacion == item.TipoDocumentoIdentidadId)
                                        .Where(c => c.NumeroIdentificacion == item.IdentificacionTercero)
                                        .FirstOrDefault();
                    if (tercero != null)
                        nuevoItem.TerceroId = tercero.TerceroId;
                }

                listaCDP.Add(nuevoItem);

            }


            return listaCDP;
        }

        private void InsertarTercero(CDPDto documento)
        {
            var tercero = new Tercero();
            tercero.NumeroIdentificacion = documento.NumeroIdentificacionTercero;
            tercero.TipoIdentificacion = documento.TipoIdentificacionTercero;
            tercero.Nombre = documento.NombreTercero;
            _context.Tercero.Add(tercero);
            _context.SaveChanges();
        }

        #endregion Carga Masiva PAA

        #region Carga Registro Gestion Presupuestal

        public bool EliminarDocumentoCDP()
        {
            try
            {
                if (!_context.DocumentoCdp.Any())
                    return true;

                if (_context.DocumentoCdp.Any())
                    return _context.DocumentoCdp.BatchDelete() > 0;
            }
            catch (Exception)
            {
                throw;
            }
            return false;
        }

        public bool EliminarDocumentoCompromiso()
        {
            try
            {
                if (!_context.DocumentoCompromiso.Any())
                    return true;

                if (_context.DocumentoCompromiso.Any())
                    return _context.DocumentoCompromiso.BatchDelete() > 0;
            }
            catch (Exception)
            {
                throw;
            }
            return false;
        }

        public bool EliminarDocumentoObligacion()
        {
            try
            {
                if (!_context.DocumentoObligacion.Any())
                    return true;

                if (_context.DocumentoObligacion.Any())
                    return _context.DocumentoObligacion.BatchDelete() > 0;
            }
            catch (Exception)
            {
                throw;
            }
            return false;
        }

        public bool EliminarDocumentoOrdenPago()
        {
            try
            {
                if (!_context.DocumentoOrdenPago.Any())
                    return true;

                if (_context.DocumentoOrdenPago.Any())
                    return _context.DocumentoOrdenPago.BatchDelete() > 0;
            }
            catch (Exception)
            {
                throw;
            }
            return false;
        }

        public bool InsertarListaDocumentoCDP(IList<DocumentoCdp> listaCdp)
        {
            try
            {
                _context.BulkInsert(listaCdp);
                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool InsertarListaDocumentoObligacion(IList<DocumentoObligacion> lista)
        {
            try
            {
                _context.BulkInsert(lista);
                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool InsertarListaDocumentoCompromiso(IList<DocumentoCompromiso> lista)
        {
            try
            {
                _context.BulkInsert(lista);
                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool InsertarListaDocumentoOrdenPago(IList<DocumentoOrdenPago> lista)
        {
            try
            {
                _context.BulkInsert(lista);
                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion Carga Registro Gestion Presupuestal
    }
}