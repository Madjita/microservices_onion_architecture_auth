using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using AuthDAL.Entities;

namespace AuthDAL.response_models
{
    public class Login
    {
        
        public enum StringType
        {
            Email,
            PhoneNumber,
            NickName,
            Unknown
        }
        
        [Required]
        // [EmailAddress]
        public string EmailOrNickNameOrPhone { get; set; }

        [Required]
        public string Password { get; set; }

        

        //methods
        //transform object model to Entitiy
        public virtual (StringType,Account) GetEntity()
        {
            var entity = new Account
            {
                Password = this.Password,
            };

            var takeType = GetStringType(EmailOrNickNameOrPhone);
            switch (takeType)
            {
                case StringType.Email : 
                    entity.Email = EmailOrNickNameOrPhone;
                    break;
                case StringType.NickName:
                    entity.NickName = EmailOrNickNameOrPhone;
                    break;
                case StringType.PhoneNumber:
                    entity.PhoneNumber = EmailOrNickNameOrPhone;
                    break;
                default:
                    entity.NickName = EmailOrNickNameOrPhone;
                    break;
            }

            return (takeType,entity);
        }

        protected StringType GetStringType(string inputString)
        {
            if (IsValidEmail(inputString))
            {
                return StringType.Email;
            }
            else if (IsValidPhone(inputString))
            {
                return StringType.PhoneNumber;
            }
            else
            {
                return StringType.NickName;
            }
        }

        bool IsValidEmail(string email)
        {
            // Валидация e-mail по регулярному выражению
            // В данном примере используется простейшая проверка на наличие символов "@" и "."
            return email.Contains("@") && email.Contains(".");
        }

        bool IsValidPhone(string phone)
        {
            // Валидация номера телефона по регулярному выражению
            // В данном примере используется простейшая проверка на наличие цифр
            return phone.All(char.IsDigit);
        }
    }

    public class LoginTwoFactorAuthentication : Login
    {
        public int CodeTwoFactorAuthentication { get; set; }

        public override (StringType,Account) GetEntity()
        {
            var entity = new Account
            {
                Password = this.Password,
                CodeTwoFactorAuthentication = this.CodeTwoFactorAuthentication
            };

            var takeType = GetStringType(EmailOrNickNameOrPhone);
            switch (takeType)
            {
                case StringType.Email : 
                    entity.Email = EmailOrNickNameOrPhone;
                    break;
                case StringType.NickName:
                    entity.NickName = EmailOrNickNameOrPhone;
                    break;
                case StringType.PhoneNumber:
                    entity.PhoneNumber = EmailOrNickNameOrPhone;
                    break;
                default:
                    entity.NickName = EmailOrNickNameOrPhone;
                    break;
            }

            return (takeType,entity);
        }
    }
}

