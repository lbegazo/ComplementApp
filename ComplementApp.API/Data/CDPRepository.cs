using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ComplementApp.API.Models;
using Microsoft.EntityFrameworkCore;
using ComplementApp.API.Dtos;

namespace ComplementApp.API.Data
{
    public class CDPRepository : ICDPRepository
    {
        private readonly DataContext _context;
        public CDPRepository(DataContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<CDP>> ObtenerListaCDP(string usuario)
        {

            var cdps = await (from c in _context.CDP
                              join d in _context.DetalleCDP on c.Cdp equals d.Cdp
                              where d.Responsable == usuario
                              group c by new
                              {
                                  c.Cdp,
                                  c.Estado,
                                  c.Fecha
                              } into g
                              select new CDP
                              {
                                  Cdp = g.Key.Cdp,
                                  Estado = g.Key.Estado,
                                  Fecha = g.Key.Fecha
                              }).Distinct()
                              .ToListAsync();
            return cdps;
        }

        public async Task<CDP> ObtenerCDP(string usuario, int numeroCDP)
        {
            var cdp = await (from c in _context.CDP
                             join d in _context.DetalleCDP on c.Cdp equals d.Cdp
                             where d.Responsable == usuario
                             where c.Cdp == numeroCDP
                             select c).FirstOrDefaultAsync();
            return cdp;
        }

        public async Task<IEnumerable<DetalleCDPDto>> ObtenerDetalleDeCDP(string usuario, int numeroCDP)
        {            
            if(numeroCDP>0)
            {
                return await ObtenerRubrosPresupuestalesConCDP(usuario, numeroCDP);
            }
            else
            {
                return await ObtenerRubrosPresupuestalesSinCDP(usuario, numeroCDP);
            }            
        }

        private async Task<ICollection<DetalleCDPDto>> ObtenerRubrosPresupuestalesConCDP(string usuario, int numeroCDP)
        {
            var detalles = await (from d in _context.DetalleCDP
                                    join i in _context.RubroPresupuestal on d.Rubro equals i.Identificacion                        
                                    join c in _context.CDP on d.Cdp equals c.Cdp
                                 where d.Rubro == c.Rubro
                                 where d.Responsable == usuario
                                 where d.Cdp == numeroCDP
                                 select new DetalleCDPDto()
                                 {
                                     Id = d.Id,
                                     Crp = d.Crp,
                                     IdArchivo=d.IdArchivo,
                                     Cdp = d.Cdp,
                                     Proy = d.Proy,
                                     Prod = d.Prod,
                                     Proyecto = d.Proyecto,
                                     ActividadBpin = d.ActividadBpin,
                                     PlanDeCompras = d.PlanDeCompras,
                                     Responsable = d.Responsable,
                                     Dependencia = d.Dependencia,
                                     Rubro = d.Rubro,
                                     RubroDescripcion = i.Descripcion,
                                     ValorAct = d.ValorAct,
                                     SaldoAct = d.SaldoAct,
                                     ValorCDP = c.Valor,
                                     SaldoCDP = c.Saldo,
                                     ValorRP = d.ValorRP,
                                     ValorOB = d.ValorOB,
                                     ValorOP = d.ValorOP,
                                     Contrato = d.Contrato,
                                     SaldoTotal = d.SaldoTotal,
                                     SaldoDisponible = d.SaldoDisponible,
                                     Area = d.Area,
                                     Paa = d.Paa,
                                     IdSofi = d.IdSofi
                                 })
                                 .Distinct()
                                 .OrderBy(x => x.Rubro)
                                 .ToListAsync();

            return detalles;
        }

        //Para solicitud inicial no tiene CDP asociado
        private async Task<ICollection<DetalleCDPDto>> ObtenerRubrosPresupuestalesSinCDP(string usuario, int numeroCDP)
        {
            var detalles = await (from d in _context.DetalleCDP
                                    join i in _context.RubroPresupuestal on d.Rubro equals i.Identificacion                        
                                    join c in _context.CDP on new {d.Cdp, d.Rubro} equals new {c.Cdp, c.Rubro } into DetalleCDP_CDP
                                    from c in DetalleCDP_CDP.DefaultIfEmpty()
                                 where c == null                                                                  
                                 where d.Responsable == usuario
                                 where d.Cdp == numeroCDP
                                 select new DetalleCDPDto()
                                 {
                                     Id = d.Id,
                                     Crp = d.Crp,
                                     IdArchivo=d.IdArchivo,
                                     Cdp = d.Cdp,
                                     Proy = d.Proy,
                                     Prod = d.Prod,
                                     Proyecto = d.Proyecto,
                                     ActividadBpin = d.ActividadBpin,
                                     PlanDeCompras = d.PlanDeCompras,
                                     Responsable = d.Responsable,
                                     Dependencia = d.Dependencia,
                                     Rubro = d.Rubro,
                                     RubroDescripcion = i.Descripcion,
                                     ValorAct = d.ValorAct,
                                     SaldoAct = d.SaldoAct,
                                     ValorCDP = c.Valor,
                                     SaldoCDP = c.Saldo,
                                     ValorRP = d.ValorRP,
                                     ValorOB = d.ValorOB,
                                     ValorOP = d.ValorOP,
                                     Contrato = d.Contrato,
                                     SaldoTotal = d.SaldoTotal,
                                     SaldoDisponible = d.SaldoDisponible,
                                     Area = d.Area,
                                     Paa = d.Paa,
                                     IdSofi = d.IdSofi
                                 })
                                 .Distinct()
                                 .OrderBy(x => x.Rubro)
                                 .ToListAsync()
                                 ;
            return detalles;
        }


    }
}