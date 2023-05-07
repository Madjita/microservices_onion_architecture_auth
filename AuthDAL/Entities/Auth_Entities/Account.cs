using System;
using System.ComponentModel.DataAnnotations;
using System.Numerics;
using AuthDAL.Entities.Base;

namespace AuthDAL.Entities
{
    public class Account : EntityBase
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Patronymic { get; set; }

        public string NickName {get;set;}
        public string PhoneNumber {get;set;}
        public string Email { get; set; }
        public string Password { get; set; }

        public int CodeForApproveRegistration {get;set;}
        public bool UserAuthenticated { get; set; }

        public int CodeTwoFactorAuthentication { get; set; }
        public bool TwoFactorAuthentication { get; set; }

        public int RoleId { get; set; }
        public Role Role { get; set; }

        [DataType(DataType.Date)]
        public DateTime LastSignIn { get; set; }
    }

}

