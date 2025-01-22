using FutureCoreBackend.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace FutureCoreBackend.DTO
{
    public class FamilyMemberDTO
    {
        public string Name { get; set; }
        public DateTime BirthDate { get; set; }
        public string Relationship { get; set; }
        public int EmployeeId { get; set; }
    }
}
