using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain;

namespace PromoCodeFactory.DataAccess.Repositories
{
    public class InMemoryRepository<T>(IList<T> data) : IRepository<T>
        where T : BaseEntity
    {
        protected IList<T> Data { get; set; } = data;

        public Task<IList<T>> GetAllAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult(Data);
        }

        public Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            var result = Task.FromResult(Data.FirstOrDefault(x => x.Id == id));
            return result;
        }

        public Task<T> UpdateByIdAsync(Guid id, T employee, CancellationToken cancellationToken)
        {
            var item = Data.FirstOrDefault(u => u.Id == id);

            if (item == null)  throw new ArgumentNullException(nameof(id));

            Data.RemoveAt(Data.IndexOf(item));
            Data.Add(employee);
            return Task.FromResult(employee);
        }

        public Task<T> AddAsync(T entity, CancellationToken cancellationToken)
        {
            Data.Add(entity);
            return Task.FromResult(entity);
        }

        public Task<bool> RemoveByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            var item = Data.FirstOrDefault(u => u.Id == id);
            if (item != null)
            {
                Data.RemoveAt(Data.IndexOf(item));
                return Task.FromResult(true);
            }
            return Task.FromResult(false);
        }
    }
}