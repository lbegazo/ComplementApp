using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ComplementApp.API.Dtos;
using ComplementApp.API.Models;
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
        public bool InsertaCabeceraCDP(IList<CDPDto> lista)
        {
            try
            {
                #region Por Eliminar

                // CancellationToken cancellation = new CancellationToken(false);
                // var bulkConfig = new BulkConfig
                // {
                //     PreserveInsertOrder = true,
                //     SetOutputIdentity = true,
                //     BatchSize = 4000
                // };
                //await _context.BulkInsertAsync(lista, bulkConfig, null, cancellation);

                #endregion Por Eliminar

                #region Setear datos

                List<CDP> listaCDP = obtenerListaCdp(lista);

                #endregion Setear datos

                _context.BulkInsert(listaCDP);
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
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
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //public async Task<bool> InsertaDetalleCDP(IList<DetalleCDP> lista)
        public bool InsertaDetalleCDP(IList<DetalleCDPDto> lista)
        {
            try
            {
                #region Setear datos

                List<DetalleCDP> listaCDP = obtenerListaDetalleCdp(lista);

                #endregion Setear datos

                _context.BulkInsert(listaCDP);
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("error " + ex.Message);
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
            catch (Exception ex)
            {
                throw ex;
            }
            return false;
        }

        public bool EliminarDetalleCDP()
        {
            try
            {
                if (!_context.DetalleCDP.Any())
                    return true;

                if (_context.DetalleCDP.Any())
                    return _context.DetalleCDP.BatchDelete() > 0;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return false;
        }

        private List<CDP> obtenerListaCdp(IList<CDPDto> lista)
        {
            List<CDP> listaCDP = new List<CDP>();
            CDP cdp = null;

            var listaRubrosPresupuestales = _context.RubroPresupuestal.ToList();
            var listaTerceros = _context.Tercero.ToList();

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

                //Tercero
                if (!string.IsNullOrEmpty(item.NumeroIdentificacion))
                {
                    var tercero = listaTerceros
                                .Where(c => c.NumeroIdentificacion == item.NumeroIdentificacion
                                            && c.TipoIdentificacion == item.TipoIdentificacion).FirstOrDefault();
                    if (tercero != null)
                    {
                        cdp.Tercero = tercero;
                        cdp.TerceroId = tercero.TerceroId;
                    }
                }
                listaCDP.Add(cdp);
            }

            return listaCDP;
        }

        private List<DetalleCDP> obtenerListaDetalleCdp(IList<DetalleCDPDto> lista)
        {
            List<DetalleCDP> listaCDP = new List<DetalleCDP>();
            DetalleCDP cdp = null;

            var listaActividadGeneral = _context.ActividadGeneral.ToList();
            var listaActividadEspecifica = _context.ActividadEspecifica.ToList();
            var listaDependencia = _context.Dependencia.ToList();
            var listaRubrosPresupuestales = _context.RubroPresupuestal.ToList();
            var listaUsuarios = _context.Usuario.ToList();
            var listaArea = _context.Area.ToList();

            foreach (var item in lista)
            {
                cdp = new DetalleCDP();

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
                    var actividad = listaActividadGeneral
                                        .Where(c => c.Nombre.ToUpper() == item.Proyecto.ToUpper())
                                        .FirstOrDefault();
                    if (actividad != null)
                        cdp.ActividadGeneralId = actividad.ActividadGeneralId;
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
                if (!string.IsNullOrEmpty(item.IdentificacionRubro))
                {
                    var rubro = listaRubrosPresupuestales
                                .Where(c => c.Identificacion.ToUpper() == item.IdentificacionRubro.ToUpper())
                                .FirstOrDefault();
                    if (rubro != null)
                        cdp.RubroPresupuestalId = rubro.RubroPresupuestalId;
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
                cdp.ValorInicial = item.ValorInicial;
                cdp.ValorAPagar = item.ValorAPagar;
                cdp.ValorPagado = item.ValorPagado;

                cdp.Viaticos = item.Viaticos == "NO" ? false : true;
                cdp.NumeroPago = item.NumeroPago;
                cdp.NumeroRadicadoProveedor = item.NumeroRadicadoProveedor;
                cdp.FechaRadicadoProveedor = item.FechaRadicadoProveedor;
                cdp.NumeroRadicadoSupervisor = item.NumeroRadicadoSupervisor;
                cdp.FechaRadicadoSupervisor = item.FechaRadicadoSupervisor;
                cdp.NumeroFactura = item.NumeroFactura;
                cdp.ValorFacturado = item.ValorFacturado;
                cdp.Observaciones = item.Observaciones;
                cdp.FechaFactura = item.FechaFactura;
                cdp.Obligacion = item.Obligacion;
                cdp.OrdenPago = item.OrdenPago;
                cdp.FechaOrdenPago = item.FechaOrdenPago;
                cdp.DiasAlPago = item.DiasAlPago;

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

    }
}