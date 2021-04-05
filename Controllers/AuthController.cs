using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AutoMapper;
using FoosballApi.Data;
using FoosballApi.Dtos.Users;
using FoosballApi.Helpers;
using FoosballApi.Models;
using FoosballApi.Models.Accounts;
using FoosballApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace FoosballApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IEmailService _emailService;
        private readonly IUserService _userService;
        private readonly IMapper _mapper;

        public AuthController(IAuthService authService, IUserService userService, IMapper mapper, IEmailService emailService)
        {
            _authService = authService;
            _userService = userService;
            _mapper = mapper;
            _emailService = emailService;
        }

        [HttpPost("login")]
        public IActionResult Authenticate([FromBody]AuthenticateModel model)
        {
            var user = _authService.Authenticate(model.Username, model.Password);

            if (user == null)
                return BadRequest(new { message = "Username or password is incorrect" });
            
            string tokenString = _authService.CreateToken(user);

            return Ok(new
            {
                Id = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Token = tokenString
            });
        }

        [HttpPost("register")]
        public ActionResult<UserReadDto> CreateUser(UserCreateDto userCreateDto)
        {
           var userModel = _mapper.Map<User>(userCreateDto);
           var user = _userService.GetUserByEmail(userCreateDto.Email);

           if (user != null) {
               return Conflict();
           }
           
           _authService.CreateUser(userModel);
           var tmpUser = _userService.GetUserByEmail(userCreateDto.Email);
           var vModel = _authService.AddVerificationInfo(tmpUser, Request.Headers["origin"]);

           var userReadDto = _mapper.Map<UserReadDto>(userModel);

           _emailService.SendVerificationEmail(vModel, tmpUser, Request.Headers["origin"]);

           return CreatedAtRoute(nameof(UsersController.GetUserById), new {Id= userReadDto.Id}, userReadDto);
        }

        [HttpPost("verify-email")]
        public IActionResult VerifyEmail(VerifyEmailRequest model)
        {
            _authService.VerifyEmail(model.Token);
            return Ok(new { message = "Verification successful, you can now login" });
        }

        [HttpPost("forgot-password")]
        public IActionResult ForgotPassword(ForgotPasswordRequest model)
        {
            var verification = _authService.ForgotPassword(model, Request.Headers["origin"]);
            var user = _userService.GetUserByEmail(model.Email);
            _emailService.SendPasswordResetEmail(verification, user, Request.Headers["origin"]);
            return Ok(new { message = "Please check your email for password reset instructions" });
        }

        [HttpPost("reset-password")]
        public IActionResult ResetPassword(ResetPasswordRequest model)
        {
            _authService.ResetPassword(model);
            return Ok(new { message = "Password reset successful, you can now login" });
        }
    }
}