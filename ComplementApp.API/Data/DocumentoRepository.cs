using System.Collections.Generic;
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
        public async Task InsertaCabeceraCDP(IList<CDP> lista)
        {
            CancellationToken cancellation = new CancellationToken(false);
            var bulkConfig = new BulkConfig { PreserveInsertOrder = true, SetOutputIdentity = true, BatchSize = 4000 };
            await _context.BulkInsertAsync(lista, bulkConfig, null, cancellation);
        }

        public async Task InsertaDetalleCDP(IList<DetalleCDP> lista)
        {
            CancellationToken cancellation = new CancellationToken(false);
            var bulkConfig = new BulkConfig { PreserveInsertOrder = true, SetOutputIdentity = true, BatchSize = 4000 };
            await _context.BulkInsertAsync(lista, bulkConfig, null, cancellation);
        }
    }
}