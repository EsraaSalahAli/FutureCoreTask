using FutureCoreBackend.Models;

namespace FutureCoreBackend.Repository
{
    public interface IFamilyMemberRepo
    {
        List<FamilyMember> GetAllWithInclude();
    }
}
