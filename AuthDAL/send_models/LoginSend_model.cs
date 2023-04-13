using System;
using System.ComponentModel.DataAnnotations;

namespace AuthDAL.send_models
{
    public class LoginSend_model
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        public RoleSend_model Role { get; set; }

        public string Access_token { get; set; }
    }
}

