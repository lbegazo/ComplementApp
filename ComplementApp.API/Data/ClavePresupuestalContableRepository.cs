using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ComplementApp.API.Dtos;
using ComplementApp.API.Helpers;
using ComplementApp.API.Interfaces;
using ComplementApp.API.Models;
using Microsoft.EntityFrameworkCore;

namespace ComplementApp.API.Data
{
    public class ClavePresupuestalContableRepository : IClavePresupuestalContableRepository
    {
        private readonly DataContext _context;

        public ClavePresupuestalContableRepository(DataContext context)
        {
            _context = context;
        }

        #region Clave Presupuestal Contable

        public async Task<PagedList<CDPDto>> ObtenerCompromisosParaClavePresupuestalContable(int tipo, int? terceroId, UserParams userParams)
        {
            IOrderedQueryable<CDPDto> lista = null;
            //var notFoundItems = _context.CDP.Where(item => !listaCompromisos.Contains(item.Crp)).Select(x => x.Crp).ToHashSet();

            var listaCompromisos = (from pp in _context.ClavePresupuestalContable
                                    where pp.PciId == userParams.PciId
                                    select pp.Crp).ToHashSet();

            if (tipo == (int)TipoOperacionTransaccion.Creacion)
            {
                lista = (from c in _context.CDP
                         join t in _context.Tercero on c.TerceroId equals t.TerceroId
                         where !listaCompromisos.Contains(c.Crp)
                         where c.PciId == userParams.PciId
                         where c.Instancia == (int)TipoDocumento.Compromiso
                         where c.SaldoActual > 0 //Saldo Disponible
                         where c.TerceroId == terceroId || terceroId == null
                         select new CDPDto()
                         {
                             Crp = c.Crp,
                             Detalle4 = c.Detalle4.Length > 180 ? c.Detalle4.Substring(0, 180) + "..." : c.Detalle4,
                             NumeroIdentificacionTercero = t.NumeroIdentificacion,
                             NombreTercero = t.Nombre,
                         })
                            .Distinct()
                            .OrderBy(x => x.Crp);
            }
            else
            {
                lista = (from cla in _context.ClavePresupuestalContable
                         join c in _context.CDP on cla.Crp equals c.Crp
                         join t in _context.Tercero on c.TerceroId equals t.TerceroId
                         where cla.PciId == c.PciId
                         where c.Instancia == (int)TipoDocumento.Compromiso
                         where cla.PciId == userParams.PciId
                         where c.SaldoActual > 0 //Saldo Disponible
                         where c.TerceroId == terceroId || terceroId == null
                         select new CDPDto()
                         {
                             Crp = c.Crp,
                             Detalle4 = c.Detalle4.Length > 180 ? c.Detalle4.Substring(0, 180) + "..." : c.Detalle4,
                             NumeroIdentificacionTercero = t.NumeroIdentificacion,
                             NombreTercero = t.Nombre,
                         })
                            .Distinct()
                            .OrderBy(x => x.Crp);
            }

            return await PagedList<CDPDto>.CreateAsync(lista, userParams.PageNumber, userParams.PageSize);
        }

