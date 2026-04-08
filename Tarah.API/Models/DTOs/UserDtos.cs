namespace Tarah.API.Models.DTOs
{
    public class UserDto
    {
        public Guid Id { get; set; }
        public string Username { get; set; }
        public ListProductsDto ListProductsDto { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public bool IsFull { get; set; }
    }

}
