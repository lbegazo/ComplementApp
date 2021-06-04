using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
        
        public async Task<ICollection<PlanAdquisicion>> ObtenerListaPlanAnualAdquisicion(int pciId)
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
                                select new PlanAdquisicion()
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
    }
}