        public async Task<ICollection<ClavePresupuestalContableDto>> ObtenerListaClavePresupuestalContable(int pciId)
        {
            var lista = (from cla in _context.ClavePresupuestalContable
                         join c in _context.CDP on cla.Crp equals c.Crp
                         join t in _context.Tercero on c.TerceroId equals t.TerceroId
                         join rp in _context.RubroPresupuestal on cla.RubroPresupuestalId equals rp.RubroPresupuestalId
                         join rc in _context.RelacionContable on cla.RelacionContableId equals rc.RelacionContableId
                         join tg in _context.TipoGasto on rc.TipoGastoId equals tg.TipoGastoId into TipoGasto
                         from tiGa in TipoGasto.DefaultIfEmpty()
                         join cc in _context.CuentaContable on rc.CuentaContableId equals cc.CuentaContableId into CuentaContable
                         from cuCo in CuentaContable.DefaultIfEmpty()
                         join ac in _context.AtributoContable on rc.AtributoContableId equals ac.AtributoContableId into AtributoContable
                         from atrCon in AtributoContable.DefaultIfEmpty()
                         join up in _context.UsoPresupuestal on cla.UsoPresupuestalId equals up.UsoPresupuestalId into UsoPresupuestal
                         from usoPre in UsoPresupuestal.DefaultIfEmpty()
                         where c.Instancia == (int)TipoDocumento.Compromiso
                         where cla.PciId == c.PciId
                         where cla.PciId == rc.PciId
                         where cla.PciId == usoPre.PciId
                         where cla.PciId == pciId
                         where rp.RubroPresupuestalId == c.RubroPresupuestalId
                         select new ClavePresupuestalContableDto()
                         {
                             Crp = c.Crp,
                             Detalle4 = c.Detalle4,

                             Tercero = new ValorSeleccion()
                             {
                                 Codigo = t.NumeroIdentificacion,
                                 Nombre = t.Nombre,
                             },
                             RubroPresupuestal = new ValorSeleccion()
                             {
                                 Codigo = rp.Identificacion,
                             },
                             RelacionContableDto = new RelacionContable()
                             {
                                 TipoOperacion = rc.TipoOperacion,
                                 UsoContable = rc.UsoContable,
                                 TipoGasto = new TipoGasto()
                                 {
                                     Codigo = rc.TipoGastoId > 0 ? tiGa.Codigo : string.Empty,
                                 },
                                 AtributoContable = new AtributoContable()
                                 {
                                     Nombre = rc.AtributoContableId > 0 ? atrCon.Nombre : string.Empty
                                 }
                             },
                             CuentaContable = new ValorSeleccion()
                             {
                                 Codigo = cuCo.NumeroCuenta,
                                 Nombre = cuCo.DescripcionCuenta
                             },
                             UsoPresupuestal = new ValorSeleccion()
                             {
                                 Codigo = cla.UsoPresupuestalId > 0 ? usoPre.Identificacion : string.Empty,
                                 Nombre = cla.UsoPresupuestalId > 0 ? usoPre.Nombre : string.Empty,
                             },
                         })
                         .Distinct()
                        .OrderBy(c => c.Crp);

            return await lista.ToListAsync();
        }

