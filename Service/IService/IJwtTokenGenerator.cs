using KinFlix.Service.AuthAPI.Models;

namespace KinFlix.Service.AuthAPI.Service.IService
{
    public interface IJwtTokenGenerator
    {
        //Passing the role through IEnumerable<string> roles , user can have multiple roles
        string GenerateToken(ApplicationUser applicationUser, IEnumerable<string> roles);
    }
}
