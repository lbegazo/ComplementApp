using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ComplementApp.API.Dtos;
using ComplementApp.API.Helpers;
using ComplementApp.API.Interfaces.Repository;
using ComplementApp.API.Models;
using Microsoft.EntityFrameworkCore;

namespace ComplementApp.API.Data
{
    public class PlanAdquisicionRepository : IPlanAdquisicionRepository
    {
        private readonly DataContext _context;
        public PlanAdquisicionRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<ICollection<PlanAdquisicionDto>> ObtenerListaPlanAnualAdquisicion(int pciId)
        {
            var listaCdp = (from c in _context.CDP
                            where c.PciId == pciId
                            where c.Instancia == (int)TipoDocumento.Compromiso
                            group c by new
                            {
                                c.Crp,
                                c.Detalle4,
                                c.SaldoActual
                            } into g
                            select new CDP
                            {
                                Crp = g.Key.Crp,
                                Detalle4 = g.Key.Detalle4,
                                SaldoActual = g.Key.SaldoActual,
                            }).OrderBy(t => t.Crp); ;

            var lista = await ((from d in _context.PlanAdquisicion
                                join rp in _context.RubroPresupuestal on d.RubroPresupuestalId equals rp.RubroPresupuestalId
                                join ag in _context.ActividadGeneral on d.ActividadGeneralId equals ag.ActividadGeneralId
                                join ae in _context.ActividadEspecifica on d.ActividadEspecificaId equals ae.ActividadEspecificaId
                                join rpae in _context.RubroPresupuestal on ae.RubroPresupuestalId equals rpae.RubroPresupuestalId
                                join c in listaCdp on d.Crp equals c.Crp into CDP
                                from cc in CDP.DefaultIfEmpty()
                                where d.PciId == ag.PciId
                                where d.PciId == ae.PciId
                                where ag.ActividadGeneralId == ae.ActividadGeneralId
                                where ag.PciId == ae.PciId
                                where ae.PciId == pciId
                                select new PlanAdquisicionDto()
                                {
                                    PlanAdquisicionId = d.PlanAdquisicionId,
                                    PlanDeCompras = d.PlanDeCompras,
                                    ValorAct = d.ValorAct,
                                    SaldoAct = d.SaldoAct,
                                    UsuarioId = d.UsuarioId,
                                    DependenciaId = d.DependenciaId,
                                    AplicaContrato = d.AplicaContrato,
                                    ActividadGeneral = new ActividadGeneral()
                                    {
                                        ActividadGeneralId = d.ActividadGeneralId,
                                    },
                                    ActividadEspecifica = new ActividadEspecifica()
                                    {
                                        ActividadEspecificaId = ae.ActividadEspecificaId,
                                        Nombre = ae.Nombre,
                                        ValorApropiacionVigente = ae.ValorApropiacionVigente,
                                        SaldoPorProgramar = ae.SaldoPorProgramar,
                                        RubroPresupuestal = new RubroPresupuestal()
                                        {
                                            RubroPresupuestalId = rpae.RubroPresupuestalId,
                                            Identificacion = rpae.Identificacion,
                                            Nombre = rpae.Nombre,
                                            PadreRubroId = 0,
                                        }
                                    },
                                    RubroPresupuestal = new RubroPresupuestal()
                                    {
                                        RubroPresupuestalId = rp.RubroPresupuestalId,
                                        Identificacion = rp.Identificacion,
                                        Nombre = rp.Nombre,
                                        PadreRubroId = 0,
                                    },
                                    CdpDocumento = new CDP()
                                    {
                                        Crp = d.Crp > 0 ? cc.Crp : 0,
                                        Detalle4 = d.Crp > 0 ? cc.Detalle4 : string.Empty,
                                        SaldoActual = d.Crp > 0 ? cc.SaldoActual : 0,
                                    }
                                })
                                .Distinct()
                                .OrderBy(t => t.RubroPresupuestal.Identificacion))
                                .ToListAsync();



            return lista;
        }

        public async Task<PlanAdquisicion> ObtenerPlanAnualAdquisicionBase(int id)
        {
            return await _context.PlanAdquisicion
                                    .FirstOrDefaultAsync(u => u.PlanAdquisicionId == id);
        }

