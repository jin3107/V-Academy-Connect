namespace VAcademyConnect.DTOs.Responses
{
    public class StudentResponse
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public Guid? TenantId { get; set; }
        public string? TenantName { get; set; }
    }
}
