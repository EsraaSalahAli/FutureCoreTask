using FutureCoreBackend.Repository;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.Net;

namespace FutureCoreBackend.Models
{
    public class Employee : ISoftDeleteRepo
    {
        public int Id { get; set; }
        public string FullName { set; get; }
        public string Phone { set; get; }

        [RegularExpression(@"^([1-9]{1})([0-9]{2})([0-9]{2})([0-9]{2})([0-9]{2})[0-9]{3}([0-9]{1})[0-9]{1}$", 
            ErrorMessage = "The national id is not valid.")]
        public string NationalId { set; get; }
        public string Address { set; get; }
        public string Email { set; get; }  
        public DateTime BirthDate { set; get; }
        public bool IsDeleted { get; set; }
        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public virtual List<FamilyMember> FamilyMembers { set; get; } = new List<FamilyMember>();
    }
}
