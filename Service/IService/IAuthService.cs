using KinFlix.Service.AuthAPI.Models.DTO;

namespace KinFlix.Service.AuthAPI.Service.IService
{
    public interface IAuthService
    {
        Task<string> Register(RegistrationRequestDto registrationRequestDto);
        Task<LoginResponseDto> Login(LoginRequestDto loginRequestDto);
        Task<bool> AssignRole(string email, string roleName);
        Task<string> GenerateSecretCode(string phoneNumber);
    }
}
