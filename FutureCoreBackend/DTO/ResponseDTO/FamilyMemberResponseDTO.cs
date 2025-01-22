namespace FutureCoreBackend.DTO
{
    public class FamilyMemberResponseDTO
    {
        public int Id { set; get; }
        public string Name { get; set; }
        public DateTime BirthDate { get; set; }
        public string Relationship { get; set; }
        public int Age { get; set; }
        public string EmployeeName { get; set; }
    }
}
