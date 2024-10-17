using System.ComponentModel.DataAnnotations.Schema;

namespace KinFlix.Service.AuthAPI.Models.DTO
{
    public class RegistrationRequestDto
    {
        public string Email { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
        public string PhoneNumber { get; set; }
        [NotMapped]
        public string? Role { get; set; }
    }
}
