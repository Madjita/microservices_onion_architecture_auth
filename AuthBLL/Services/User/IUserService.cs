using System;
using System.Threading.Tasks;
using AuthDAL.Entities;
using AuthDAL.Models;
using static AuthDAL.response_models.Login;

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
        Task<(IDtoResultBase, Account)> AuthenticateAsync((StringType,Account) model);
        Task RegisterAsync(Account register_model);
    }

    public interface IUserServiceAuthenticationTwoFactor
    {
        Task<(IDtoResultBase, Account)> TwoFactorAuthenticationAsync((StringType,Account) model);
        Task ApproveRegistrationAsync(int accessCode);
    }
}

