using System.Collections.Generic;
using System.Threading.Tasks;
using ComplementApp.API.Models;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using ComplementApp.API.Dtos;
using ComplementApp.API.Interfaces;
using ComplementApp.API.Helpers;
using System.Globalization;

namespace ComplementApp.API.Data
{
    public class ObligacionRepository : IObligacionRepository
    {
        private readonly DataContext _context;

        public ObligacionRepository(DataContext context)
        {
            _context = context;
        }

        #region Registro de Solicitud de Pago

        public async Task<PagedList<CDPDto>> ObtenerCompromisosParaSolicitudRegistroPago(int usuarioId, int perfilId, int? terceroId, UserParams userParams)
        {
            IQueryable<CDPDto> lista = null;
            var usuario = await _context.Usuario.Where(x => x.UsuarioId == usuarioId).FirstOrDefaultAsync();
            string nombreCompleto = usuario.Nombres.ToUpper().Trim() + ' ' + usuario.Apellidos.ToUpper().Trim();


            if (perfilId == (int)PerfilUsuario.Administrador || perfilId == (int)PerfilUsuario.CoordinadorFinanciero)
            {
                lista = (from c in _context.CDP
                         join t in _context.Tercero on c.TerceroId equals t.TerceroId
                         where c.Instancia == (int)TipoDocumento.Compromiso
                         where c.SaldoActual > 0 //Saldo Disponible
                         where c.TerceroId == terceroId || terceroId == null
                         select new CDPDto()
                         {
                             CdpId = c.CdpId,
                             Crp = c.Crp,
                             Detalle4 = c.Detalle4.Length > 100 ? c.Detalle4.Substring(0, 100) + "..." : c.Detalle4,
                             NumeroIdentificacionTercero = t.NumeroIdentificacion,
                             NombreTercero = t.Nombre,
                         })
                      .Distinct()
                      .OrderBy(x => x.Crp);
            }
            else if (perfilId == (int)PerfilUsuario.SupervisorContractual)
            {
                lista = (from c in _context.CDP
                         join t in _context.Tercero on c.TerceroId equals t.TerceroId
                         where c.Instancia == (int)TipoDocumento.Compromiso
                         where c.SaldoActual > 0 //Saldo Disponible
                         where c.Detalle5.ToUpper() == nombreCompleto
                         where c.TerceroId == terceroId || terceroId == null
                         select new CDPDto()
                         {
                             CdpId = c.CdpId,
                             Crp = c.Crp,
                             Detalle4 = c.Detalle4.Length > 100 ? c.Detalle4.Substring(0, 100) + "..." : c.Detalle4,
                             NumeroIdentificacionTercero = t.NumeroIdentificacion,
                             NombreTercero = t.Nombre,
                         })
                        .Distinct()
                        .OrderBy(x => x.Crp);
            }
            else if (perfilId == (int)PerfilUsuario.Contratista)
            {
                lista = (from c in _context.CDP
                         join t in _context.Tercero on c.TerceroId equals t.TerceroId
                         where c.Instancia == (int)TipoDocumento.Compromiso
                         where c.SaldoActual > 0 //Saldo Disponible
                         where c.TerceroId == terceroId
                         select new CDPDto()
                         {
                             CdpId = c.CdpId,
                             Crp = c.Crp,
                             Detalle4 = c.Detalle4.Length > 100 ? c.Detalle4.Substring(0, 100) + "..." : c.Detalle4,
                             NumeroIdentificacionTercero = t.NumeroIdentificacion,
                             NombreTercero = t.Nombre,
                         })
                        .Distinct()
                        .OrderBy(x => x.Crp);
            }

            return await PagedList<CDPDto>.CreateAsync(lista, userParams.PageNumber, userParams.PageSize);
        }