        public async Task<ICollection<ClavePresupuestalContableDto>> ObtenerRubrosPresupuestalesXCompromiso(int crp, int pciId)
        {

            var detalles = await (from rp in _context.RubroPresupuestal
                                  join c in _context.CDP on rp.RubroPresupuestalId equals c.RubroPresupuestalId
                                  join t in _context.Tercero on c.TerceroId equals t.TerceroId
                                  join sf in _context.SituacionFondo on c.Detalle9 equals sf.Nombre
                                  join ff in _context.FuenteFinanciacion on c.Detalle8 equals ff.Nombre
                                  join r in _context.RecursoPresupuestal on c.Detalle10 equals r.Codigo
                                  where c.Crp == crp
                                  where c.Instancia == (int)TipoDocumento.Compromiso
                                  where c.PciId == pciId
                                  select new ClavePresupuestalContableDto()
                                  {
                                      CdpId = c.CdpId,
                                      Crp = c.Crp,
                                      Dependencia = c.Detalle2 + " " + (c.Detalle3.Length > 100 ? c.Detalle3.Substring(0, 100) + "..." : c.Detalle3),
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

        public async Task<ICollection<ClavePresupuestalContableDto>> ObtenerClavesPresupuestalContableXCompromiso(int crp, int pciId)
        {

            var detalles = await (from cla in _context.ClavePresupuestalContable
                                  join rp in _context.RubroPresupuestal on cla.RubroPresupuestalId equals rp.RubroPresupuestalId
                                  join c in _context.CDP on cla.Crp equals c.Crp
                                  join t in _context.Tercero on c.TerceroId equals t.TerceroId
                                  join sf in _context.SituacionFondo on c.Detalle9 equals sf.Nombre
                                  join ff in _context.FuenteFinanciacion on c.Detalle8 equals ff.Nombre
                                  join r in _context.RecursoPresupuestal on c.Detalle10 equals r.Codigo
                                  join rc in _context.RelacionContable on cla.RelacionContableId equals rc.RelacionContableId
                                  join cc in _context.CuentaContable on rc.CuentaContableId equals cc.CuentaContableId into CuentaContable
                                  from cuCo in CuentaContable.DefaultIfEmpty()
                                  join up in _context.UsoPresupuestal on cla.UsoPresupuestalId equals up.UsoPresupuestalId into UsoPresupuestal
                                  from usoPre in UsoPresupuestal.DefaultIfEmpty()
                                  where rp.RubroPresupuestalId == c.RubroPresupuestalId
                                  where cla.PciId == c.PciId
                                  where cla.PciId == rc.PciId
                                  where cla.PciId == usoPre.PciId
                                  where cla.Crp == crp
                                  where cla.PciId == pciId
                                  where c.Instancia == (int)TipoDocumento.Compromiso

                                  select new ClavePresupuestalContableDto()
                                  {
                                      ClavePresupuestalContableId = cla.ClavePresupuestalContableId,
                                      CdpId = c.CdpId,
                                      Crp = c.Crp,
                                      Dependencia = c.Detalle2 + " " + (c.Detalle3.Length > 100 ? c.Detalle3.Substring(0, 100) + "..." : c.Detalle3),
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
                                      RelacionContable = new ValorSeleccion()
                                      {
                                          Codigo = cuCo.NumeroCuenta
                                      },
                                      UsoPresupuestal = new ValorSeleccion()
                                      {
                                          Codigo = cla.UsoPresupuestalId > 0 ? usoPre.Identificacion : string.Empty,
                                          Nombre = cla.UsoPresupuestalId > 0 ? usoPre.Nombre : string.Empty,
                                      },
                                  })
                                 .Distinct()
                                 .OrderBy(rp => rp.RubroPresupuestal.Codigo)
                                 .ToListAsync();
            return detalles;
        }

        public async Task<ICollection<RelacionContableDto>> ObtenerRelacionesContableXRubroPresupuestal(int rubroPresupuestalId, int pciId)
        {
            var lista = await (from rc in _context.RelacionContable
                               join cc in _context.CuentaContable on rc.CuentaContableId equals cc.CuentaContableId
                               join ac in _context.AtributoContable on rc.AtributoContableId equals ac.AtributoContableId
                               join tg in _context.TipoGasto on rc.TipoGastoId equals tg.TipoGastoId into tipoGasto
                               from t in tipoGasto.DefaultIfEmpty()
                               where rc.RubroPresupuestalId == rubroPresupuestalId
                               where rc.PciId == pciId
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
                                       Id = rc.TipoGastoId != null ? t.TipoGastoId : 0,
                                       Nombre = rc.TipoGastoId != null ? t.Nombre : string.Empty
                                   }
                               })
                                 .Distinct()
                                 .OrderBy(cc => cc.CuentaContable.Codigo)
                                 .ToListAsync();

            return lista;
        }

        public async Task<ICollection<ValorSeleccion>> ObtenerUsosPresupuestalesXRubroPresupuestal(int rubroPresupuestalId)
        {
            var lista = await (from up in _context.UsoPresupuestal
                               where up.RubroPresupuestalId == rubroPresupuestalId
                               select new ValorSeleccion()
                               {
                                   Id = up.UsoPresupuestalId,
                                   Codigo = up.Identificacion,
                                   Nombre = up.Nombre,
                               })
                                 .Distinct()
                                 .OrderBy(up => up.Codigo)
                                 .ToListAsync();

            return lista;
        }

        public async Task<bool> RegistrarRelacionContable(RelacionContable relacion)
        {
            await _context.RelacionContable.AddAsync(relacion);
            return true;
        }

        public async Task<ClavePresupuestalContable> ObtenerClavePresupuestalContableBase(int claveId)
        {
            return await _context.ClavePresupuestalContable
                        .FirstOrDefaultAsync(u => u.ClavePresupuestalContableId == claveId);
        }

        #endregion Clave Presupuestal Contable
    }
}