        public async Task<PagedList<DetalleCDPDto>> ObtenerListaPlanAnualAdquisicionPaginada(int usuarioId, int esCreacion, int? rubroPresupuestalId,
                                                                                             int? numeroCdp, UserParams userParams)
        {
            var cdp = esCreacion > 0 ? 0 : numeroCdp;
            if (esCreacion > 0)
            {
                return await ObtenerListaPlanAnualAdquisicionSinCDP(usuarioId, rubroPresupuestalId, userParams);
            }
            else
            {
                return await ObtenerListaPlanAnualAdquisicionConCDP(usuarioId, cdp.Value, userParams);
            }
        }

        private async Task<PagedList<DetalleCDPDto>> ObtenerListaPlanAnualAdquisicionConCDP(int usuarioId, int numeroCdp, UserParams userParams)
        {
            var lista = (from d in _context.PlanAdquisicion
                         join i in _context.RubroPresupuestal on d.RubroPresupuestalId equals i.RubroPresupuestalId
                         join dec in _context.RubroPresupuestal on d.DecretoId equals dec.RubroPresupuestalId
                         join c in _context.CDP on d.Cdp equals c.Cdp
                         join u in _context.Usuario on d.UsuarioId equals u.UsuarioId
                         join ag in _context.ActividadGeneral on new { d.ActividadGeneralId } equals new { ag.ActividadGeneralId } into ActividadGeneralDetalle
                         from acGe in ActividadGeneralDetalle.DefaultIfEmpty()
                         join rp in _context.RubroPresupuestal on acGe.RubroPresupuestalId equals rp.RubroPresupuestalId into RubroActividadGeneral
                         from ruAG in RubroActividadGeneral.DefaultIfEmpty()
                         join ae in _context.ActividadEspecifica on new { d.ActividadEspecificaId } equals new { ae.ActividadEspecificaId } into ActividadEspecificaDetalle
                         from acEs in ActividadEspecificaDetalle.DefaultIfEmpty()
                         join de in _context.Dependencia on new { d.DependenciaId } equals new { de.DependenciaId } into DependenciaDetalle
                         from de in DependenciaDetalle.DefaultIfEmpty()
                         join a in _context.Area on new { d.AreaId } equals new { a.AreaId } into AreaDetalle
                         from a in AreaDetalle.DefaultIfEmpty()
                         where c.PciId == acGe.PciId
                         where c.PciId == acEs.PciId
                         where c.PciId == u.PciId
                         where d.RubroPresupuestalId == c.RubroPresupuestalId
                         where c.Instancia == (int)TipoDocumento.Cdp
                         where d.UsuarioId == usuarioId
                         where d.Cdp == numeroCdp
                         select new DetalleCDPDto()
                         {
                             DetalleCdpId = d.PlanAdquisicionId,
                             PcpId = d.PcpId,
                             IdArchivo = d.IdArchivo,
                             Cdp = d.Cdp,
                             Proy = d.Proy,
                             Prod = d.Prod,
                             Proyecto = ruAG.Nombre,
                             ActividadBpin = acEs.Nombre,
                             PlanDeCompras = d.PlanDeCompras,
                             Responsable = u.Nombres + ' ' + u.Apellidos,
                             Dependencia = de.Nombre,
                             ValorCDP = c.ValorTotal,
                             SaldoCDP = c.SaldoActual,
                             ValorAct = d.ValorAct,
                             SaldoAct = d.SaldoAct,
                             ValorRP = d.ValorRP,
                             ValorOB = d.ValorOB,
                             ValorOP = d.ValorOP,
                             AplicaContratoDescripcion = d.AplicaContrato ? "SI" : "NO",
                             SaldoTotal = d.SaldoTotal,
                             SaldoDisponible = d.SaldoDisponible,
                             Area = a.Nombre,
                             Valor_Convenio = d.Valor_Convenio,
                             Convenio = d.Convenio,
                             Decreto = dec.Identificacion,
                             RubroPresupuestal = new RubroPresupuestal()
                             {
                                 RubroPresupuestalId = i.RubroPresupuestalId,
                                 Identificacion = i.Identificacion,
                                 Nombre = i.Nombre,
                             }
                         })
                        .Distinct()
                        .OrderBy(x => x.RubroPresupuestal.Identificacion);

            return await PagedList<DetalleCDPDto>.CreateAsync(lista, userParams.PageNumber, userParams.PageSize);
        }

