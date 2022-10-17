using System;
using System.Numerics;

namespace Data_layer.EF_entities
{
    public class Account : BaseEntity
    {

        public string Email { get; set; }
        public string Password { get; set; }

        public int Code { get; set; }
        public bool twoFactorAuthentication { get; set; }

        public int RoleId { get; set; }
        public Role Role { get; set; }
    }

}

