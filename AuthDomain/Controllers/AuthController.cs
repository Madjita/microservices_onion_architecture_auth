using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuthDAL.response_models;
using AuthDAL.send_models;
using AuthBLL.Bearer.Auth.Auth.JWT;
using AuthBLL.Services.User;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace AuthDomain.Controllers
{
    public class AuthController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly IOptions<AuthOptions> _authOptrions;

        private readonly IUserService _userService;


        public AuthController(IOptions<AuthOptions> authOptrions, IConfiguration configuration,IUserService userService)
        {
            _authOptrions = authOptrions;
            _configuration = configuration;

            _userService = userService;
        }

        [Route("login")]
        [HttpPost]
        public async Task<IActionResult> LoginAsync([FromBody] Login login_request)
        {
            if(login_request == null)
            {
                return Unauthorized();
            }

            var response = await _userService.AuthenticateAsync(login_request.GetEntity());

            if (response == null)
            {
                return Unauthorized();
            }


            var jwt = response.twoFactorAuthentication ? null: _configuration.GenerateJwtToken(_authOptrions, response);

            return Ok(new LoginSend_model
            {
                Email = response.Email,
                Role  = new RoleSend_model
                {
                    Name = response.Role.Name,
                },
                Access_token = jwt
            });
        }

        [Route("login")]
        [HttpPatch]
        public async Task<IActionResult> GetLoginCodeAsync([FromBody] Login login_request)
        {
        
            var response = await _userService.TwoFactorAuthenticationAsync(login_request.GetEntity());

            if (response == null)
            {
                return BadRequest(new { message = "Didn't register!" });
            }


            var jwt = _configuration.GenerateJwtToken(_authOptrions, response);

            return Ok(new LoginSend_model
            {
                Email = response.Email,
                Role = new RoleSend_model
                {
                    Name = response.Role.Name,
                },
                Access_token = jwt
            });
        }


        [Route("register")]
        [HttpPost]
        public async Task<IActionResult> RegisterAsync([FromBody] Login loginRegister_request)
        {
            if (loginRegister_request == null)
            {
                return BadRequest(new { message = "Didn't register!" });
            }

            var response = await _userService.RegisterAsync(loginRegister_request.GetEntity());

            if (response == null)
            {
                return BadRequest(new { message = "Didn't register!" });
            }

            return Ok();
        }


    }
}