        //Para solicitud inicial no tiene CDP asociado
        public async Task<PagedList<DetalleCDPDto>> ObtenerListaPlanAnualAdquisicionSinCDP(int usuarioId, int? rubroPresupuestalId, UserParams userParams)
        {
            IOrderedQueryable<DetalleCDPDto> lista = null;
            var listaPerfilxUsuario = await _context.UsuarioPerfil.Where(x => x.UsuarioId == usuarioId).ToListAsync();

            if (listaPerfilxUsuario != null && listaPerfilxUsuario.Count > 0)
            {
                var listaPerfilId = listaPerfilxUsuario.Select(x => x.PerfilId);

                if (listaPerfilId.Contains((int)PerfilUsuario.Administrador) || listaPerfilId.Contains((int)PerfilUsuario.CoordinadorFinanciero))
                {
                    #region Administrador o Coordinador Financiero

                    lista = (from d in _context.PlanAdquisicion
                             join u in _context.Usuario on d.UsuarioId equals u.UsuarioId
                             join r in _context.RubroPresupuestal on d.RubroPresupuestalId equals r.RubroPresupuestalId
                             join dec in _context.RubroPresupuestal on d.DecretoId equals dec.RubroPresupuestalId
                             join ag in _context.ActividadGeneral on new { d.ActividadGeneralId } equals new { ag.ActividadGeneralId } into ActividadGeneralDetalle
                             from acGe in ActividadGeneralDetalle.DefaultIfEmpty()
                             join rp in _context.RubroPresupuestal on acGe.RubroPresupuestalId equals rp.RubroPresupuestalId into RubroActividadGeneral
                             from rpAG in RubroActividadGeneral.DefaultIfEmpty()
                             join ae in _context.ActividadEspecifica on new { d.ActividadEspecificaId } equals new { ae.ActividadEspecificaId } into ActividadEspecificaDetalle
                             from acEs in ActividadEspecificaDetalle.DefaultIfEmpty()
                             join de in _context.Dependencia on new { d.DependenciaId } equals new { de.DependenciaId } into DependenciaDetalle
                             from de in DependenciaDetalle.DefaultIfEmpty()
                             join a in _context.Area on new { d.AreaId } equals new { a.AreaId } into AreaDetalle
                             from a in AreaDetalle.DefaultIfEmpty()
                             where d.RubroPresupuestalId == rubroPresupuestalId || rubroPresupuestalId == null
                             where d.PciId == u.PciId
                             where d.PciId == acGe.PciId
                             where d.PciId == acEs.PciId
                             where d.PciId == a.PciId
                             where d.PciId == userParams.PciId
                             where d.Cdp == 0
                             where d.SaldoAct > 0

                             select new DetalleCDPDto()
                             {
                                 DetalleCdpId = d.PlanAdquisicionId,
                                 PcpId = d.PcpId,
                                 IdArchivo = d.IdArchivo,
                                 Cdp = d.Cdp,
                                 Proy = d.Proy,
                                 Prod = d.Prod,
                                 Proyecto = rpAG.Nombre,
                                 ActividadBpin = acEs.Nombre,
                                 PlanDeCompras = d.PlanDeCompras,
                                 Responsable = u.Nombres + " " + u.Apellidos,
                                 Dependencia = de.Nombre,
                                 ValorAct = d.ValorAct,
                                 SaldoAct = d.SaldoAct,
                                 ValorRP = d.ValorRP,
                                 ValorOB = d.ValorOB,
                                 ValorOP = d.ValorOP,
                                 AplicaContratoDescripcion = d.AplicaContrato ? "SI" : "NO",
                                 SaldoTotal = d.SaldoTotal,
                                 SaldoDisponible = d.SaldoDisponible,
                                 Crp = d.Crp,
                                 Area = a.Nombre,
                                 Valor_Convenio = d.Valor_Convenio,
                                 Convenio = d.Convenio,
                                 Decreto = dec.Identificacion,
                                 RubroPresupuestal = new RubroPresupuestal()
                                 {
                                     RubroPresupuestalId = r.RubroPresupuestalId,
                                     Identificacion = r.Identificacion,
                                     Nombre = r.Nombre,
                                 }
                             })
                           .Distinct()
                           .OrderBy(x => x.RubroPresupuestal.Identificacion);
                    #endregion Administrador o Coordinador Financiero
                }
                else
                {
                    #region Usuario

                    lista = (from d in _context.PlanAdquisicion
                             join u in _context.Usuario on d.UsuarioId equals u.UsuarioId
                             join r in _context.RubroPresupuestal on d.RubroPresupuestalId equals r.RubroPresupuestalId
                             join dec in _context.RubroPresupuestal on d.DecretoId equals dec.RubroPresupuestalId
                             join ag in _context.ActividadGeneral on new { d.ActividadGeneralId } equals new { ag.ActividadGeneralId } into ActividadGeneralDetalle
                             from acGe in ActividadGeneralDetalle.DefaultIfEmpty()
                             join rp in _context.RubroPresupuestal on acGe.RubroPresupuestalId equals rp.RubroPresupuestalId into RubroActividadGeneral
                             from rpAG in RubroActividadGeneral.DefaultIfEmpty()
                             join ae in _context.ActividadEspecifica on new { d.ActividadEspecificaId } equals new { ae.ActividadEspecificaId } into ActividadEspecificaDetalle
                             from acEs in ActividadEspecificaDetalle.DefaultIfEmpty()
                             join de in _context.Dependencia on new { d.DependenciaId } equals new { de.DependenciaId } into DependenciaDetalle
                             from de in DependenciaDetalle.DefaultIfEmpty()
                             join a in _context.Area on new { d.AreaId } equals new { a.AreaId } into AreaDetalle
                             from a in AreaDetalle.DefaultIfEmpty()
                             where d.UsuarioId == usuarioId
                             where d.RubroPresupuestalId == rubroPresupuestalId || rubroPresupuestalId == null
                             where d.PciId == u.PciId
                             where d.PciId == acGe.PciId
                             where d.PciId == acEs.PciId
                             where d.PciId == a.PciId
                             where d.PciId == userParams.PciId
                             where d.Cdp == 0

                             select new DetalleCDPDto()
                             {
                                 DetalleCdpId = d.PlanAdquisicionId,
                                 PcpId = d.PcpId,
                                 IdArchivo = d.IdArchivo,
                                 Cdp = d.Cdp,
                                 Proy = d.Proy,
                                 Prod = d.Prod,
                                 Proyecto = rpAG.Nombre,
                                 ActividadBpin = acEs.Nombre,
                                 PlanDeCompras = d.PlanDeCompras,
                                 Responsable = u.Nombres + " " + u.Apellidos,
                                 Dependencia = de.Nombre,
                                 ValorAct = d.ValorAct,
                                 SaldoAct = d.SaldoAct,
                                 ValorRP = d.ValorRP,
                                 ValorOB = d.ValorOB,
                                 ValorOP = d.ValorOP,
                                 AplicaContratoDescripcion = d.AplicaContrato ? "SI" : "NO",
                                 SaldoTotal = d.SaldoTotal,
                                 SaldoDisponible = d.SaldoDisponible,
                                 Crp = d.Crp,
                                 Area = a.Nombre,
                                 Valor_Convenio = d.Valor_Convenio,
                                 Convenio = d.Convenio,
                                 Decreto = dec.Identificacion,
                                 RubroPresupuestal = new RubroPresupuestal()
                                 {
                                     RubroPresupuestalId = r.RubroPresupuestalId,
                                     Identificacion = r.Identificacion,
                                     Nombre = r.Nombre,
                                 }
                             })
                               .Distinct()
                               .OrderBy(x => x.RubroPresupuestal.Identificacion);

                    #endregion Usuario
                }
            }


            return await PagedList<DetalleCDPDto>.CreateAsync(lista, userParams.PageNumber, userParams.PageSize);
        }

