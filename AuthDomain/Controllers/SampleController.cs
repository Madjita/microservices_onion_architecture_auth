using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using AuthenticationSchemes = AuthDAL.Auth.AuthService.AuthenticationSchemes;


namespace AuthDomain.Controllers
{
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.Default)]
    [Route("/")]
    public class SampleController : ControllerBase
    {
        public SampleController()
        {

        }

        [HttpGet]
        public IActionResult Get()
        {
            return Ok();
        }
    }
}