using Microsoft.EntityFrameworkCore;
using TVScanner.Shared;

namespace TVScanner.Data.AccessProvider
{
    public class RepositoryContext : IRepositoryContext
    {
        private readonly TVScannerContext _context;

        public RepositoryContext(IDbContextFactory<TVScannerContext> myDbContextFactory)
        {
            _context = myDbContextFactory.CreateDbContext();
        }

        public DbSet<T> Set<T>() where T : class
        {
            return _context.Set<T>();
        }

        public async Task<T?> FindByIdAsync<T>(Guid id) where T : class
        {
            return await _context.FindAsync<T>(id);
        }

        public void Remove<T>(T entity) where T : class
        {
            _context.Remove(entity);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