        public async Task<List<DetalleCDPDto>> ObtenerListaPlanAdquisicionSinCDPXIds(List<int> listaId)
        {
            var lista = await (from d in _context.PlanAdquisicion
                               join u in _context.Usuario on d.UsuarioId equals u.UsuarioId
                               join r in _context.RubroPresupuestal on d.RubroPresupuestalId equals r.RubroPresupuestalId
                               join dec in _context.RubroPresupuestal on d.DecretoId equals dec.RubroPresupuestalId
                               join ag in _context.ActividadGeneral on new { d.ActividadGeneralId } equals new { ag.ActividadGeneralId } into ActividadGeneralDetalle
                               from acGe in ActividadGeneralDetalle.DefaultIfEmpty()
                               join rp in _context.RubroPresupuestal on acGe.RubroPresupuestalId equals rp.RubroPresupuestalId into RubroActividadGeneral
                               from rpAG in RubroActividadGeneral.DefaultIfEmpty()
                               join ae in _context.ActividadEspecifica on new { d.ActividadEspecificaId } equals new { ae.ActividadEspecificaId } into ActividadEspecificaDetalle
                               from acEs in ActividadEspecificaDetalle.DefaultIfEmpty()
                               join de in _context.Dependencia on new { d.DependenciaId } equals new { de.DependenciaId } into DependenciaDetalle
                               from de in DependenciaDetalle.DefaultIfEmpty()
                               join a in _context.Area on new { d.AreaId } equals new { a.AreaId } into AreaDetalle
                               from a in AreaDetalle.DefaultIfEmpty()
                               where d.PciId == u.PciId
                               where d.PciId == acGe.PciId
                               where d.PciId == acEs.PciId
                               where d.PciId == a.PciId
                               where listaId.Contains(d.PlanAdquisicionId)
                               select new DetalleCDPDto()
                               {
                                   DetalleCdpId = d.PlanAdquisicionId,
                                   PcpId = d.PcpId,
                                   IdArchivo = d.IdArchivo,
                                   Cdp = d.Cdp,
                                   Proy = d.Proy,
                                   Prod = d.Prod,
                                   Proyecto = rpAG.Nombre,
                                   ActividadBpin = acEs.Nombre,
                                   PlanDeCompras = d.PlanDeCompras,
                                   Responsable = u.Nombres + " " + u.Apellidos,
                                   Dependencia = de.Nombre,
                                   ValorAct = d.ValorAct,
                                   SaldoAct = d.SaldoAct,
                                   ValorRP = d.ValorRP,
                                   ValorOB = d.ValorOB,
                                   ValorOP = d.ValorOP,
                                   AplicaContratoDescripcion = d.AplicaContrato ? "SI" : "NO",
                                   SaldoTotal = d.SaldoTotal,
                                   SaldoDisponible = d.SaldoDisponible,
                                   Crp = d.Crp,
                                   Area = a.Nombre,
                                   Valor_Convenio = d.Valor_Convenio,
                                   Convenio = d.Convenio,
                                   Decreto = dec.Identificacion,
                                   RubroPresupuestal = new RubroPresupuestal()
                                   {
                                       RubroPresupuestalId = r.RubroPresupuestalId,
                                       Identificacion = r.Identificacion,
                                       Nombre = r.Nombre,
                                   }
                               })
                        .Distinct()
                        .OrderBy(x => x.RubroPresupuestal.Identificacion)
                        .ToListAsync();

            return lista;
        }

