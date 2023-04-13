using System;
using System.Numerics;
using AuthDAL.Entities.Base;

namespace AuthDAL.Entities
{
    public class Account : EntityBase
    {

        public string Email { get; set; }
        public string Password { get; set; }

        public int Code { get; set; }
        public bool twoFactorAuthentication { get; set; }

        public int RoleId { get; set; }
        public Role Role { get; set; }
    }

}

