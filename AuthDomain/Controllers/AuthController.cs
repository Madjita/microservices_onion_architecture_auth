using System.Threading.Tasks;
using AuthBLL.Handlers;
using AuthBLL.Services.User;
using AuthDAL.Models;
using AuthDAL.response_models;
using AuthDAL.send_models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Swashbuckle.AspNetCore.Annotations;
using AuthenticationSchemes = AuthDAL.Auth.AuthService.AuthenticationSchemes;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace AuthDomain.Controllers
{
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.JsonWebToken)]
    public class AuthController : Controller
    {
        private readonly IConfiguration _configuration;
        // private readonly IOptions<AuthOptions> _authOptrions;
        private readonly IAuthHandler _authHandler;
        private readonly IUserService _userService;


        public AuthController(
            IConfiguration configuration,
            // IOptions<AuthOptions> authOptrions, 
            IUserService userService,
            IAuthHandler authHandler
        )
        {
            // _authOptrions = authOptrions;
            _configuration = configuration;

            _userService = userService;
            _authHandler = authHandler;
        }

        // [Route("login")]
        // [HttpPost]
        // [AllowAnonymous]
        // [SwaggerOperation(Summary = "Signs in as a User account", Description = "You must use User account in order to interact with the rest of the API")]
        // [ProducesResponseType(typeof(LoginSend_model), StatusCodes.Status200OK)]
        // public async Task<IActionResult> LoginAsync([FromBody] Login login_request)
        // {
        //     if(login_request == null)
        //     {
        //         return Unauthorized();
        //     }
            
        //     var response = await _userService.AuthenticateAsync(login_request.GetEntity());

        //     if (response == null)
        //     {
        //         return Unauthorized();
        //     }


        //     var jwt = response.twoFactorAuthentication ? null: _configuration.GenerateJwtToken(_authOptrions, response);

        //     return Ok(new LoginSend_model
        //     {
        //         Email = response.Email,
        //         Role  = new RoleSend_model
        //         {
        //             Name = response.Role.Name,
        //         },
        //         Access_token = jwt
        //     });
        // }

        [Route("login")]
        [HttpPost]
        [AllowAnonymous]
        [SwaggerOperation(Summary = "Signs in as a User account", Description = "You must use User account in order to interact with the rest of the API")]
        [ProducesResponseType(typeof(LoginSend_model), StatusCodes.Status200OK)]
        public async Task<IActionResult> LoginAsync([FromBody] Login login_request)
        {
            try
            {
                var response = await _userService.AuthenticateAsync(login_request.GetEntity());
                return Ok(response.Item1);
            }
            catch (System.Exception e)
            {
                return Unauthorized(e.Message);
            }
        }

        [Route("login")]
        [HttpPatch]
        [AllowAnonymous]
        public async Task<IActionResult> GetLoginCodeAsync([FromBody] LoginTwoFactorAuthentication login_request)
        {
            
            try
            {
                var response = await _userService.TwoFactorAuthenticationAsync(login_request.GetEntity());
                return Ok(response.Item1);
            }
            catch (System.Exception e)
            {
                return BadRequest(new { message = $"Didn't register! : {e.Message}" });
            }
        }


        [Route("registration")]
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> RegisterAsync([FromBody] Register loginRegister_request)
        {
            try
            {
                await _userService.RegisterAsync(loginRegister_request.GetEntity());
                return Ok();
            }
            catch (System.Exception e)
            {
                return BadRequest(new { message = $"Didn't register! : {e.Message}" });
            }
        }

        [Route("confirm-registration")]
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetRegisterCodeAsync(int accessCode) //RegisterApproveRegistration loginRegister_request
        {
            try
            {
                await _userService.ApproveRegistrationAsync(accessCode);
                return Ok();
            }
            catch (System.Exception e)
            {
                return BadRequest(new { message = $"Didn't register! : {e.Message}" });
            }
        }
        
    }
}

