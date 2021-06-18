using System;
using System.Collections.Generic;
using System.Linq;
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
    }
}