        public async Task<PagedList<CDPDto>> ObtenerSolicitudesPagoParaAprobar(int usuarioId, int? terceroId, UserParams userParams)
        {
            IQueryable<CDPDto> lista = null;
            var usuario = await _context.Usuario.Where(x => x.UsuarioId == usuarioId).FirstOrDefaultAsync();
            string nombreCompleto = usuario.Nombres.ToUpper().Trim() + ' ' + usuario.Apellidos.ToUpper().Trim();

            lista = (from s in _context.FormatoSolicitudPago
                     join c in _context.CDP on s.Crp equals c.Crp
                     join t in _context.Tercero on c.TerceroId equals t.TerceroId
                     where s.EstadoId == (int)EstadoSolicitudPago.Generado
                     where c.Instancia == (int)TipoDocumento.Compromiso
                     where c.SaldoActual > 0 //Saldo Disponible
                     where c.Detalle5.ToUpper() == nombreCompleto
                     where c.TerceroId == terceroId || terceroId == null
                     select new CDPDto()
                     {
                         CdpId = c.CdpId,
                         Crp = c.Crp,
                         Detalle4 = c.Detalle4.Length > 100 ? c.Detalle4.Substring(0, 100) + "..." : c.Detalle4,
                         NumeroIdentificacionTercero = t.NumeroIdentificacion,
                         NombreTercero = t.Nombre,
                         FormatoSolicitudPagoId = s.FormatoSolicitudPagoId
                     })
                    .Distinct()
                    .OrderBy(x => x.Crp);


            return await PagedList<CDPDto>.CreateAsync(lista, userParams.PageNumber, userParams.PageSize);
        }

        public async Task<FormatoSolicitudPago> ObtenerFormatoSolicitudPagoBase(int formatoSolicitudPagoId)
        {
            return await _context.FormatoSolicitudPago.FirstOrDefaultAsync(u => u.FormatoSolicitudPagoId == formatoSolicitudPagoId);
        }


        #endregion Registro de Solicitud de Pago

        public async Task<PagedList<CDPDto>> ObtenerCompromisosParaClavePresupuestalContable(int? terceroId, int? numeroCrp, UserParams userParams)
        {
            var listaCompromisos = _context.ClavePresupuestalContable.Select(x => x.Crp).ToHashSet();
            var notFoundItems = _context.CDP.Where(item => !listaCompromisos.Contains(item.Crp)).Select(x => x.Crp).ToHashSet();

            var lista = (from c in _context.CDP
                         join t in _context.Tercero on c.TerceroId equals t.TerceroId
                         where notFoundItems.Contains(c.Crp)
                         where c.Instancia == (int)TipoDocumento.Compromiso
                         where c.SaldoActual > 0 //Saldo Disponible
                         where c.Crp == numeroCrp || numeroCrp == null
                         where c.TerceroId == terceroId || terceroId == null
                         select new CDPDto()
                         {
                             CdpId = c.CdpId,
                             Crp = c.Crp,
                             Detalle4 = c.Detalle4.Length > 100 ? c.Detalle4.Substring(0, 100) + "..." : c.Detalle4,
                             NumeroIdentificacionTercero = t.NumeroIdentificacion,
                             NombreTercero = t.Nombre,
                         })
                        .Distinct()
                        .OrderBy(x => x.Crp);

            return await PagedList<CDPDto>.CreateAsync(lista, userParams.PageNumber, userParams.PageSize);
        }

        public async Task<ICollection<ClavePresupuestalContableDto>> ObtenerRubrosParaClavePresupuestalContable(int cdpId)
        {

            var detalles = await (from rp in _context.RubroPresupuestal
                                  join c in _context.CDP on rp.RubroPresupuestalId equals c.RubroPresupuestalId
                                  join t in _context.Tercero on c.TerceroId equals t.TerceroId
                                  join sf in _context.SituacionFondo on c.Detalle9 equals sf.Nombre
                                  join ff in _context.FuenteFinanciacion on c.Detalle8 equals ff.Nombre
                                  join r in _context.RecursoPresupuestal on c.Detalle10 equals r.Codigo
                                  where c.CdpId == cdpId
                                  select new ClavePresupuestalContableDto()
                                  {
                                      Crp = c.Crp,
                                      Dependencia = c.Detalle2,
                                      RubroPresupuestal = new ValorSeleccion()
                                      {
                                          Id = rp.RubroPresupuestalId,
                                          Codigo = rp.Identificacion,
                                          Nombre = rp.Nombre,
                                      },
                                      Tercero = new ValorSeleccion()
                                      {
                                          Id = t.TerceroId,
                                          Codigo = t.NumeroIdentificacion,
                                          Nombre = t.Nombre,
                                      },
                                      FuenteFinanciacion = new ValorSeleccion()
                                      {
                                          Id = ff.FuenteFinanciacionId,
                                          Nombre = c.Detalle8
                                      },
                                      SituacionFondo = new ValorSeleccion()
                                      {
                                          Id = sf.SituacionFondoId,
                                          Nombre = c.Detalle9
                                      },
                                      RecursoPresupuestal = new ValorSeleccion()
                                      {
                                          Id = r.RecursoPresupuestalId,
                                          Codigo = c.Detalle10,
                                          Nombre = r.Nombre
                                      },
                                  })
                                 .Distinct()
                                 .OrderBy(rp => rp.RubroPresupuestal.Codigo)
                                 .ToListAsync();

            return detalles;
        }

