using System;
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
                lista = (from c in _context.DocumentoCompromiso
                         join t in _context.Tercero on c.TerceroId equals t.TerceroId
                         where !listaCompromisos.Contains(c.NumeroDocumento)
                         where c.PciId == userParams.PciId
                         where c.SaldoPorUtilizar > 0 //Saldo Disponible
                         where c.TerceroId == terceroId || terceroId == null
                         select new CDPDto()
                         {
                             Crp = c.NumeroDocumento,
                             //bool success = Int64.TryParse(c.Cdp, out long number);
                             Cdp = c.Cdp,
                             Detalle4 = c.Observaciones,
                             NumeroIdentificacionTercero = t.NumeroIdentificacion,
                             NombreTercero = t.Nombre,
                         })
                            .Distinct()
                            .OrderBy(x => x.Crp);
            }
            else
            {
                lista = (from cla in _context.ClavePresupuestalContable
                         join c in _context.DocumentoCompromiso on cla.Crp equals c.NumeroDocumento
                         join t in _context.Tercero on c.TerceroId equals t.TerceroId
                         where cla.PciId == c.PciId
                         where cla.PciId == userParams.PciId
                         where c.SaldoPorUtilizar > 0 //Saldo Disponible
                         where c.TerceroId == terceroId || terceroId == null
                         select new CDPDto()
                         {
                             Crp = c.NumeroDocumento,
                             Cdp = c.Cdp,
                             Detalle4 = c.Observaciones,
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
                         join c in _context.DocumentoCompromiso on cla.Crp equals c.NumeroDocumento
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
                         where cla.PciId == c.PciId
                         where cla.PciId == rc.PciId
                         where cla.PciId == usoPre.PciId
                         where cla.PciId == pciId
                         where rp.RubroPresupuestalId == c.RubroPresupuestalId
                         select new ClavePresupuestalContableDto()
                         {
                             Crp = c.NumeroDocumento,
                             Detalle4 = c.Observaciones,

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
                                  join c in _context.DocumentoCompromiso on rp.RubroPresupuestalId equals c.RubroPresupuestalId
                                  join t in _context.Tercero on c.TerceroId equals t.TerceroId
                                  join sf in _context.SituacionFondo on c.SituacionFondo equals sf.Nombre
                                  join ff in _context.FuenteFinanciacion on c.FuenteFinanciacion equals ff.Nombre
                                  join r in _context.RecursoPresupuestal on c.RecursoPresupuestal.ToUpper() equals r.Nombre.ToUpper()
                                  where c.NumeroDocumento == crp
                                  where c.PciId == pciId
                                  select new ClavePresupuestalContableDto()
                                  {
                                      DocumentoCompromisoId = c.DocumentoCompromisoId,
                                      Crp = c.NumeroDocumento,
                                      Dependencia = c.Dependencia	,
                                      DependenciaDescripcion = c.Dependencia + " " + (c.DependenciaDescripcion.Length > 100 ? c.DependenciaDescripcion.Substring(0, 100) + "..." : c.DependenciaDescripcion),
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
                                          Nombre = ff.Nombre
                                      },
                                      SituacionFondo = new ValorSeleccion()
                                      {
                                          Id = sf.SituacionFondoId,
                                          Nombre = sf.Nombre
                                      },
                                      RecursoPresupuestal = new ValorSeleccion()
                                      {
                                          Id = r.RecursoPresupuestalId,
                                          Codigo = r.Codigo,
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
                                  join c in _context.DocumentoCompromiso on cla.Crp equals c.NumeroDocumento
                                  join t in _context.Tercero on c.TerceroId equals t.TerceroId
                                  join sf in _context.SituacionFondo on cla.SituacionFondoId equals sf.SituacionFondoId
                                  join ff in _context.FuenteFinanciacion on cla.FuenteFinanciacionId equals ff.FuenteFinanciacionId
                                  join r in _context.RecursoPresupuestal on cla.RecursoPresupuestalId equals r.RecursoPresupuestalId
                                  join rc in _context.RelacionContable on cla.RelacionContableId equals rc.RelacionContableId
                                  join cc in _context.CuentaContable on rc.CuentaContableId equals cc.CuentaContableId into CuentaContable
                                  from cuCo in CuentaContable.DefaultIfEmpty()
                                  join up in _context.UsoPresupuestal on new { UsoPresupuestalId = cla.UsoPresupuestalId.Value, PciId = cla.PciId.Value } equals
                                                                         new { UsoPresupuestalId = up.UsoPresupuestalId, PciId = up.PciId.Value } into UsoPresupuestal
                                  from usoPre in UsoPresupuestal.DefaultIfEmpty()
                                  where cla.Crp == crp
                                  where rp.RubroPresupuestalId == c.RubroPresupuestalId
                                  where cla.Crp == c.NumeroDocumento
                                  where cla.Dependencia == c.Dependencia
                                  where cla.PciId == c.PciId   
                                  where cla.PciId == rc.PciId 
                                  where cla.PciId == pciId                                  

                                  select new ClavePresupuestalContableDto()
                                  {
                                      ClavePresupuestalContableId = cla.ClavePresupuestalContableId,
                                      //DocumentoCompromisoId = c.DocumentoCompromisoId,
                                      Crp = c.NumeroDocumento,
                                      Dependencia = c.Dependencia,
                                      DependenciaDescripcion = c.Dependencia + " " + (c.DependenciaDescripcion.Length > 100 ? c.DependenciaDescripcion.Substring(0, 100) + "..." : c.DependenciaDescripcion),
                                      RubroPresupuestal = new ValorSeleccion()
                                      {
                                          Id = rp.RubroPresupuestalId,
                                          Codigo = rp.Identificacion,
                                          Nombre = rp.Nombre,
                                          Valor = rp.PadreContableId>0 ? rp.PadreContableId.ToString() : string.Empty
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
                                          Nombre = ff.Nombre
                                      },
                                      SituacionFondo = new ValorSeleccion()
                                      {
                                          Id = sf.SituacionFondoId,
                                          Nombre = sf.Nombre
                                      },
                                      RecursoPresupuestal = new ValorSeleccion()
                                      {
                                          Id = r.RecursoPresupuestalId,
                                          Codigo = r.Codigo,
                                          Nombre = r.Nombre
                                      },
                                      RelacionContable = new ValorSeleccion()
                                      {
                                          Id = cla.RelacionContableId > 0 ? cla.RelacionContableId : 0,
                                          Codigo = cuCo.NumeroCuenta,
                                          Nombre = cuCo.DescripcionCuenta
                                      },
                                      UsoPresupuestal = new ValorSeleccion()
                                      {
                                          Id = cla.UsoPresupuestalId.HasValue ? cla.UsoPresupuestalId.Value : 0,
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
            //Obtener el padre contable Id asociado al rubro presupuestal
            var padreContableId = await _context.RubroPresupuestal
                                 .Where(rp => rp.RubroPresupuestalId == rubroPresupuestalId)
                                 .Select(rp => rp.PadreContableId)
                                 .FirstOrDefaultAsync();


            //La relación contable esta relacionado al padre contable(rubro presupuestal)
            var lista = await (from rc in _context.RelacionContable
                               join ac in _context.AtributoContable on rc.AtributoContableId equals ac.AtributoContableId
                               join tg in _context.TipoGasto on rc.TipoGastoId equals tg.TipoGastoId into tipoGasto
                               from t in tipoGasto.DefaultIfEmpty()
                               join cc in _context.CuentaContable on rc.CuentaContableId equals cc.CuentaContableId into CuentaContable
                               from cuco in CuentaContable.DefaultIfEmpty()
                               where rc.RubroPresupuestalId == padreContableId
                               where rc.PciId == pciId
                               where rc.Estado == true
                               select new RelacionContableDto()
                               {
                                   RelacionContableId = rc.RelacionContableId,
                                   TipoOperacion = rc.TipoOperacion,
                                   UsoContable = rc.UsoContable,
                                   CuentaContable = new ValorSeleccion()
                                   {
                                       Id = rc.CuentaContableId != null ? cuco.CuentaContableId : 0,
                                       Codigo = rc.CuentaContableId != null ? cuco.NumeroCuenta : string.Empty,
                                       Nombre = rc.CuentaContableId != null ? cuco.DescripcionCuenta.ToUpper() : string.Empty,
                                   },
                                   AtributoContable = new ValorSeleccion()
                                   {
                                       Id = ac.AtributoContableId,
                                       Nombre = ac.Codigo +'-'+ ac.Nombre.ToUpper()
                                   },

                                   TipoGasto = new ValorSeleccion()
                                   {
                                       Id = rc.TipoGastoId != null ? t.TipoGastoId : 0,
                                       Nombre = rc.TipoGastoId != null ? t.Codigo+'-'+ t.Nombre.ToUpper() : string.Empty
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