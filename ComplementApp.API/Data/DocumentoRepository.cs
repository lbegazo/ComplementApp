using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ComplementApp.API.Models;
using EFCore.BulkExtensions;

namespace ComplementApp.API.Data
{
    public class DocumentoRepository : IDocumentoRepository
    {
        private readonly DataContext _context;

        public DocumentoRepository(DataContext context)
        {
            _context = context;
        }
        public async Task<bool> InsertaCabeceraCDP(IList<CDP> lista)
        {
            try
            {
                CancellationToken cancellation = new CancellationToken(false);
                var bulkConfig = new BulkConfig
                {
                    PreserveInsertOrder = true,
                    SetOutputIdentity = true,
                    BatchSize = 4000
                };
                await _context.BulkInsertAsync(lista, bulkConfig, null, cancellation);
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<bool> InsertaDetalleCDP(IList<DetalleCDP> lista)
        {
            try
            {
                CancellationToken cancellation = new CancellationToken(false);
                var bulkConfig = new BulkConfig
                {
                    PreserveInsertOrder = true,
                    SetOutputIdentity = true,
                    BatchSize = 4000
                };
                await _context.BulkInsertAsync(lista, bulkConfig, null, cancellation);
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("error " +ex.Message);
            }
        }

        public bool EliminarCabeceraCDP()
        {
            try
            {
                if (!_context.CDP.Any())
                    return true;

                if (_context.CDP.Any())
                    return _context.CDP.BatchDelete() > 0;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return false;
        }

        public bool EliminarDetalleCDP()
        {
            try{
            if (!_context.DetalleCDP.Any())
                return true;

            if (_context.DetalleCDP.Any())
                return _context.DetalleCDP.BatchDelete() > 0;
            }
            catch(Exception ex)
            {
                throw ex;
            }
            return false;
        }
    }
}