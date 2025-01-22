using FutureCoreBackend.Models;
using Microsoft.EntityFrameworkCore.Storage;
using System;

namespace FutureCoreBackend.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        public IRepository<Employee> EmployeeRepo { get; private set; }
        public IRepository<FamilyMember> FamilyMemberRepo { get; private set; }

        public IFamilyMemberRepo FamilyMemberRepository { get; private set; }

        private IDbContextTransaction currentTransaction;
        private Context Context;

        public UnitOfWork(Context _context, IFamilyMemberRepo _FamilyMemberRepository)
        {
            Context = _context;
            EmployeeRepo = new Repository<Employee>(this.Context);
            FamilyMemberRepo = new Repository<FamilyMember>(this.Context);
            FamilyMemberRepository = _FamilyMemberRepository;
        }

        public async Task<IDbContextTransaction> BeginTransactionAsync()
        {
            currentTransaction = await Context.Database.BeginTransactionAsync();
            return currentTransaction;
        }

        public async Task<int> CommitAsync()
        {
            try
            {
                return await Context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while committing changes.", ex);
            }
        }

        public async Task RollbackAsync()
        {
            await currentTransaction.RollbackAsync();
        }

        public void Dispose()
        {
            Context.Dispose();
        }
    }
}