        public async Task<ICollection<RelacionContableDto>> ObtenerRelacionesContableXRubro(int rubroPresupuestalId)
        {
            var lista = await (from rc in _context.RelacionContable
                               join cc in _context.CuentaContable on rc.CuentaContableId equals cc.CuentaContableId
                               join ac in _context.AtributoContable on rc.AtributoContableId equals ac.AtributoContableId
                               join tg in _context.TipoGasto on rc.TipoGastoId equals tg.TipoGastoId into tipoGasto
                               from t in tipoGasto.DefaultIfEmpty()
                               where rc.RubroPresupuestalId == rubroPresupuestalId
                               select new RelacionContableDto()
                               {
                                   RelacionContableId = rc.RelacionContableId,
                                   TipoOperacion = rc.TipoOperacion,
                                   UsoContable = rc.UsoContable,
                                   CuentaContable = new ValorSeleccion()
                                   {
                                       Id = cc.CuentaContableId,
                                       Codigo = cc.NumeroCuenta,
                                       Nombre = cc.DescripcionCuenta,
                                   },
                                   AtributoContable = new ValorSeleccion()
                                   {
                                       Id = ac.AtributoContableId,
                                       Nombre = ac.Nombre
                                   },
                                   TipoGasto = new ValorSeleccion()
                                   {
                                       Id = t.TipoGastoId,
                                       Nombre = t.Nombre
                                   }
                               })
                                 .Distinct()
                                 .OrderBy(cc => cc.CuentaContable.Codigo)
                                 .ToListAsync();

            return lista;
        }

        public async Task<bool> RegistrarRelacionContable(RelacionContable relacion)
        {
            await _context.RelacionContable.AddAsync(relacion);
            return true;
        }

        public async Task<FormatoSolicitudPagoDto> ObtenerFormatoSolicitudPago(int cdpId)
        {
            var formato = await (from c in _context.CDP
                                 join co in _context.Contrato on c.Crp equals co.Crp
                                 join t in _context.Tercero on c.TerceroId equals t.TerceroId
                                 where c.CdpId == cdpId
                                 select new FormatoSolicitudPagoDto()
                                 {
                                     Cdp = new CDPDto()
                                     {
                                         CdpId = c.CdpId,
                                         Cdp = c.Cdp,
                                         Crp = c.Crp,
                                         Fecha = c.Fecha, //Fecha compromiso
                                         Detalle5 = c.Detalle5, //Supervisor
                                         Detalle4 = c.Detalle4, //objeto contrato
                                         ValorInicial = c.ValorInicial,
                                         Operacion = c.Operacion, //Valor adicion/reduccion
                                         ValorTotal = c.ValorTotal, //valor actual
                                         SaldoActual = c.SaldoActual, //saldo actual
                                     },
                                     Contrato = new Contrato()
                                     {
                                         ContratoId = co.ContratoId,
                                         NumeroContrato = co.NumeroContrato,
                                         Crp = co.Crp,
                                         FechaInicio = co.FechaInicio,
                                         FechaFinal = co.FechaFinal,
                                         FechaRegistro = co.FechaRegistro
                                     },
                                     Tercero = new TerceroDto()
                                     {
                                         TerceroId = t.TerceroId,
                                         Nombre = t.Nombre,
                                         NumeroIdentificacion = t.NumeroIdentificacion,
                                         Email = t.Email,
                                         Telefono = t.Telefono,
                                         Direccion = t.Direccion,
                                         FechaExpedicionDocumento = t.FechaExpedicionDocumento,
                                         RegimenTributario = t.RegimenTributario,
                                         DeclaranteRentaDescripcion = t.DeclaranteRenta ? "SI" : "NO",
                                         FacturadorElectronicoDescripcion = t.FacturadorElectronico ? "SI" : "NO"
                                     }
                                 }).FirstOrDefaultAsync();


            return formato;
        }

