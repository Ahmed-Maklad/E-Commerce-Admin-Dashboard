namespace AllBirds.DTOs.AccountDTOs
{
    public class ClientDetailsDTO
    {
        public int Id { get; set; }
        public string? Email { get; set; }
        public string? UserName { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Country { get; set; }
        public string? City { get; set; }
        public string? Address { get; set; }
        public string? ZipCode { get; set; }
        public string? ImagePath { get; set; }
    }
}
