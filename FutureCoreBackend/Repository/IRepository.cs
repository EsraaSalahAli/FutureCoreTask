using FutureCoreBackend.Models;

namespace FutureCoreBackend.Repository
{
    public interface IRepository<T> where T : class
    {
        Task<List<T>> GetAllAsync();
        Task<T> GetByID(int ID);
        Task Add(T item);
        void Update(T item);
        void SoftDelete(T item);
        void Delete(T item);
        Task<PaginatedList<T>> GetAllPaginated(List<T> list, int pageIndex, int pageSize);
    }
}
