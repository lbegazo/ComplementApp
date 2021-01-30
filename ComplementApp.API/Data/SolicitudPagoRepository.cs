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
    public class SolicitudPagoRepository : ISolicitudPagoRepository
    {
        private readonly DataContext _context;
        private readonly IGeneralInterface _generalInterface;

        public SolicitudPagoRepository(DataContext context, IGeneralInterface generalInterface)
        {
            _context = context;
            this._generalInterface = generalInterface;
        }

        #region Registro de Solicitud de Pago

        public async Task<PagedList<CDPDto>> ObtenerCompromisosParaSolicitudRegistroPago(int usuarioId, int perfilId, int? terceroId, UserParams userParams)
        {
            IQueryable<CDPDto> lista = null;
            var usuario = await _context.Usuario.Where(x => x.UsuarioId == usuarioId).FirstOrDefaultAsync();


            if (perfilId == (int)PerfilUsuario.Administrador || perfilId == (int)PerfilUsuario.CoordinadorFinanciero)
            {
                lista = (from c in _context.CDP
                         join t in _context.Tercero on c.TerceroId equals t.TerceroId
                         where c.Instancia == (int)TipoDocumento.Compromiso
                         where c.SaldoActual > 0 //Saldo Disponible
                         where c.TerceroId == terceroId || terceroId == null
                         select new CDPDto()
                         {
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
                         join p in _context.ParametroLiquidacionTercero on t.TerceroId equals p.TerceroId into ParametroTercero
                         from pt in ParametroTercero.DefaultIfEmpty()
                         where c.Instancia == (int)TipoDocumento.Compromiso
                         where c.SaldoActual > 0 //Saldo Disponible
                         where pt.SupervisorId == usuarioId
                         where c.TerceroId == terceroId || terceroId == null
                         select new CDPDto()
                         {
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
                terceroId = usuario.TerceroId;

                lista = (from c in _context.CDP
                         join t in _context.Tercero on c.TerceroId equals t.TerceroId
                         where c.Instancia == (int)TipoDocumento.Compromiso
                         where c.SaldoActual > 0 //Saldo Disponible
                         where c.TerceroId == terceroId
                         select new CDPDto()
                         {
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
            var perfilId = 0;

            var usuarioPerfil = await _context.UsuarioPerfil.Where(x => x.UsuarioId == usuarioId).FirstOrDefaultAsync();
            if (usuarioPerfil != null)
            {
                perfilId = usuarioPerfil.PerfilId;
            }

            if (perfilId == (int)PerfilUsuario.Administrador)
            {
                lista = (from s in _context.FormatoSolicitudPago
                         join c in _context.CDP on s.Crp equals c.Crp
                         join t in _context.Tercero on c.TerceroId equals t.TerceroId
                         where s.EstadoId == (int)EstadoSolicitudPago.Generado
                         where c.Instancia == (int)TipoDocumento.Compromiso
                         where c.SaldoActual > 0 //Saldo Disponible                     
                         where c.TerceroId == terceroId || terceroId == null
                         select new CDPDto()
                         {
                             Crp = c.Crp,
                             Detalle4 = c.Detalle4.Length > 100 ? c.Detalle4.Substring(0, 100) + "..." : c.Detalle4,
                             NumeroIdentificacionTercero = t.NumeroIdentificacion,
                             NombreTercero = t.Nombre,
                             FormatoSolicitudPagoId = s.FormatoSolicitudPagoId
                         })
                        .Distinct()
                        .OrderBy(x => x.Crp);
            }
            else
            {
                lista = (from s in _context.FormatoSolicitudPago
                         join c in _context.CDP on s.Crp equals c.Crp
                         join t in _context.Tercero on c.TerceroId equals t.TerceroId
                         where s.SupervisorId == usuarioId
                         where s.EstadoId == (int)EstadoSolicitudPago.Generado
                         where c.Instancia == (int)TipoDocumento.Compromiso
                         where c.SaldoActual > 0 //Saldo Disponible                     
                         where c.TerceroId == terceroId || terceroId == null
                         select new CDPDto()
                         {
                             Crp = c.Crp,
                             Detalle4 = c.Detalle4.Length > 100 ? c.Detalle4.Substring(0, 100) + "..." : c.Detalle4,
                             NumeroIdentificacionTercero = t.NumeroIdentificacion,
                             NombreTercero = t.Nombre,
                             FormatoSolicitudPagoId = s.FormatoSolicitudPagoId
                         })
                        .Distinct()
                        .OrderBy(x => x.Crp);
            }

            return await PagedList<CDPDto>.CreateAsync(lista, userParams.PageNumber, userParams.PageSize);
        }

        public async Task<FormatoSolicitudPago> ObtenerFormatoSolicitudPagoBase(int formatoSolicitudPagoId)
        {
            return await _context.FormatoSolicitudPago.FirstOrDefaultAsync(u => u.FormatoSolicitudPagoId == formatoSolicitudPagoId);
        }

        public async Task<FormatoSolicitudPagoDto> ObtenerFormatoSolicitudPago(int crp)
        {
            var lista = (from c in _context.CDP
                         join t in _context.Tercero on c.TerceroId equals t.TerceroId
                         where c.Crp == crp
                         where c.Instancia == (int)TipoDocumento.Compromiso
                         select new CDPDto()
                         {
                             Cdp = c.Cdp,
                             Crp = c.Crp,
                             Fecha = c.Fecha, //Fecha compromiso
                             Detalle4 = c.Detalle4, //objeto contrato
                             TerceroId = c.TerceroId,
                             ValorInicial = c.ValorInicial,
                             Operacion = c.Operacion, //Valor adicion/reduccion
                             ValorTotal = c.ValorTotal, //valor actual
                             SaldoActual = c.SaldoActual, //saldo actual
                         });

            var listaAgrupada = (from i in lista
                                 group i by new
                                 {
                                     i.Cdp,
                                     i.Crp,
                                     i.Fecha,
                                     i.Detalle4,
                                     i.TerceroId,
                                     i.ValorInicial,
                                     i.Operacion,
                                     i.ValorTotal,
                                     i.SaldoActual
                                 }
                          into grp
                                 select new CDPDto()
                                 {
                                     Cdp = grp.Key.Cdp,
                                     Crp = grp.Key.Crp,
                                     Fecha = grp.Key.Fecha,
                                     Detalle4 = grp.Key.Detalle4,
                                     TerceroId = grp.Key.TerceroId,
                                     ValorInicial = grp.Sum(i => i.SaldoActual),
                                     Operacion = grp.Sum(i => i.Operacion),
                                     ValorTotal = grp.Sum(i => i.ValorTotal),
                                     SaldoActual = grp.Sum(i => i.SaldoActual)
                                 });

            var formato = await (from c in listaAgrupada
                                 join t in _context.Tercero on c.TerceroId equals t.TerceroId
                                 join co in _context.Contrato on c.Crp equals co.Crp into Contrato
                                 from ctr in Contrato.DefaultIfEmpty()
                                 join p in _context.ParametroLiquidacionTercero on t.TerceroId equals p.TerceroId into ParametroTercero
                                 from pt in ParametroTercero.DefaultIfEmpty()
                                 join sup in _context.Usuario on pt.SupervisorId equals sup.UsuarioId into Supervisor
                                 from super in Supervisor.DefaultIfEmpty()
                                 select new FormatoSolicitudPagoDto()
                                 {
                                     Cdp = new CDPDto()
                                     {
                                         Cdp = c.Cdp,
                                         Crp = c.Crp,
                                         Fecha = c.Fecha, //Fecha compromiso
                                         Detalle5 = super.Nombres + ' ' + super.Apellidos, //Supervisor
                                         SupervisorId = super.UsuarioId,
                                         Detalle4 = c.Detalle4, //objeto contrato
                                         ValorInicial = c.ValorInicial,
                                         Operacion = c.Operacion, //Valor adicion/reduccion
                                         ValorTotal = c.ValorTotal, //valor actual
                                         SaldoActual = c.SaldoActual, //saldo actual
                                     },
                                     Contrato = new Contrato()
                                     {
                                         ContratoId = ctr.ContratoId > 0 ? ctr.ContratoId : 0,
                                         NumeroContrato = ctr.ContratoId > 0 ? ctr.NumeroContrato : string.Empty,
                                         Crp = ctr.ContratoId > 0 ? ctr.Crp : 0,
                                         FechaInicio = ctr.ContratoId > 0 ? ctr.FechaInicio : _generalInterface.ObtenerFechaHoraActual(),
                                         FechaFinal = ctr.ContratoId > 0 ? ctr.FechaFinal : _generalInterface.ObtenerFechaHoraActual(),
                                         FechaRegistro = ctr.ContratoId > 0 ? ctr.FechaRegistro : _generalInterface.ObtenerFechaHoraActual(),
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
            var lista = (from s in _context.FormatoSolicitudPago
                         join c in _context.CDP on s.Crp equals c.Crp
                         join t in _context.Tercero on c.TerceroId equals t.TerceroId
                         where s.FormatoSolicitudPagoId == formatoSolicitudPagoId
                         where c.Instancia == (int)TipoDocumento.Compromiso
                         select new CDPDto()
                         {
                             Cdp = c.Cdp,
                             Crp = c.Crp,
                             Fecha = c.Fecha, //Fecha compromiso
                             Detalle4 = c.Detalle4, //objeto contrato
                             TerceroId = c.TerceroId,
                             ValorInicial = c.ValorInicial,
                             Operacion = c.Operacion, //Valor adicion/reduccion
                             ValorTotal = c.ValorTotal, //valor actual
                             SaldoActual = c.SaldoActual, //saldo actual
                         });

            var listaAgrupada = (from i in lista
                                 group i by new
                                 {
                                     i.Cdp,
                                     i.Crp,
                                     i.Fecha,
                                     i.Detalle4,
                                     i.TerceroId,
                                     i.ValorInicial,
                                     i.Operacion,
                                     i.ValorTotal,
                                     i.SaldoActual
                                 }
                          into grp
                                 select new CDPDto()
                                 {
                                     Cdp = grp.Key.Cdp,
                                     Crp = grp.Key.Crp,
                                     Fecha = grp.Key.Fecha,
                                     Detalle4 = grp.Key.Detalle4,
                                     TerceroId = grp.Key.TerceroId,
                                     ValorInicial = grp.Sum(i => i.SaldoActual),
                                     Operacion = grp.Sum(i => i.Operacion),
                                     ValorTotal = grp.Sum(i => i.ValorTotal),
                                     SaldoActual = grp.Sum(i => i.SaldoActual)
                                 });

            var formato = await (from s in _context.FormatoSolicitudPago
                                 join c in listaAgrupada on s.Crp equals c.Crp
                                 join t in _context.Tercero on c.TerceroId equals t.TerceroId
                                 join ae in _context.ActividadEconomica on s.ActividadEconomicaId equals ae.ActividadEconomicaId
                                 join pp in _context.PlanPago on s.PlanPagoId equals pp.PlanPagoId
                                 join co in _context.Contrato on c.Crp equals co.Crp into Contrato
                                 from ctro in Contrato.DefaultIfEmpty()
                                 join plt in _context.ParametroLiquidacionTercero on t.TerceroId equals plt.TerceroId into ParametroTercero
                                 from par in ParametroTercero.DefaultIfEmpty()
                                 join sup in _context.Usuario on s.SupervisorId equals sup.UsuarioId into Supervisor
                                 from super in Supervisor.DefaultIfEmpty()
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
                                         Cdp = c.Cdp,
                                         Crp = c.Crp,
                                         Fecha = c.Fecha, //Fecha compromiso
                                         Detalle5 = s.SupervisorId.HasValue ? (super.Nombres + ' ' + super.Apellidos) : string.Empty, //Supervisor
                                         SupervisorId = s.SupervisorId,
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
                                         ContratoId = ctro.ContratoId > 0 ? ctro.ContratoId : 0,
                                         NumeroContrato = ctro.ContratoId > 0 ? ctro.NumeroContrato : string.Empty,
                                         Crp = ctro.ContratoId > 0 ? ctro.Crp : 0,
                                         FechaInicio = ctro.ContratoId > 0 ? ctro.FechaInicio : System.DateTime.Now,
                                         FechaFinal = ctro.ContratoId > 0 ? ctro.FechaFinal : System.DateTime.Now,
                                         FechaRegistro = ctro.ContratoId > 0 ? ctro.FechaRegistro : System.DateTime.Now,
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
                                         FacturadorElectronicoDescripcion = t.FacturadorElectronico ? "SI" : "NO",
                                         ModalidadContrato = par.TerceroId > 0 ? par.ModalidadContrato : 0,
                                     },
                                     PlanPago = new PlanPagoDto()
                                     {
                                         PlanPagoId = pp.PlanPagoId,
                                         NumeroPago = pp.NumeroPago,
                                         ValorFacturado = pp.ValorFacturado.HasValue ? pp.ValorFacturado.Value : 0,
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

        #endregion Registro de Solicitud de Pago
        public async Task<ICollection<FormatoSolicitudPago>> ObtenerListaSolicitudPagoXPlanPagoIds(List<int> planPagoIds)
        {
            return await (from sp in _context.FormatoSolicitudPago
                          where planPagoIds.Contains(sp.PlanPagoId)
                          where sp.EstadoId == (int)EstadoSolicitudPago.Aprobado
                          select sp
                          ).ToListAsync();
        }

        public async Task<FormatoSolicitudPago> ObtenerSolicitudPagoXPlanPagoId(int planPagoId)
        {
            return await (from sp in _context.FormatoSolicitudPago
                          where sp.PlanPagoId == planPagoId
                          where sp.EstadoId == (int)EstadoSolicitudPago.Aprobado
                          select sp
                          ).FirstOrDefaultAsync();
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