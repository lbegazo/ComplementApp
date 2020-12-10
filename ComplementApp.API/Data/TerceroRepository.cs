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
    }
}