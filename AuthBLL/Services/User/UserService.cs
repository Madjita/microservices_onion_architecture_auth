using System;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using AuthBLL.Services.Repository;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Net.Mail;
using AuthBLL.Services.SmtpEmailSender;
using System.Text;
using AuthDAL.Entities;
using static AuthDAL.response_models.Login;
using AuthBLL.Handlers;
using AuthDAL.Models;
using AuthDAL.Dtos;

namespace AuthBLL.Services.User
{
    public class UserService : IUserService
    {
        private readonly IRepository<Account> _accountRepository;
        private readonly IRepository<Role> _roleRepository;
        private readonly IEmailSender _emailSender;
        private readonly IAuthHandler _authHandler;

        public UserService(
            IRepository<Account> account_repository,
            IRepository<Role> role_repository,
            IEmailSender emailSender,
            IAuthHandler authHandler
        )
        {
            _accountRepository = account_repository;
            _roleRepository = role_repository;

            _emailSender = emailSender;
            _authHandler = authHandler;
        }

        private async Task<Account> GetAccountFromBD((StringType,Account) modelTuple)
        {
            var model = modelTuple.Item2;
            var modelType = modelTuple.Item1;

            var account = modelType switch
            {
                StringType.Email => await _accountRepository
                    .Get()
                    .Where(x => x.Email == model.Email)
                    .FirstOrDefaultAsync(),
                StringType.NickName => await _accountRepository
                    .Get()
                    .Where(x => x.NickName == model.NickName)
                    .FirstOrDefaultAsync(),
                StringType.PhoneNumber => await _accountRepository
                    .Get()
                    .Where(x => x.PhoneNumber == model.PhoneNumber)
                    .FirstOrDefaultAsync(),
                _ => null // handle unknown StringType values
            };
            
            return account;
        }

        private bool GetCheckBCryptEmailOrNickNameOrPhone((StringType,Account) modelTuple,Account account)
        {
            var model = modelTuple.Item2;
            var modelType = modelTuple.Item1;
            var checkBCryptEmailOrNickNameOrPhone = modelType switch
            {
                StringType.Email => !BCrypt.Net.BCrypt.Verify(model.Password, account.Password) && !BCrypt.Net.BCrypt.Verify(model.Email, account.Email),
                StringType.NickName => !BCrypt.Net.BCrypt.Verify(model.Password, account.Password) && !BCrypt.Net.BCrypt.Verify(model.NickName, account.NickName),
                StringType.PhoneNumber => !BCrypt.Net.BCrypt.Verify(model.Password, account.Password) && !BCrypt.Net.BCrypt.Verify(model.PhoneNumber, account.PhoneNumber),
                _ => false // handle unknown StringType values
            };
            return checkBCryptEmailOrNickNameOrPhone;
        }

        public async Task<(IDtoResultBase, Account)> AuthenticateAsync((StringType,Account) modelTuple)
        {
             if (modelTuple == default)
            {
                throw new Exception("Error in Tuple Model.");
            }

            var model = modelTuple.Item2;
            var modelType = modelTuple.Item1;

            var account = await GetAccountFromBD(modelTuple);

            if (account == null)
            {
                throw new Exception("Error Account don't find.");
            }

            var checkBCryptEmailOrNickNameOrPhone = GetCheckBCryptEmailOrNickNameOrPhone(modelTuple,account);

            if (checkBCryptEmailOrNickNameOrPhone)
            {
                throw new Exception("Error Email or Password or NikName no correct. Try again.");
            }

            if (!account.UserAuthenticated)
            {
                throw new Exception("Error Account did not confirm authentication.");
            }

            await _accountRepository.LoadReferenceAsync(account, _ => _.Role);

            if(account.TwoFactorAuthentication)
            {
                //Сгенерировать случайный код
                //Создание объекта для генерации чисел
                Random rnd = new Random();

                //Получить случайное число (в диапазоне от 0 до 999999)
                int value = rnd.Next(0, 999999);

                account.CodeTwoFactorAuthentication = value;

                StringBuilder body = new StringBuilder();

                body.Append($"<table style=\"border-collapse:collapse\">");
                body.Append("<tbody>");
                body.Append("<tr>");
                body.Append("<td style=\"border-collapse:collapse;font:14px/22px 'arial' , sans-serif;vertical-align:top\">");
                body.Append("<p class=\"325413c6a9ddf781paragraph\" style=\"margin:0\">");
                body.Append($"<b>{value}</b>");
                body.Append("</p>");
                body.Append("</td>");
                body.Append("<td style=\"border-collapse:collapse;font:14px/22px 'arial' , sans-serif;padding-left:10px;vertical-align:top\">");
                body.Append("<p class=\"325413c6a9ddf781paragraph\" style=\"margin:0\">— Ваш код для авторизации&nbsp;</p>");
                body.Append("</td>");
                body.Append("<td style=\"border-collapse:collapse;font:14px/22px 'arial' , sans-serif;padding-left:10px;vertical-align:top\">");
                body.Append("<p class=\"325413c6a9ddf781paragraph\" style=\"margin:0\">( Your access code)</p>");
                body.Append("</td>");
                body.Append("</tr>");
                body.Append("</tbody>");
                body.Append("</table>");


                _ = _accountRepository.UpdateAsync(account);
                _ = _emailSender.SendEmailAsync(account.Email, "Access code", body.ToString()); //$"<h1>Code: {value}</h1>"

                return (new AuthSignInResultDto
                {
                    Email = account.Email,
                }, account);
            }

            var result = await _authHandler.SignIn(account);
            return result;
        }

