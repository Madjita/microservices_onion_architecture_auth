﻿using System;
using AuthDAL.EF_entities;
using System.Threading.Tasks;

namespace AuthBLL.Services.User
{
    public interface IUserService :
        IUserServiceRepository,
        IUserServiceAuthenticate,
        IUserServiceAuthenticationTwoFactor
    {

    }

    public interface IUserServiceRepository
    {
        Task<Account> GetByIdAsync(int id);
    }

    public interface IUserServiceAuthenticate
    {
        Task<Account> AuthenticateAsync(Account model);
        Task<Account> RegisterAsync(Account register_model);
    }

    public interface IUserServiceAuthenticationTwoFactor
    {
        Task<Account> TwoFactorAuthenticationAsync(Account model);
    }
}