        public async Task<FormatoSolicitudPagoDto> ObtenerFormatoSolicitudPagoXId(int formatoSolicitudPagoId)
        {
            var formato = await (from s in _context.FormatoSolicitudPago
                                 join c in _context.CDP on s.Crp equals c.Crp
                                 join co in _context.Contrato on c.Crp equals co.Crp
                                 join t in _context.Tercero on c.TerceroId equals t.TerceroId
                                 join ae in _context.ActividadEconomica on s.ActividadEconomicaId equals ae.ActividadEconomicaId
                                 join pp in _context.PlanPago on s.PlanPagoId equals pp.PlanPagoId
                                 where s.FormatoSolicitudPagoId == formatoSolicitudPagoId
                                 select new FormatoSolicitudPagoDto()
                                 {
                                     FormatoSolicitudPagoId = s.FormatoSolicitudPagoId,
                                     PlanPagoId = s.PlanPagoId,
                                     FechaInicio = s.FechaInicio,
                                     FechaFinal = s.FechaFinal,
                                     ValorFacturado = s.valorFacturado,
                                     MesId = s.MesId,
                                     MesDescripcion = DateTimeFormatInfo.CurrentInfo.GetMonthName(s.MesId).ToUpper(),
                                     NumeroPlanilla = s.NumeroPlanilla,
                                     NumeroFactura = s.NumeroFactura,
                                     Observaciones = s.Observaciones,
                                     BaseCotizacion = s.BaseCotizacion,

                                     Cdp = new CDPDto()
                                     {
                                         CdpId = c.CdpId,
                                         Cdp = c.Cdp,
                                         Crp = c.Crp,
                                         Fecha = c.Fecha, //Fecha compromiso
                                         Detalle5 = c.Detalle5, //Supervisor
                                         Detalle4 = c.Detalle4, //objeto contrato
                                         ValorInicial = c.ValorInicial,
                                         Operacion = c.Operacion, //Valor adicion/reduccion
                                         ValorTotal = c.ValorTotal, //valor actual
                                         SaldoActual = c.SaldoActual, //saldo actual
                                     },
                                     ActividadEconomica = new ValorSeleccion()
                                     {
                                         Id = ae.ActividadEconomicaId,
                                         Codigo = ae.Codigo,
                                         Nombre = ae.Nombre,
                                     },
                                     Contrato = new Contrato()
                                     {
                                         ContratoId = co.ContratoId,
                                         NumeroContrato = co.NumeroContrato,
                                         Crp = co.Crp,
                                         FechaInicio = co.FechaInicio,
                                         FechaFinal = co.FechaFinal,
                                         FechaRegistro = co.FechaRegistro
                                     },
                                     Tercero = new TerceroDto()
                                     {
                                         TerceroId = t.TerceroId,
                                         Nombre = t.Nombre,
                                         NumeroIdentificacion = t.NumeroIdentificacion,
                                         Email = t.Email,
                                         Telefono = t.Telefono,
                                         Direccion = t.Direccion,
                                         FechaExpedicionDocumento = t.FechaExpedicionDocumento,
                                         RegimenTributario = t.RegimenTributario,
                                         DeclaranteRentaDescripcion = t.DeclaranteRenta ? "SI" : "NO",
                                         FacturadorElectronicoDescripcion = t.FacturadorElectronico ? "SI" : "NO"
                                     },
                                     PlanPago = new PlanPagoDto()
                                     {
                                         PlanPagoId = pp.PlanPagoId,
                                         NumeroPago = pp.NumeroPago,
                                         ValorFacturado = pp.ValorFacturado.Value,
                                         ViaticosDescripcion = pp.Viaticos ? "SI" : "NO",
                                     }
                                 }).FirstOrDefaultAsync();

            return formato;
        }


        public async Task<ICollection<CDPDto>> ObtenerPagosRealizadosXCompromiso(long crp)
        {
            var lista = await (from c in _context.CDP
                               where c.Instancia == (int)TipoDocumento.OrdenPago
                               where c.Crp == crp
                               select new CDPDto()
                               {
                                   CdpId = c.CdpId,
                                   Cdp = c.Cdp,
                                   Crp = c.Crp,
                                   OrdenPago = c.OrdenPago,
                                   Detalle1 = c.Detalle1.ToUpper(), //Estado OP
                                   Fecha = c.Fecha, //Fecha Orden Pago
                                   Detalle5 = c.Detalle5, //Supervisor
                                   Detalle4 = c.Detalle4, //objeto contrato
                                   ValorInicial = c.ValorInicial, //Valor Bruto
                                   Operacion = c.Operacion, //Valor deducciones
                                   ValorTotal = c.ValorTotal, //valor neto
                               })
                                 .Distinct()
                                 .OrderBy(c => c.OrdenPago)
                                 .ToListAsync();
            return lista;
        }

        static string UppercaseFirst(string s)
        {
            // Check for empty string.
            if (string.IsNullOrEmpty(s))
            {
                return string.Empty;
            }
            // Return char and concat substring.
            return char.ToUpper(s[0]) + s.Substring(1);
        }
    }
}