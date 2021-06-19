using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ComplementApp.API.Dtos;
using ComplementApp.API.Helpers;
using ComplementApp.API.Interfaces.Repository;
using ComplementApp.API.Models;
using EFCore.BulkExtensions;

namespace ComplementApp.API.Data
{
    public class CargaObligacionRepository : ICargaObligacionRepository
    {
        private readonly DataContext _context;

        public CargaObligacionRepository(DataContext context)
        {
            _context = context;
        }
        public bool EliminarCargaObligacion()
        {
            try
            {
                if (!_context.CargaObligacion.Any())
                    return true;

                if (_context.CargaObligacion.Any())
                    return _context.CargaObligacion.BatchDelete() > 0;
            }
            catch (Exception)
            {
                throw;
            }
            return false;
        }
        public bool InsertarListaCargaObligacion(IList<CargaObligacion> listaCdp)
        {
            try
            {
                _context.BulkInsert(listaCdp);
                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<PagedList<CDPDto>> ObtenerListaCargaObligacion(string estado, UserParams userParams)
        {
            var lista = (from c in _context.CargaObligacion
                         where c.Estado.ToLower() == estado.ToLower()
                         where c.PciId == userParams.PciId
                         select new CDPDto()
                         {
                             CdpId = c.CargaObligacionId,
                             Obligacion = c.Obligacion,
                             ValorInicial = c.ValorActual2,
                             NumeroIdentificacionTercero = c.NumeroIdentificacion,
                             NombreTercero = c.NombreRazonSocial,
                             IdentificacionRubro = c.RubroIdentificacion,
                             NombreRubro = c.RubroDescripcion,
                         })
                         .Distinct();

            return await PagedList<CDPDto>.CreateAsync(lista, userParams.PageNumber, userParams.PageSize);
        }
    }
}