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

        #region Tercero
        public async Task<Tercero> ObtenerTerceroBase(int terceroId)
        {
            return await _context.Tercero
                        .FirstOrDefaultAsync(u => u.TerceroId == terceroId);
        }

        public async Task<TerceroDto> ObtenerTercero(int terceroId)
        {
            var lista = await (from t in _context.Tercero
                               join ti in _context.TipoDocumentoIdentidad on t.TipoIdentificacion equals ti.TipoDocumentoIdentidadId
                               where t.TerceroId == terceroId
                               select new TerceroDto()
                               {
                                   TerceroId = t.TerceroId,

                                   TipoDocumentoIdentidadId = t.TipoIdentificacion,
                                   TipoDocumentoIdentidad = ti.Nombre,
                                   NumeroIdentificacion = t.NumeroIdentificacion,
                                   Nombre = t.Nombre,
                                   Direccion = t.Direccion,
                                   Email = t.Email,
                                   Telefono = t.Telefono,
                                   DeclaranteRenta = t.DeclaranteRenta,
                                   DeclaranteRentaDescripcion = string.Empty,
                                   FacturadorElectronico = t.FacturadorElectronico,
                                   FacturadorElectronicoDescripcion = string.Empty,

                                   RegimenTributario = t.RegimenTributario,
                                   FechaExpedicionDocumento = (t.FechaExpedicionDocumento.HasValue ? (t.FechaExpedicionDocumento.Value.ToString() != "0001-01-01 00:00:00.0000000" ? t.FechaExpedicionDocumento.Value : null) : null),
                               }).FirstOrDefaultAsync();

            return lista;
        }

        public async Task<PagedList<TerceroDto>> ObtenerTerceros(int? terceroId, UserParams userParams)
        {
            IOrderedQueryable<TerceroDto> lista = null;

            lista = (from t in _context.Tercero
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


            return await PagedList<TerceroDto>.CreateAsync(lista, userParams.PageNumber, userParams.PageSize);
        }

        public async Task<bool> ValidarExistenciaTercero(int TipoIdentificacion, string numeroIdentificacion)
        {
            var tercero = await (from t in _context.Tercero
                                 where t.TipoIdentificacion == TipoIdentificacion
                                 where t.NumeroIdentificacion == numeroIdentificacion
                                 select new TerceroDto()
                                 {
                                     TerceroId = t.TerceroId
                                 }).FirstOrDefaultAsync();

            if (tercero != null)
            {
                return true;
            }

            return false;
        }

        #endregion Tercero

        #region Parametrización de Liquidación Tercero

        public async Task<PagedList<TerceroDto>> ObtenerTercerosParaParametrizacionLiquidacion(int tipo, int? terceroId, UserParams userParams)
        {
            IOrderedQueryable<TerceroDto> lista = null;

            if (tipo == (int)TipoOperacionTransaccion.Creacion)
            {
                var terceroConParametrizacionIds = _context.ParametroLiquidacionTercero.Select(x => x.TerceroId).ToHashSet();

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

                                   FacturaElectronicaId = plt.FacturaElectronica ? 1 : 0,
                                   SubcontrataId = plt.Subcontrata ? 1 : 0,
                                   SupervisorId = plt.SupervisorId,
                                   OtrosDescuentos = plt.OtrosDescuentos,
                                   FechaInicioOtrosDescuentos = plt.FechaInicioOtrosDescuentos,
                                   FechaFinalOtrosDescuentos = plt.FechaFinalOtrosDescuentos,

                                   TipoAdminPilaId = plt.TipoAdminPilaId,
                                   NotaLegal1 = plt.NotaLegal1,
                                   NotaLegal2 = plt.NotaLegal2,
                                   NotaLegal3 = plt.NotaLegal3,
                                   NotaLegal4 = plt.NotaLegal4,
                                   NotaLegal5 = plt.NotaLegal5,
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
            var lista = await (from td in _context.TerceroDeducciones
                               join ae in _context.ActividadEconomica on td.ActividadEconomicaId equals ae.ActividadEconomicaId into ActividadEconomica
                               from acteco in ActividadEconomica.DefaultIfEmpty()
                               join d in _context.Deduccion on td.DeduccionId equals d.DeduccionId into Deducciones
                               from ded in Deducciones.DefaultIfEmpty()
                               join td1 in _context.Tercero on ded.TerceroId equals td1.TerceroId into TerceroDeducciones1
                               from ter1 in TerceroDeducciones1.DefaultIfEmpty()
                               join td2 in _context.Tercero on td.TerceroDeDeduccionId equals td2.TerceroId into TerceroDeducciones2
                               from ter2 in TerceroDeducciones2.DefaultIfEmpty()
                               where (td.TerceroId == terceroId)
                               //where (ded.estado == true)
                               select new TerceroDeduccionDto()
                               {
                                   TerceroDeduccionId = td.TerceroDeduccionId,
                                   Codigo = ded.DeduccionId > 0 ? ded.Codigo : string.Empty,
                                   Tercero = new ValorSeleccion()
                                   {
                                       Id = td.TerceroId
                                   },
                                   ActividadEconomica = new ValorSeleccion()
                                   {
                                       Id = acteco.ActividadEconomicaId > 0 ? acteco.ActividadEconomicaId : 0,
                                       Codigo = acteco.ActividadEconomicaId > 0 ? acteco.Codigo : string.Empty,
                                       Nombre = acteco.ActividadEconomicaId > 0 ? acteco.Nombre : string.Empty,
                                   },
                                   Deduccion = new ValorSeleccion()
                                   {
                                       Id = ded.DeduccionId > 0 ? ded.DeduccionId : 0,
                                       Codigo = ded.DeduccionId > 0 ? ded.Codigo : string.Empty,
                                       Nombre = ded.DeduccionId > 0 ? ded.Nombre : string.Empty
                                   },
                                   TerceroDeDeduccion = new ValorSeleccion()
                                   {
                                       Id = ter2.TerceroId > 0 ? ter2.TerceroId : 0,
                                       Codigo = ter2.TerceroId > 0 ? ter2.NumeroIdentificacion : string.Empty,
                                       Nombre = ter2.TerceroId > 0 ? ter2.Nombre : string.Empty,
                                       Valor = ter2.TerceroId > 0 ? "SI" : "NO",
                                   },
                                   //    TerceroDeDeduccion = new ValorSeleccion()
                                   //    {
                                   //        Id = ded.DeduccionId > 0 ? ((ter1.TerceroId > 0) ? (ter1.TerceroId) : (ter2.TerceroId)) : 0,
                                   //        Codigo = ded.DeduccionId > 0 ? ((ter1.TerceroId > 0) ? (ter1.NumeroIdentificacion) : (ter2.NumeroIdentificacion)) : string.Empty,
                                   //        Nombre = ded.DeduccionId > 0 ? ((ter1.TerceroId > 0) ? (ter1.Nombre) : (ter2.Nombre)) : string.Empty,
                                   //        Valor = ded.DeduccionId > 0 ? ((ter1.TerceroId > 0) ? ("SI") : (string.Empty)) : "NO",
                                   //    },
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
                              ActividadEconomicaId = td.ActividadEconomicaId,
                              Deduccion = d,
                          }).ToListAsync();
        }

        #endregion Parametrización de Liquidación Tercero
    }
}