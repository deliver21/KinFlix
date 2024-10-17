using KinFlix.Service.AuthAPI.Models.DTO;
using KinFlix.Service.AuthAPI.Service.IService;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace KinFlix.Service.AuthAPI.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthAPIController : ControllerBase
    {
        private readonly IAuthService _authService;
        protected ResponseDto _response;
        public AuthAPIController(IAuthService authService)
        {
            _authService = authService;
            _response = new();
        }
        [HttpPost("login")]
        public async Task<IActionResult> LogIn([FromBody] LoginRequestDto model)
        {
            var loginResponse = await _authService.Login(model);
            if (loginResponse == null || loginResponse.Token == "")
            {
                _response.IsSuccess = false;
                _response.Message = "Username or password is incorrect";
                return BadRequest(_response);
            }
            // Serialize
            string jsonResponse = JsonConvert.SerializeObject(loginResponse);
            // Deserialize
            var deserializedResponse = JsonConvert.DeserializeObject<LoginResponseDto>(jsonResponse);
            //Success Login
            _response.Result = deserializedResponse;
            _response.IsSuccess = true;
            return Ok(_response);
        }
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegistrationRequestDto model)
        {
            var errorMessage = await _authService.Register(model);
            if (!string.IsNullOrEmpty(errorMessage))
            {
                _response.IsSuccess = false;
                _response.Message = errorMessage;
                return BadRequest(_response);
            }
            return Ok(_response);
        }
        [HttpPost("Assign_Role")]
        public async Task<IActionResult> AssignRole([FromBody] RegistrationRequestDto model)
        {
            var assignRoleSuccessful = await _authService.AssignRole(model.Email, model.Role.ToUpper());
            if (!assignRoleSuccessful)
            {
                _response.IsSuccess = false;
                _response.Message = "Error encountered";
                return BadRequest(_response);
            }
            _response.Result = assignRoleSuccessful;
            return Ok(_response);
        }
        [HttpPost("generatecode")]
        public async Task<ResponseDto> GenerateCode([FromBody] UserDto userDto)
        {
            var code = await _authService.GenerateSecretCode(userDto.PhoneNumber);
            if (string.IsNullOrEmpty(code))
            {
                _response.IsSuccess = false;
                _response.Message = "Operation has failed";
            }
            _response.Result = code;            
            return _response;
        }
    }
}
