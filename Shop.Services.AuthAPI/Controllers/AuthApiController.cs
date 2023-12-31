﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Shop.Services.AuthAPI.Models.Dto;
using Shop.Services.AuthAPI.Service.IService;

namespace Shop.Services.AuthAPI.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthApiController : ControllerBase
    {
        private readonly IAuthService _authService;
        protected ResponseDto _responseDto;

        public AuthApiController(IAuthService authService)
        {
            _authService = authService;
            _responseDto = new();
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegistrationRequestDto model)
        {
            var errorMessage = await _authService.Register(model);
            if (!string.IsNullOrEmpty(errorMessage))
            {
                _responseDto.IsSuccess = false;
                _responseDto.Message = errorMessage;
                
                return BadRequest(_responseDto);
            }
            else
            {
                _responseDto.IsSuccess = true;
            }

            return Ok(_responseDto);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto requestDto)
        {
            var loginResponse = await _authService.Login(requestDto);

            if(loginResponse.User == null)
            {
                _responseDto.IsSuccess=false;
                _responseDto.Message = "Username or password is incorrect";
                return BadRequest(_responseDto);
            }
            _responseDto.IsSuccess = true;
            _responseDto.Result = loginResponse;
            
            return Ok(_responseDto);
        }

        [HttpPost("AssignRole")]
        public async Task<IActionResult> AssignRole([FromBody] RegistrationRequestDto model)
        {
            var assignRole = await _authService.AssignRole(model.Email, model.Role.ToLower());

            if (!assignRole)
            {
                _responseDto.IsSuccess = false;
                _responseDto.Message = "Error while adding role";
                return BadRequest(_responseDto);
            }
            _responseDto.IsSuccess = true;

            return Ok(_responseDto);
        }

    }
}
