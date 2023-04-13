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

namespace AuthBLL.Services.User
{
    public class UserService : IUserService
    {
        private readonly IRepository<Account> _accountRepository;
        private readonly IRepository<Role> _roleRepository;
        private readonly IEmailSender _emailSender;

        public UserService(
           IRepository<Account> account_repository,
           IRepository<Role> role_repository,
           IEmailSender emailSender
        )
        {
            _accountRepository = account_repository;
            _roleRepository = role_repository;

            _emailSender = emailSender;
        }

        public async Task<Account> AuthenticateAsync(Account model)
        {
            if (model == null)
            {
                return null;
            }

            var account = await _accountRepository
                .Get()
                .Where(x => x.Email == model.Email)
                .FirstOrDefaultAsync();

            if (account == null)
            {
                return null;
            }


            if (!BCrypt.Net.BCrypt.Verify(model.Password, account.Password) &&
                !BCrypt.Net.BCrypt.Verify(model.Email, account.Email))
            {
                return null;
            }

            _accountRepository.GetContext().Entry(account).Reference(x => x.Role).Load();


            if(account.twoFactorAuthentication)
            {
                //Сгенерировать случайный код
                //Создание объекта для генерации чисел
                Random rnd = new Random();

                //Получить случайное число (в диапазоне от 0 до 999999)
                int value = rnd.Next(0, 999999);

                //не ждем когда выполняться запросы так как на данный момент нам это не надо
                account.Code = value;

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

            }


            return account;
        }

        public async Task<Account> TwoFactorAuthenticationAsync(Account model)
        {
            if (model == null)
            {
                return null;
            }

            var account = await _accountRepository
                .Get()
                .Where(x => x.Email == model.Email)
                .FirstOrDefaultAsync();

            if (account == null)
            {
                return null;
            }

            if(account.Code != model.Code)
            {
                return null;
            }


            if (!BCrypt.Net.BCrypt.Verify(model.Password, account.Password) &&
                !BCrypt.Net.BCrypt.Verify(model.Email, account.Email))
            {
                return null;
            }

            _accountRepository.GetContext().Entry(account).Reference(x => x.Role).Load();

            return account;
        }

        public async Task<Account> RegisterAsync(Account register_model)
        {
            if (register_model == null)
            {
                return null;
            }

            var account = await _accountRepository
                .Get()
                .AnyAsync(x => x.Email == register_model.Email);

            if (account)
            {
                //В базе уже существует данный пользователь
                return null;
            }

            string saveModelPassword = register_model.Password;
            register_model.Password = BCrypt.Net.BCrypt.HashPassword(saveModelPassword);

            if (!BCrypt.Net.BCrypt.Verify(saveModelPassword,register_model.Password))
            {
                return null;
            }

            register_model.Role = await _roleRepository.Get().Where(x => x.Name == "guest").FirstOrDefaultAsync();

            if(register_model.Role == null)
            {
                //В базе нет роли guest
                return null;
            }

            var addedAccount = await _accountRepository.AddAsync(register_model);

            return register_model;
        }

        public async Task<Account> GetByIdAsync(int id)
        {
            return await _accountRepository.GetByIdAsync(id);
        }

       
    }
}

