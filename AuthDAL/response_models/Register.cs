using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using AuthDAL.Entities;

namespace AuthDAL.response_models
{
    public class Register
    {
        [Phone]
        public string PhoneNumber { get; set; }
        [Required]
        public string NickName { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
        public bool TwoFactorAuthentication { get; set; }
        public virtual Account GetEntity()
        {
            var entity = new Account
            {
                NickName = this.NickName,
                Email = this.Email,
                Password = this.Password,
                PhoneNumber = this.PhoneNumber,
                TwoFactorAuthentication = this.TwoFactorAuthentication
            };

            return entity;
        }
    }

    public class RegisterApproveRegistration : Register
    {
        public int CodeForApproveRegistration {get;set;}
        public override Account GetEntity()
        {
            var entity = new Account
            {
                NickName = this.NickName,
                Email = this.Email,
                Password = this.Password,
                PhoneNumber = this.PhoneNumber,
                TwoFactorAuthentication = this.TwoFactorAuthentication,
                CodeForApproveRegistration = this.CodeForApproveRegistration
            };

            return entity;
        }
    }
}