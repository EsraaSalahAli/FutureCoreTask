using FutureCoreBackend.Models;
using FutureCoreBackend.Repository;
using Microsoft.EntityFrameworkCore.Storage;

namespace FutureCoreBackend.Repository
{
    public interface IUnitOfWork
    {
        IRepository<Employee> EmployeeRepo { get; }
        IRepository<FamilyMember> FamilyMemberRepo { get; }

        IFamilyMemberRepo FamilyMemberRepository { get; }
        Task<IDbContextTransaction> BeginTransactionAsync();
        Task RollbackAsync();
        Task<int> CommitAsync();
    }
}
