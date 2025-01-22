using FutureCoreBackend.Repository;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations.Schema;

namespace FutureCoreBackend.Models
{
    public class FamilyMember : ISoftDeleteRepo
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime BirthDate { get; set; }
        public string Relationship { get; set; }

        [ForeignKey("Employee")]
        public int EmployeeId { get; set; }

        public virtual Employee Employee { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }
    }
}