        public async Task<(IDtoResultBase, Account)> TwoFactorAuthenticationAsync((StringType,Account) modelTuple)
        {
            if (modelTuple == default)
            {
                throw new Exception("Error in Tuple Model.");
            }

            var model = modelTuple.Item2;
            var modelType = modelTuple.Item1;

            var account = await GetAccountFromBD(modelTuple);

            if (account == null)
            {
                throw new Exception("Error Account don't find.");
            }

            if(account.CodeTwoFactorAuthentication != model.CodeTwoFactorAuthentication)
            {
                throw new Exception("Error write code from client don't correct. Try again.");
            }

            var checkBCryptEmailOrNickNameOrPhone = GetCheckBCryptEmailOrNickNameOrPhone(modelTuple,account);

            if (checkBCryptEmailOrNickNameOrPhone)
            {
                throw new Exception("Error Email or Password or NikName no correct. Try again.");
            }

            if (!account.UserAuthenticated)
            {
                throw new Exception("Error Account did not confirm authentication.");
            }
            
            await _accountRepository.LoadReferenceAsync(account, _ => _.Role);

            var result = await _authHandler.SignIn(account);

            return result;
        }

        public async Task RegisterAsync(Account register_model)
        {
            if (register_model == null)
            {
               throw new Exception("Error in Tuple Model.");
            }

            var account = await _accountRepository
                .QueryAll(false)
                .AnyAsync(x => x.Email == register_model.Email);

            if (account)
            {
                //В базе уже существует данный пользователь
                throw new Exception("Error This user already exists in the database. Try again.");
            }

            string saveModelPassword = register_model.Password;
            register_model.Password = BCrypt.Net.BCrypt.HashPassword(saveModelPassword);

            if (!BCrypt.Net.BCrypt.Verify(saveModelPassword,register_model.Password))
            {
                throw new Exception("Error Password no correct. Try again.");
            }

            register_model.Role = await _roleRepository.SingleOrDefaultAsync(x => x.Name == "guest");

            if(register_model.Role == null)
            {
                //В базе нет роли guest
                throw new Exception("There is no guest role in the database.");
            }

            //Отправить код подтверждения регистрации на почту.
            //Сгенерировать случайный код
            //Создание объекта для генерации чисел
            Random rnd = new Random();
            //Получить случайное число (в диапазоне от 0 до 999999)
            int value = rnd.Next(0, 999999);
            register_model.CodeForApproveRegistration = value;
            
            string body = $"<table style=\"border-collapse:collapse\">" +
              $"<tbody>" +
              $"<tr>" +
              $"<td style=\"border-collapse:collapse;font:14px/22px 'arial' , sans-serif;vertical-align:top\">" +
              $"<p class=\"325413c6a9ddf781paragraph\" style=\"margin:0\">" +
              $"<b>{register_model.CodeForApproveRegistration}</b>" +
              $"</p>" +
              $"</td>" +
              $"<td style=\"border-collapse:collapse;font:14px/22px 'arial' , sans-serif;padding-left:10px;vertical-align:top\">" +
              $"<p class=\"325413c6a9ddf781paragraph\" style=\"margin:0\">" +
              $"— Ваш код для подтверждения регистрации&nbsp;" +
              "<a href=\"https://95.188.89.10:5000/confirm-registration?accessCode=" + register_model.CodeForApproveRegistration+"\" style=\"color:blue;text-decoration:none;\">Подтверждение регистрации</a>" +
              $"</p>" +
              $"</td>" +
              $"<td style=\"border-collapse:collapse;font:14px/22px 'arial' , sans-serif;padding-left:10px;vertical-align:top\">" +
              $"<p class=\"325413c6a9ddf781paragraph\" style=\"margin:0\">" +
              $"( Your access code for approve registration)" +
              $"</p>" +
              $"</td>" +
              $"</tr>" +
              $"</tbody>" +
              $"</table>";
            
            await _accountRepository.AddAsync(register_model);
            _ = _emailSender.SendEmailAsync(register_model.Email, "Access code for approve registration", body); //$"<h1>Code: {value}</h1>"

        }

        public async Task<Account> GetByIdAsync(int id)
        {
            return await _accountRepository.GetByIdAsync(id);
        }

        public async Task ApproveRegistrationAsync(int  accessCode)
        {
            if (accessCode == default)
            {
                throw new Exception("Error in Tuple Model.");
            }

            var account = await _accountRepository
                    .Get()
                    .Where(x => x.CodeForApproveRegistration == accessCode)
                    .FirstOrDefaultAsync();

            if (account == null)
            {
                throw new Exception("Error Account don't find.");
            }
            
            account.UserAuthenticated = true;
            await _accountRepository.UpdateAsync(account);

            await _accountRepository.LoadReferenceAsync(account, _ => _.Role);

            //TODO: информировать front о том что аунтификация прошла успешно.
            _ = _authHandler.SignIn(account);
        }

        public Task<IDtoResultBase> LogoutAsync()
        {
           return _authHandler.SignOut();
        }
    }
}

