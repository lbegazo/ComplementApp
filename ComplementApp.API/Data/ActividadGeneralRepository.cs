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

        public async Task<ICollection<ActividadGeneralDto>> ObtenerActividadesGenerales(int pciId)
        {

            var lista = await ((from rp in _context.RubroPresupuestal
                                join ag in _context.ActividadGeneral on rp.RubroPresupuestalId equals ag.RubroPresupuestalId into RubroActividadGeneral
                                from ruAcGe in RubroActividadGeneral.DefaultIfEmpty()
                                join ff in _context.FuenteFinanciacion on ruAcGe.FuenteFinanciacionId equals ff.FuenteFinanciacionId into FuenteFinanciacion
                                from fuente in FuenteFinanciacion.DefaultIfEmpty()
                                join sf in _context.SituacionFondo on ruAcGe.SituacionFondoId equals sf.SituacionFondoId into SituacionFondo
                                from situacion in SituacionFondo.DefaultIfEmpty()
                                join re in _context.RecursoPresupuestal on ruAcGe.RecursoPresupuestalId equals re.RecursoPresupuestalId into RecursoPresupuestal
                                from recurso in RecursoPresupuestal.DefaultIfEmpty()
                                where ruAcGe.PciId == pciId
                                where rp.PadreRubroId == 0
                                select new ActividadGeneralDto()
                                {
                                    RubroPresupuestal = new RubroPresupuestal()
                                    {
                                        RubroPresupuestalId = rp.RubroPresupuestalId,
                                        Identificacion = rp.Identificacion,
                                        Nombre = rp.Nombre.Length > 100 ? rp.Nombre.Substring(0, 100) + "..." : rp.Nombre,
                                        PadreRubroId = 0,
                                    },
                                    ActividadGeneralId = ruAcGe.ActividadGeneralId > 0 ? ruAcGe.ActividadGeneralId : 0,
                                    ApropiacionDisponible = ruAcGe.ActividadGeneralId > 0 ? ruAcGe.ApropiacionDisponible : 0,
                                    ApropiacionVigente = ruAcGe.ActividadGeneralId > 0 ? ruAcGe.ApropiacionVigente : 0,
                                    FuenteFinanciacion = new ValorSeleccion()
                                    {
                                        Codigo = fuente.Codigo,
                                        Nombre = fuente.Nombre
                                    },
                                    SituacionFondo = new ValorSeleccion()
                                    {
                                        Codigo = situacion.Codigo,
                                        Nombre = situacion.Nombre,
                                    },
                                    RecursoPresupuestal = new ValorSeleccion()
                                    {
                                        Codigo = recurso.Codigo,
                                        Nombre = recurso.Nombre,
                                    }
                                }).Distinct()
                            .OrderBy(t => t.RubroPresupuestal.Identificacion))
                            .ToListAsync();

            return lista;
        }

        public async Task<ActividadEspecifica> ObtenerActividadEspecificaBase(int id)
        {
            return await _context.ActividadEspecifica
                        .FirstOrDefaultAsync(u => u.ActividadEspecificaId == id);
        }

        public async Task<ICollection<ActividadEspecifica>> ObtenerActividadesEspecificas(int pciId)
        {
            var lista = await ((from ae in _context.ActividadEspecifica
                                join ag in _context.ActividadGeneral on ae.ActividadGeneralId equals ag.ActividadGeneralId
                                join rp in _context.RubroPresupuestal on ae.RubroPresupuestalId equals rp.RubroPresupuestalId
                                where ae.PciId == ag.PciId
                                where ae.PciId == pciId
                                //where rp.PadreRubroId == 0
                                select new ActividadEspecifica()
                                {
                                    ActividadEspecificaId = ae.ActividadEspecificaId,
                                    Nombre = ae.Nombre,
                                    ValorApropiacionVigente = ae.ValorApropiacionVigente,
                                    SaldoPorProgramar = ae.SaldoPorProgramar,
                                    RubroPresupuestal = new RubroPresupuestal()
                                    {
                                        RubroPresupuestalId = rp.RubroPresupuestalId,
                                        Identificacion = rp.Identificacion,
                                        Nombre = rp.Nombre.Length > 100 ? rp.Nombre.Substring(0, 100) + "..." : rp.Nombre,
                                        PadreRubroId = 0,
                                    },
                                    ActividadGeneral = new ActividadGeneral()
                                    {
                                        ActividadGeneralId = ag.ActividadGeneralId,
                                        ApropiacionDisponible = ag.ApropiacionDisponible,
                                        ApropiacionVigente = ag.ApropiacionVigente,
                                        RubroPresupuestal = new RubroPresupuestal()
                                        {
                                            RubroPresupuestalId = rp.RubroPresupuestalId,
                                            Identificacion = rp.Identificacion,
                                            Nombre = rp.Nombre.Length > 100 ? rp.Nombre.Substring(0, 100) + "..." : rp.Nombre,
                                            PadreRubroId = 0,
                                        },
                                    }
                                }).Distinct()
                            .OrderBy(t => t.RubroPresupuestal.Identificacion))
                            .ToListAsync();

            return lista;
        }

        public async Task<PagedList<ActividadEspecifica>> ObtenerListaActividadEspecifica(UserParams userParams)
        {
            var lista = ((from ae in _context.ActividadEspecifica
                          join ag in _context.ActividadGeneral on ae.ActividadGeneralId equals ag.ActividadGeneralId
                          join rp in _context.RubroPresupuestal on ae.RubroPresupuestalId equals rp.RubroPresupuestalId
                          where ae.PciId == ag.PciId
                          where ae.PciId == userParams.PciId
                          where ae.SaldoPorProgramar > 0
                          select new ActividadEspecifica()
                          {
                              ActividadEspecificaId = ae.ActividadEspecificaId,
                              Nombre = ae.Nombre,
                              ValorApropiacionVigente = ae.ValorApropiacionVigente,
                              SaldoPorProgramar = ae.SaldoPorProgramar,
                              RubroPresupuestal = new RubroPresupuestal()
                              {
                                  RubroPresupuestalId = rp.RubroPresupuestalId,
                                  Identificacion = rp.Identificacion,
                                  Nombre = rp.Nombre.Length > 100 ? rp.Nombre.Substring(0, 100) + "..." : rp.Nombre,
                                  PadreRubroId = 0,
                              },
                              ActividadGeneral = new ActividadGeneral()
                              {
                                  ActividadGeneralId = ag.ActividadGeneralId,
                                  ApropiacionDisponible = ag.ApropiacionDisponible,
                                  ApropiacionVigente = ag.ApropiacionVigente,
                                  RubroPresupuestal = new RubroPresupuestal()
                                  {
                                      RubroPresupuestalId = rp.RubroPresupuestalId,
                                      Identificacion = rp.Identificacion,
                                      Nombre = rp.Nombre.Length > 100 ? rp.Nombre.Substring(0, 100) + "..." : rp.Nombre,
                                      PadreRubroId = 0,
                                  },
                              }
                          }).Distinct()
                            .OrderBy(t => t.RubroPresupuestal.Identificacion));

            return await PagedList<ActividadEspecifica>.CreateAsync(lista, userParams.PageNumber, userParams.PageSize);
        }


        #endregion Tercero
    }
}