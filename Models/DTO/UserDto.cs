namespace KinFlix.Service.AuthAPI.Models.DTO
{
    public class UserDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public bool PhoneNumberComfirmed { get; set; } = false;
    }
}
