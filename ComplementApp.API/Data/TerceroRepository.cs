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
    public class TerceroRepository : ITerceroRepository
    {
        private readonly DataContext _context;

        public TerceroRepository(DataContext context)
        {
            _context = context;
        }
        public async Task<PagedList<TerceroDto>> ObtenerTercerosParaParametrizacionLiquidacion(int tipo, int? terceroId, UserParams userParams)
        {
            IOrderedQueryable<TerceroDto> lista = null;

            if (tipo == (int)TipoOperacionTransaccion.Creacion)
            {
                var terceroConParametrizacionIds = _context.ParametroLiquidacionTercero.Select(x => x.TerceroId).ToHashSet();
                //var notFoundItems = _context.CDP.Where(item => !listaCompromisos.Contains(item.Crp)).Select(x => x.Crp).ToHashSet();

                lista = (from t in _context.Tercero
                         join c in _context.CDP on t.TerceroId equals c.TerceroId
                         join ti in _context.TipoDocumentoIdentidad on t.TipoIdentificacion equals ti.TipoDocumentoIdentidadId
                         where !terceroConParametrizacionIds.Contains(t.TerceroId)
                         where c.Instancia == (int)TipoDocumento.Compromiso
                         where c.SaldoActual > 0 //Saldo Disponible
                         where c.TerceroId == terceroId || terceroId == null
                         select new TerceroDto()
                         {
                             TerceroId = t.TerceroId,
                             TipoDocumentoIdentidadId = t.TipoIdentificacion,
                             TipoDocumentoIdentidad = ti.Nombre,
                             NumeroIdentificacion = t.NumeroIdentificacion,
                             Nombre = t.Nombre,
                         })
                           .Distinct()
                           .OrderBy(t => t.Nombre);
            }
            else
            {
                lista = (from t in _context.Tercero
                         join tpl in _context.ParametroLiquidacionTercero on t.TerceroId equals tpl.TerceroId
                         join ti in _context.TipoDocumentoIdentidad on t.TipoIdentificacion equals ti.TipoDocumentoIdentidadId
                         where t.TerceroId == terceroId || terceroId == null
                         select new TerceroDto()
                         {
                             TerceroId = t.TerceroId,
                             TipoDocumentoIdentidadId = t.TipoIdentificacion,
                             TipoDocumentoIdentidad = ti.Nombre,
                             NumeroIdentificacion = t.NumeroIdentificacion,
                             Nombre = t.Nombre,
                         })
                           .Distinct()
                           .OrderBy(t => t.Nombre);
            }

            return await PagedList<TerceroDto>.CreateAsync(lista, userParams.PageNumber, userParams.PageSize);

        }

        public async Task<ParametroLiquidacionTerceroDto> ObtenerParametrizacionLiquidacionXTercero(int terceroId)
        {
            var lista = await (from plt in _context.ParametroLiquidacionTercero
                               join t in _context.Tercero on plt.TerceroId equals t.TerceroId
                               join ti in _context.TipoDocumentoIdentidad on t.TipoIdentificacion equals ti.TipoDocumentoIdentidadId
                               where plt.TerceroId == terceroId
                               select new ParametroLiquidacionTerceroDto()
                               {
                                   ParametroLiquidacionTerceroId = plt.ParametroLiquidacionTerceroId,

                                   TerceroId = t.TerceroId,
                                   TipoDocumentoIdentidadId = t.TipoIdentificacion,
                                   TipoDocumentoIdentidad = ti.Nombre,
                                   IdentificacionTercero = t.NumeroIdentificacion,
                                   NombreTercero = t.Nombre,

                                   ModalidadContrato = plt.ModalidadContrato,
                                   TipoPago = plt.TipoPago,
                                   HonorarioSinIva = plt.HonorarioSinIva,
                                   TarifaIva = plt.TarifaIva,
                                   TipoIva = plt.TipoIva,
                                   TipoCuentaPorPagar = plt.TipoCuentaPorPagar,
                                   TipoDocumentoSoporte = plt.TipoDocumentoSoporte,

                                   BaseAporteSalud = plt.BaseAporteSalud,
                                   AporteSalud = plt.AporteSalud,
                                   AportePension = plt.AportePension,
                                   RiesgoLaboral = plt.RiesgoLaboral,
                                   FondoSolidaridad = plt.FondoSolidaridad,

                                   PensionVoluntaria = plt.PensionVoluntaria,
                                   Dependiente = plt.Dependiente,
                                   Afc = plt.Afc,
                                   MedicinaPrepagada = plt.MedicinaPrepagada,
                                   InteresVivienda = plt.InteresVivienda,
                                   FechaInicioDescuentoInteresVivienda = plt.FechaInicioDescuentoInteresVivienda,
                                   FechaFinalDescuentoInteresVivienda = plt.FechaFinalDescuentoInteresVivienda,
                               }).FirstOrDefaultAsync();

            return lista;
        }

        public async Task<ParametroLiquidacionTercero> ObtenerParametrizacionLiquidacionTerceroBase(int parametroLiquidacionTerceroId)
        {
            return await _context.ParametroLiquidacionTercero
                        .FirstOrDefaultAsync(u => u.ParametroLiquidacionTerceroId == parametroLiquidacionTerceroId);
        }

        public async Task<ICollection<TerceroDeduccionDto>> ObtenerDeduccionesXTercero(int terceroId)
        {
            var lista = await (from d in _context.Deduccion
                               join td in _context.TerceroDeducciones on d.DeduccionId equals td.DeduccionId
                               join ae in _context.ActividadEconomica on td.ActividadEconomicaId equals ae.ActividadEconomicaId
                               where (td.TerceroId == terceroId)
                               where (d.estado == true)
                               select new TerceroDeduccionDto()
                               {
                                   TerceroDeduccionId = td.TerceroDeduccionId,
                                   Codigo = d.Codigo,
                                   Tercero = new ValorSeleccion()
                                   {
                                       Id = td.TerceroId
                                   },
                                   Deduccion = new ValorSeleccion()
                                   {
                                       Id = d.DeduccionId,
                                       Codigo = d.Codigo,
                                       Nombre = d.Nombre
                                   },
                                   ActividadEconomica = new ValorSeleccion()
                                   {
                                       Id = ae.ActividadEconomicaId,
                                       Codigo = ae.Codigo,
                                       Nombre = ae.Nombre
                                   }
                               }
                         )
                         .Distinct()
                         .OrderBy(d => d.Codigo)
                         .ToListAsync();

            return lista;
        }

        public async Task<PagedList<DeduccionDto>> ObteneListaDeducciones(UserParams userParams)
        {
            var lista = (from d in _context.Deduccion
                             //join td in _context.TerceroDeducciones on d.DeduccionId equals td.DeduccionId
                             //join ae in _context.ActividadEconomica on td.ActividadEconomicaId equals ae.ActividadEconomicaId
                         where (d.estado == true)
                         select new DeduccionDto()
                         {
                             DeduccionId = d.DeduccionId,
                             Codigo = d.Codigo,
                             Nombre = d.Nombre
                         }
                         )
                         .Distinct()
                         .OrderBy(x => x.Codigo);

            return await PagedList<DeduccionDto>.CreateAsync(lista, userParams.PageNumber, userParams.PageSize);

        }

        public async Task<bool> EliminarTerceroDeduccionesXTercero(int terceroId)
        {
            var listaExistente = await _context.TerceroDeducciones.Where(x => x.TerceroId == terceroId).ToListAsync();
            _context.TerceroDeducciones.RemoveRange(listaExistente);
            return true;
        }

        public async Task<ICollection<ValorSeleccion>> ObtenerListaActividadesEconomicaXTercero(int terceroId)
        {
            var lista = await (from ae in _context.ActividadEconomica
                               join td in _context.TerceroDeducciones on ae.ActividadEconomicaId equals td.ActividadEconomicaId
                               where td.TerceroId == terceroId
                               select new ValorSeleccion()
                               {
                                   Id = ae.ActividadEconomicaId,
                                   Codigo = ae.Codigo,
                                   Nombre = ae.Nombre
                               })
                                .Distinct()
                                .ToListAsync();

            return lista;
        }

        public List<int> ObtenerTercerosConMasDeUnaActividadEconomica()
        {
            List<int> terceros = null;
            //var query = _context.TerceroDeducciones.GroupBy(x => new { x.TerceroId, x.ActividadEconomicaId.Value });

            var query1 = (from t in _context.TerceroDeducciones
                          group t by new { t.ActividadEconomicaId, t.TerceroId }
                         into grp
                          select new
                          {
                              grp.Key.TerceroId,
                              grp.Key.ActividadEconomicaId
                          });

            var query2 = (from p in query1
                          group p by new { p.TerceroId } into g
                          where g.Count() > 0
                          select new
                          {
                              g.Key.TerceroId,
                              Count = g.Count()
                          });

            var query3 = (from p in query2
                          where p.Count > 1
                          select new
                          {
                              p.TerceroId
                          });

            terceros = query3.Select(s => s.TerceroId).ToList();

            return terceros;
        }


        public async Task<ParametroLiquidacionTercero> ObtenerParametroLiquidacionXTercero(int terceroId)
        {
            return await _context.ParametroLiquidacionTercero
                        .Where(x => x.TerceroId == terceroId).FirstOrDefaultAsync();
        }

        public async Task<ICollection<ParametroLiquidacionTercero>> ObtenerListaParametroLiquidacionTerceroXIds(List<int> listaTerceroId)
        {
            return await (from p in _context.ParametroLiquidacionTercero
                          where listaTerceroId.Contains(p.TerceroId)
                          select p).ToListAsync();
        }

        public async Task<ICollection<Deduccion>> ObtenerDeduccionesXTercero(int terceroId, int? actividadEconomicaId)
        {
            var query = (from d in _context.Deduccion
                         join td in _context.TerceroDeducciones on d.DeduccionId equals td.DeduccionId
                         where (td.TerceroId == terceroId)
                         where (td.ActividadEconomicaId == actividadEconomicaId || actividadEconomicaId == null)
                         where (d.estado == true)
                         select d);

            return await query.ToListAsync();
        }

        public async Task<ICollection<TerceroDeduccion>> ObtenerListaDeduccionesXTerceroIds(List<int> listaTerceroId)
        {
            return await (from td in _context.TerceroDeducciones
                          join d in _context.Deduccion on td.DeduccionId equals d.DeduccionId
                          where listaTerceroId.Contains(td.TerceroId)
                          where (d.estado == true)
                          select new TerceroDeduccion()
                          {
                              TerceroId = td.TerceroId,
                              Deduccion = d,
                          }).ToListAsync();
        }
    }
}