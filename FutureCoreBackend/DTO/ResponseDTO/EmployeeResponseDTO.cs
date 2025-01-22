namespace FutureCoreBackend.DTO.ResponseDTO
{
    public class EmployeeResponseDTO
    {
        public int Id { set; get; }
        public string FullName { set; get; }
        public string Phone { set; get; }
        public string NationalId { set; get; }
        public string Address { set; get; }
        public string Email { set; get; }
        public int Age { set; get; }
        public DateTime BirthDate { set; get; }
        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }
    }
}
