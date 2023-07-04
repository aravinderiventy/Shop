using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using Shop.Web.Models;
using Shop.Web.Service.IService;
using Shop.Web.Utility;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Shop.Web.Controllers
{
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;
        private readonly ITokenProvider _tokenProvider;

        public AuthController(IAuthService authService, ITokenProvider tokenProvider)
        {
            _authService = authService;
            _tokenProvider = tokenProvider;

        }

        [HttpGet]
        public IActionResult Login()
        {
            LoginRequestDto loginRequestDto = new();
            _tokenProvider.ClearToken();
           
            return View(loginRequestDto);
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginRequestDto dto)
        {
            ResponseDto responseDto = await _authService.LoginAsync(dto);

            if (responseDto != null && responseDto.IsSuccess)
            {
                LoginResponseDto loginResponseDto = JsonConvert.DeserializeObject<LoginResponseDto>(Convert.ToString(responseDto.Result));
                await SignInUser(loginResponseDto);
                _tokenProvider.SetToken(loginResponseDto.Token);
                
                return RedirectToAction("Index", "Home");
            }
            else
            {
                TempData["error"] = responseDto.Message;

                return View(dto);
            }
        }

        [HttpGet]
        public IActionResult Register()
        {
            var roleList = new List<SelectListItem>()
            {
                new SelectListItem{Text=Enums.RoleAdmin, Value=Enums.RoleAdmin},
                new SelectListItem{Text=Enums.RoleCustomer, Value=Enums.RoleCustomer }
            };
            ViewBag.RoleList = roleList;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegistrationRequestDto dto)
        {
            var registerRoleResponse = await _authService.RegisterAsync(dto);
            ResponseDto assignRoleResponse;

            if (registerRoleResponse != null && registerRoleResponse.IsSuccess)
            {
                if (string.IsNullOrEmpty(dto.Role))
                {
                    dto.Role = Enums.RoleCustomer;
                }
                assignRoleResponse = await _authService.AssignRoleAsync(dto);
                if (assignRoleResponse != null && assignRoleResponse.IsSuccess)
                {
                    TempData["success"] = "Registration Successful";
                    
                    return RedirectToAction(nameof(Login));
                }
            }
            else
                TempData["error"] = registerRoleResponse?.Message??"Failed to register user.";
            
            var roleList = new List<SelectListItem>()
            {
                new SelectListItem{Text=Enums.RoleAdmin, Value=Enums.RoleAdmin},
                new SelectListItem{Text=Enums.RoleCustomer, Value=Enums.RoleCustomer }
            }; 
            ViewBag.RoleList = roleList;
            
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            _tokenProvider.ClearToken();
            
            return RedirectToAction("Index", "Home");
        }


        private async Task SignInUser(LoginResponseDto dto)
        {
            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(dto.Token);
            var identity =new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
            identity.AddClaim(new Claim(JwtRegisteredClaimNames.Email, jwt.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Email).Value));
            identity.AddClaim(new Claim(JwtRegisteredClaimNames.Sub, jwt.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub).Value));
            identity.AddClaim(new Claim(JwtRegisteredClaimNames.Name, jwt.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Name).Value));

            identity.AddClaim(new Claim(ClaimTypes.Name, jwt.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Email).Value));
            identity.AddClaim(new Claim(ClaimTypes.Role, jwt.Claims.FirstOrDefault(c => c.Type == "role").Value));

            var principal = new ClaimsPrincipal(identity);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
        }
    }
}
