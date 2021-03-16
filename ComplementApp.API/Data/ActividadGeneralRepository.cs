using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ComplementApp.API.Helpers;
using ComplementApp.API.Interfaces;
using ComplementApp.API.Models;
using Microsoft.EntityFrameworkCore;

namespace ComplementApp.API.Data
{
    public class ActividadGeneralRepository : IActividadGeneralRepository
    {
        private readonly DataContext _context;
        public ActividadGeneralRepository(DataContext context)
        {
            _context = context;
        }

        #region Tercero
        public async Task<ActividadGeneral> ObtenerActividadGeneralBase(int id)
        {
            return await _context.ActividadGeneral
                        .FirstOrDefaultAsync(u => u.ActividadGeneralId == id);
        }

        public async Task<ICollection<ActividadGeneral>> ObtenerActividadesGenerales()
        {

            var lista = await ((from rp in _context.RubroPresupuestal
                                join ag in _context.ActividadGeneral on rp.RubroPresupuestalId equals ag.RubroPresupuestalId into RubroActividadGeneral
                                from ruAcGe in RubroActividadGeneral.DefaultIfEmpty()
                                where rp.PadreRubroId == 0
                                select new ActividadGeneral()
                                {
                                    RubroPresupuestal = new RubroPresupuestal()
                                    {
                                        RubroPresupuestalId = rp.RubroPresupuestalId,
                                        Identificacion = rp.Identificacion,
                                        Nombre = rp.Nombre.Length > 130 ? rp.Nombre.Substring(0, 130) + "..." : rp.Nombre,
                                        PadreRubroId = 0,
                                    },
                                    ActividadGeneralId = ruAcGe.ActividadGeneralId > 0 ? ruAcGe.ActividadGeneralId : 0,
                                    ApropiacionDisponible = ruAcGe.ActividadGeneralId > 0 ? ruAcGe.ApropiacionDisponible : 0,
                                    ApropiacionVigente = ruAcGe.ActividadGeneralId > 0 ? ruAcGe.ApropiacionVigente : 0,
                                }).Distinct()
                            .OrderBy(t => t.RubroPresupuestal.Identificacion))
                            .ToListAsync();

            return lista;
        }

        #endregion Tercero
    }
}