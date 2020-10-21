using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ComplementApp.API.Models;
using Microsoft.EntityFrameworkCore;
using ComplementApp.API.Dtos;
using ComplementApp.API.Interfaces;

namespace ComplementApp.API.Data
{
    public class CDPRepository : ICDPRepository
    {

        private readonly DataContext _context;
        public CDPRepository(DataContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<CDP>> ObtenerListaCDP(int usuarioId)
        {

            var cdps = await (from c in _context.CDP
                              join d in _context.DetalleCDP on c.Cdp equals d.Cdp
                              where d.UsuarioId == usuarioId
                              where c.Instancia == (int)TipoDocumento.Cdp
                              group c by new
                              {
                                  c.Cdp,
                                  c.Fecha
                              } into g
                              select new CDP
                              {
                                  Cdp = g.Key.Cdp,
                                  Fecha = g.Key.Fecha
                              }).Distinct()
                              .ToListAsync();
            return cdps;
        }

        public async Task<CDPDto> ObtenerCDP(int usuarioId, int numeroCDP)
        {
            var cdp = await (from c in _context.CDP
                             join d in _context.DetalleCDP on c.Cdp equals d.Cdp
                             where d.UsuarioId == usuarioId
                             where c.Cdp == numeroCDP
                             where c.Instancia == (int)TipoDocumento.Cdp
                             select new CDPDto()
                             {
                                 Instancia = c.Instancia,
                                 Cdp = c.Cdp,
                                 Fecha = c.Fecha,
                                 Detalle1 = c.Detalle1, //Estado
                                 Detalle4 = c.Detalle4 //Objeto del bien
                             }).FirstOrDefaultAsync();
            return cdp;
        }

        public async Task<IEnumerable<DetalleCDPDto>> ObtenerDetalleDeCDP(int usuarioId, int numeroCDP)
        {
            if (numeroCDP > 0)
            {
                return await ObtenerRubrosPresupuestalesConCDP(usuarioId, numeroCDP);
            }
            else
            {
                return await ObtenerRubrosPresupuestalesSinCDP(usuarioId, numeroCDP);
            }
        }

        private async Task<ICollection<DetalleCDPDto>> ObtenerRubrosPresupuestalesConCDP(int usuarioId, int numeroCDP)
        {
            var detalles = await (from d in _context.DetalleCDP
                                  join i in _context.RubroPresupuestal on d.RubroPresupuestalId equals i.RubroPresupuestalId
                                  join dec in _context.RubroPresupuestal on d.DecretoId equals dec.RubroPresupuestalId
                                  join c in _context.CDP on d.Cdp equals c.Cdp
                                  join u in _context.Usuario on d.UsuarioId equals u.UsuarioId
                                  join ag in _context.ActividadGeneral on new { d.ActividadGeneralId } equals new { ag.ActividadGeneralId } into ActividadGeneralDetalle
                                  from ag in ActividadGeneralDetalle.DefaultIfEmpty()
                                  join ae in _context.ActividadEspecifica on new { d.ActividadEspecificaId } equals new { ae.ActividadEspecificaId } into ActividadEspecificaDetalle
                                  from ae in ActividadEspecificaDetalle.DefaultIfEmpty()
                                  join de in _context.Dependencia on new { d.DependenciaId } equals new { de.DependenciaId } into DependenciaDetalle
                                  from de in DependenciaDetalle.DefaultIfEmpty()
                                  join a in _context.Area on new { d.AreaId } equals new { a.AreaId } into AreaDetalle
                                  from a in AreaDetalle.DefaultIfEmpty()
                                  where d.RubroPresupuestalId == c.RubroPresupuestalId
                                  where c.Instancia == (int)TipoDocumento.Cdp
                                  where d.UsuarioId == usuarioId
                                  where d.Cdp == numeroCDP                                  
                                  select new DetalleCDPDto()
                                  {
                                      Id = d.DetalleCdpId,
                                      PcpId = d.PcpId,
                                      IdArchivo = d.IdArchivo,
                                      Cdp = d.Cdp,
                                      Proy = d.Proy,
                                      Prod = d.Prod,
                                      Proyecto = ag.Nombre,
                                      ActividadBpin = ae.Nombre,
                                      PlanDeCompras = d.PlanDeCompras,
                                      Responsable = u.Nombres + ' ' + u.Apellidos,
                                      Dependencia = de.Nombre,
                                      IdentificacionRubro = i.Identificacion,
                                      RubroNombre = i.Nombre,
                                      ValorCDP = c.ValorTotal,
                                      SaldoCDP = c.SaldoActual,
                                      ValorAct = d.ValorAct,
                                      SaldoAct = d.SaldoAct,                                     
                                      ValorRP = d.ValorRP,
                                      ValorOB = d.ValorOB,
                                      ValorOP = d.ValorOP,
                                      AplicaContrato = d.AplicaContrato,
                                      SaldoTotal = d.SaldoTotal,
                                      SaldoDisponible = d.SaldoDisponible,
                                      Area = a.Nombre,
                                      Valor_Convenio = d.Valor_Convenio,
                                      Convenio = d.Convenio,
                                      Decreto = dec.Identificacion
                                  })
                                 .Distinct()
                                 .OrderBy(x => x.IdentificacionRubro)
                                 .ToListAsync();

            return detalles;
        }

        //Para solicitud inicial no tiene CDP asociado
        private async Task<ICollection<DetalleCDPDto>> ObtenerRubrosPresupuestalesSinCDP(int usuarioId, int numeroCDP)
        {
            var detalles = await (from d in _context.DetalleCDP
                                  join u in _context.Usuario on d.UsuarioId equals u.UsuarioId
                                  join r in _context.RubroPresupuestal on d.RubroPresupuestalId equals r.RubroPresupuestalId
                                  join dec in _context.RubroPresupuestal on d.DecretoId equals dec.RubroPresupuestalId
                                  join ag in _context.ActividadGeneral on new { d.ActividadGeneralId } equals new { ag.ActividadGeneralId } into ActividadGeneralDetalle
                                  from ag in ActividadGeneralDetalle.DefaultIfEmpty()
                                  join ae in _context.ActividadEspecifica on new { d.ActividadEspecificaId } equals new { ae.ActividadEspecificaId } into ActividadEspecificaDetalle
                                  from ae in ActividadEspecificaDetalle.DefaultIfEmpty()
                                  join de in _context.Dependencia on new { d.DependenciaId } equals new { de.DependenciaId } into DependenciaDetalle
                                  from de in DependenciaDetalle.DefaultIfEmpty()
                                  join a in _context.Area on new { d.AreaId } equals new { a.AreaId } into AreaDetalle
                                  from a in AreaDetalle.DefaultIfEmpty()
                                  //join c in _context.CDP on new { d.Cdp, d.RubroPresupuestalId } equals new { c.Cdp, c.RubroPresupuestalId } into DetalleCDP_CDP
                                  //from c in DetalleCDP_CDP.DefaultIfEmpty()
                                  //where c.Instancia == (int)TipoDocumento.Cdp
                                  where d.UsuarioId == usuarioId
                                  //where c == null                                  
                                  where d.Cdp == numeroCDP
                                  
                                  select new DetalleCDPDto()
                                  {
                                      Id = d.DetalleCdpId,
                                      PcpId = d.PcpId,
                                      IdArchivo = d.IdArchivo,
                                      Cdp = d.Cdp,
                                      Proy = d.Proy,
                                      Prod = d.Prod,
                                      Proyecto = ag.Nombre,
                                      ActividadBpin = ae.Nombre,
                                      PlanDeCompras = d.PlanDeCompras,
                                      Responsable = u.Nombres + " " + u.Apellidos,
                                      Dependencia = de.Nombre,
                                      IdentificacionRubro = r.Identificacion,
                                      RubroNombre = r.Nombre,
                                      ValorAct = d.ValorAct,
                                      SaldoAct = d.SaldoAct,
                                      ValorRP = d.ValorRP,
                                      ValorOB = d.ValorOB,
                                      ValorOP = d.ValorOP,
                                      AplicaContrato = d.AplicaContrato,
                                      SaldoTotal = d.SaldoTotal,
                                      SaldoDisponible = d.SaldoDisponible,
                                      Rp = d.Rp,
                                      Area = a.Nombre,
                                      Valor_Convenio = d.Valor_Convenio,
                                      Convenio = d.Convenio,
                                      Decreto = dec.Identificacion
                                  })
                                 .Distinct()
                                 .OrderBy(x => x.IdentificacionRubro)
                                 .ToListAsync()
                                 ;
            return detalles;
        }


    }
}