using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using PromoCodeFactory.Core.Domain;
using PromoCodeFactory.Core.Domain.Administration;

namespace PromoCodeFactory.Core.Abstractions.Repositories
{
    public interface IRepository<T> where T: BaseEntity
    {
        Task<IList<T>> GetAllAsync(CancellationToken cancellationToken);

        Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

        Task<bool> RemoveByIdAsync(Guid id, CancellationToken cancellationToken);

        Task<T> UpdateByIdAsync(Guid id, T employee, CancellationToken cancellationToken);

        Task<T> AddAsync(T employee, CancellationToken cancellationToken);
    }
}