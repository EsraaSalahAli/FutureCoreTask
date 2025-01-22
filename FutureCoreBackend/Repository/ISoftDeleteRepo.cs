using System.ComponentModel;

namespace FutureCoreBackend.Repository
{
    public interface ISoftDeleteRepo
    {
        [DefaultValue(false)]
        public bool IsDeleted { get; set; }
    }
}