        public async Task<PagedList<DetalleCDPDto>> ObtenerListaPlanAdquisicionReporte(int usuarioId, int? rubroPresupuestalId, UserParams userParams)
        {
            IOrderedQueryable<DetalleCDPDto> lista = null;
            var listaPerfilxUsuario = await _context.UsuarioPerfil.Where(x => x.UsuarioId == usuarioId).ToListAsync();

            if (listaPerfilxUsuario != null && listaPerfilxUsuario.Count > 0)
            {
                var listaPerfilId = listaPerfilxUsuario.Select(x => x.PerfilId);

                if (listaPerfilId.Contains((int)PerfilUsuario.Administrador) ||
                    listaPerfilId.Contains((int)PerfilUsuario.CoordinadorFinanciero))
                {
                    #region Administrador o Coordinador Financiero

                    lista = (from d in _context.PlanAdquisicion
                             join u in _context.Usuario on d.UsuarioId equals u.UsuarioId
                             join de in _context.Dependencia on new { d.DependenciaId } equals new { de.DependenciaId } into DependenciaDetalle
                             from de in DependenciaDetalle.DefaultIfEmpty()
                             join a in _context.Area on new { d.AreaId } equals new { a.AreaId } into AreaDetalle
                             from a in AreaDetalle.DefaultIfEmpty()
                             where d.PciId == u.PciId
                             //where c.Instancia == (int)TipoDocumento.Cdp
                             where d.PciId == userParams.PciId
                             where d.RubroPresupuestalId == rubroPresupuestalId || rubroPresupuestalId == null

                             select new DetalleCDPDto()
                             {
                                 DetalleCdpId = d.PlanAdquisicionId,
                                 PlanDeCompras = d.PlanDeCompras,
                                 Cdp = d.Cdp,

                                 ValorAct = d.ValorAct,
                                 ValorModif = 0,
                                 SaldoAct = d.SaldoAct,
                                 ValorCDP = d.ValorCDP,
                                 ValorRP = d.ValorRP,
                                 ValorOB = d.ValorOB,
                                 ValorOP = d.ValorOP,

                                 Responsable = d.UsuarioId > 0 ? u.Nombres + ' ' + u.Apellidos : string.Empty,
                                 Dependencia = d.DependenciaId > 0 ? de.Nombre : string.Empty,
                                 AplicaContratoDescripcion = d.AplicaContrato ? "SI" : "NO",
                                 Area = d.AreaId > 0 ? a.Nombre : string.Empty,
                             })
                                .Distinct()
                                .OrderBy(x => x.Cdp);

                    #endregion Administrador o Coordinador Financiero
                }
                else
                {
                    #region Usuario

                    lista = (from d in _context.PlanAdquisicion
                             join u in _context.Usuario on d.UsuarioId equals u.UsuarioId
                             join de in _context.Dependencia on new { d.DependenciaId } equals new { de.DependenciaId } into DependenciaDetalle
                             from de in DependenciaDetalle.DefaultIfEmpty()
                             join a in _context.Area on new { d.AreaId } equals new { a.AreaId } into AreaDetalle
                             from a in AreaDetalle.DefaultIfEmpty()
                             where d.PciId == u.PciId
                             //where c.Instancia == (int)TipoDocumento.Cdp
                             where d.UsuarioId == usuarioId
                             where d.PciId == userParams.PciId
                             where d.RubroPresupuestalId == rubroPresupuestalId || rubroPresupuestalId == null

                             select new DetalleCDPDto()
                             {
                                 DetalleCdpId = d.PlanAdquisicionId,
                                 PlanDeCompras = d.PlanDeCompras,
                                 Cdp = d.Cdp,

                                 ValorAct = d.ValorAct,
                                 ValorModif = 0,
                                 SaldoAct = d.SaldoAct,
                                 ValorCDP = d.ValorCDP,
                                 ValorRP = d.ValorRP,
                                 ValorOB = d.ValorOB,
                                 ValorOP = d.ValorOP,

                                 Responsable = d.UsuarioId > 0 ? u.Nombres + ' ' + u.Apellidos : string.Empty,
                                 Dependencia = d.DependenciaId > 0 ? de.Nombre : string.Empty,
                                 AplicaContratoDescripcion = d.AplicaContrato ? "SI" : "NO",
                                 Area = d.AreaId > 0 ? a.Nombre : string.Empty,
                             })
                                .Distinct()
                                .OrderBy(x => x.Cdp);

                    #endregion Usuario
                }
            }

            return await PagedList<DetalleCDPDto>.CreateAsync(lista, userParams.PageNumber, userParams.PageSize);
        }
    }
}