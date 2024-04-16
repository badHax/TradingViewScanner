using Microsoft.EntityFrameworkCore;

namespace TVScanner.Shared
{
    public interface IRepositoryContext
    {
        Task<T?> FindByIdAsync<T>(Guid id) where T : class;
        void Remove<T>(T entity) where T : class;
        Task SaveChangesAsync();
        DbSet<T> Set<T>() where T : class;
    }
}
