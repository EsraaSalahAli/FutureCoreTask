using FutureCoreBackend.Models;
using Microsoft.EntityFrameworkCore;


namespace FutureCoreBackend.Repository
{
    public class FamilyMemberRepo : IFamilyMemberRepo
    {
        private readonly Context Context;
        public FamilyMemberRepo(Context _context)
        {
            Context = _context;
        }

        public List<FamilyMember> GetAllWithInclude()
        {
            return Context.FamilyMembers
                .Include(fm => fm.Employee)
                .ToList();
        }
    }
}
