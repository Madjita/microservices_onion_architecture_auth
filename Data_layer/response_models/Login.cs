using System;
using System.ComponentModel.DataAnnotations;
using Data_layer.EF_entities;

namespace Data_layer.response_models
{
    public class Login
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        public int Code { get; set; }



        //methods
        //transform object model to Entitiy
        public Account GetEntity()
        {
            Account entity = null;

            entity = new Account
            {
                Email = this.Email,
                Password = this.Password,
                Code = this.Code
            };

            return entity;
        }
    }
}

