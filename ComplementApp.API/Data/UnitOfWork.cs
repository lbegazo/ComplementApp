using System.Threading.Tasks;
using AutoMapper;
using ComplementApp.API.Interfaces;
using EFCore.BulkExtensions;

namespace ComplementApp.API.Data
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly IMapper _mapper;
        private readonly DataContext _context;

        public UnitOfWork(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;

        }

        public IPlanPagoRepository PlanPagoRepository => new PlanPagoRepository(_context, _mapper);

        public async Task<bool> CompleteAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }

        public bool Complete()
        {
            return _context.SaveChanges() > 0;
        }

        public void Rollback()
        {
            _context.Dispose();
        }

        public async Task RollbackAsync()
        {
            await _context.DisposeAsync();
        }
    }
}