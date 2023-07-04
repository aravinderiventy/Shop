using Shop.Web.Models;
using Shop.Web.Service.IService;
using Shop.Web.Utility;

namespace Shop.Web.Service
{
    public class AuthService : IAuthService
    {
        private readonly IBaseService _baseService;
        public AuthService(IBaseService baseService)
        {
            _baseService = baseService;
        }

        public async Task<ResponseDto?> AssignRoleAsync(RegistrationRequestDto registrationRequestDto) => await _baseService.SendAsync(new RequestDto()
        {
            ApiType = Enums.ApiType.POST,
            Data = registrationRequestDto,
            Url = $"{Enums.AuthAPIBase}/api/auth/AssignRole"
        });

        public async Task<ResponseDto?> LoginAsync(LoginRequestDto loginRequestDto)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = Enums.ApiType.POST,
                Data = loginRequestDto,
                Url = $"{Enums.AuthAPIBase}/api/auth/login"
            },withBearer:false);
        }

        public async Task<ResponseDto?> RegisterAsync(RegistrationRequestDto registrationRequestDto)
        {
            return await _baseService.SendAsync(new RequestDto() 
            { 
                ApiType = Enums.ApiType.POST, 
                Data = registrationRequestDto, 
                Url = $"{Enums.AuthAPIBase}/api/auth/register" 
            },withBearer:false);
        }
    }
}
