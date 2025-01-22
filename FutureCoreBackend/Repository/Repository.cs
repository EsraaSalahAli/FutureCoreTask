using FutureCoreBackend.Models;
using Microsoft.EntityFrameworkCore;

namespace FutureCoreBackend.Repository
{
    public class Repository<T> : IRepository<T> where T : class , ISoftDeleteRepo
    {
        private readonly Context Context;
        private DbSet<T> entities;
        public Repository(Context _context)
        {
            Context = _context;
            entities = _context.Set<T>();
        }

        public async Task Add(T item)
        {
            if (item != null)
            {
                await entities.AddAsync(item);
            }
        }

        public void Delete(T item)
        {
            if (item != null)
            {
                entities.Remove(item);
            }
        }

        public async Task<List<T>> GetAllAsync()
        {
            return await entities.ToListAsync();
        }

        public IQueryable<T> GetAll()
        {
            Console.WriteLine($"Entities type: {entities.GetType()}");
            return entities;
        }

        public async Task<T> GetByID(int ID)
        {
            return await entities.FindAsync(ID);
        }

        public void Update(T item)
        {
            if (item != null)
            {
                entities.Update(item);
            }
        }

        public void SoftDelete(T item)
        {
            item.IsDeleted = true;
            entities.Update(item);
        }

        public async Task<PaginatedList<T>> GetAllPaginated(List<T> list, int pageIndex, int pageSize)
        {
            PaginatedList<T> paginatedList = new PaginatedList<T>();

            if (list != null)
            {
                paginatedList.Items = list.Skip((pageIndex - 1) * pageSize)
                    .Take(pageSize).ToList();
            }
            else
            {
                paginatedList.Items = await entities.Skip((pageIndex - 1) * pageSize)
                    .Take(pageSize).ToListAsync();
            }

            var count = list != null ? list.Count() : await entities.CountAsync();
            paginatedList.PageCount = (int)Math.Ceiling(count / (double)pageSize);
            paginatedList.PageIndex = pageIndex;
            paginatedList.Count = count;

            return paginatedList;
        }

    }
}
