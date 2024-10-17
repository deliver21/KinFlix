using KinFlix.Service.AuthAPI.Data;
using KinFlix.Service.AuthAPI.Models.DTO;
using KinFlix.Service.AuthAPI.Models;
using KinFlix.Service.AuthAPI.Service.IService;
using Microsoft.AspNetCore.Identity;
using KinFlix.Service.AuthAPI.Utilities;
using RestSharp;
using Azure;
using System.Reflection;
using System.Text;

namespace KinFlix.Service.AuthAPI.Service
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;

        public AuthService(AppDbContext db, RoleManager<IdentityRole> roleManager,
            UserManager<ApplicationUser> userManager, IJwtTokenGenerator jwtTokenGenerator)
        {
            _db = db;
            _jwtTokenGenerator = jwtTokenGenerator;
            _userManager = userManager;
            _roleManager = roleManager;
        }
        //todo AuthService Uncomment line27 43 92
        public async Task<bool> AssignRole(string email, string roleName)
        {
            var user = new ApplicationUser(); /*_db.ApplicationUsers.FirstOrDefault(u => u.Email.ToLower() == email.ToLower());*/
            if (user != null)
            {
                if (!_roleManager.RoleExistsAsync(roleName).GetAwaiter().GetResult())
                {
                    //Create role because it doesn't exist
                    _roleManager.CreateAsync(new IdentityRole(roleName)).GetAwaiter().GetResult();
                }
                await _userManager.AddToRoleAsync(user, roleName);
                return true;
            }
            return false;
        }

        public async Task<LoginResponseDto> Login(LoginRequestDto loginRequestDto)
        {
            var user = new ApplicationUser(); /*_db.ApplicationUsers.FirstOrDefault(u => u.UserName.ToLower() == loginRequestDto.UserName.ToLower());*/
            bool isValid = await _userManager.CheckPasswordAsync(user, loginRequestDto.Password);
            if (user == null || !isValid)
            {
                return new LoginResponseDto()
                {
                    User = null,
                    Token = ""
                };
            }
            //User was found , Generate JWt Token
            // Get token  _jwtTokenGenerator.GenerateToken(user)
            //var roles contains the role infos that need to be loaded in JWT
            var roles = await _userManager.GetRolesAsync(user);
            var token = _jwtTokenGenerator.GenerateToken(user, roles);
            //End

            UserDto userDto = new UserDto()
            {
                Email = user.Email,
                Id = user.Id,
                Name = user.Name,
                PhoneNumber = user.Name
            };
            LoginResponseDto loginResponseDto = new()
            {
                User = userDto,
                //add token
                Token = token
            };
            return loginResponseDto;
        }

        public async Task<string> Register(RegistrationRequestDto registrationRequestDto)
        {
            ApplicationUser user = new()
            {
                UserName = registrationRequestDto.Email,
                Email = registrationRequestDto.Email,
                NormalizedEmail = registrationRequestDto.Email.ToUpper(),
                Name = registrationRequestDto.Name,
                PasswordHash = registrationRequestDto.Password,
                PhoneNumber = registrationRequestDto.PhoneNumber
            };
            try
            {
                var result = await _userManager.CreateAsync(user, registrationRequestDto.Password);
                if (result.Succeeded)
                {
                    var userToReturn = new ApplicationUser(); /*_db.ApplicationUsers.First(u => u.UserName == registrationRequestDto.Email);*/
                    UserDto userDto = new()
                    {
                        Email = userToReturn.Email,
                        Id = userToReturn.Id,
                        Name = userToReturn.Name,
                        PhoneNumber = userToReturn.PhoneNumber
                    };
                    return "";
                }
                else
                {
                    return result.Errors.FirstOrDefault().Description;
                }

            }
            catch (Exception ex)
            {

            }
            return "Error encountered";
        }
        public async Task<string> GenerateSecretCode(string phoneNumber)
        {
            try
            {
                string credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{SD.UserName}:{SD.Password}"));
                var options = new RestClientOptions(SD.APIBaseURL)
                {
                    MaxTimeout = -1,
                };
                var client = new RestClient(options);
                var request = new RestRequest("/sms/2/text/advanced", Method.Post);
                request.AddHeader("Authorization", $"Basic {credentials}");
                //request.AddHeader("Authorization", SD.InfoBipAPIKey);
                request.AddHeader("Content-Type", "application/json");
                request.AddHeader("Accept", "application/json");
                var body = $@"{{""messages"": [{{""destinations"": [{{""to"": ""{phoneNumber}""}}],""from"": ""{SD.Sender}"",
                ""text"": ""Congratulations on sending your first message.\nGo ahead and check the delivery report in the next step.""}}]}}";
                //var body = @"{""messages"":[{""destinations"":[{""to"":""243890111936""}],""from"":""447491163443"",""text"":""Congratulations on sending your first message.\nGo ahead and check the delivery report in the next step.""}]}";
                request.AddStringBody(body, DataFormat.Json);
                RestResponse response = await client.ExecuteAsync(request);
                return response.Content;
            }            
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message.ToString());
                return "";
            }
            
        }
    }
}
