using System;
using System.Collections.Generic;
using System.Linq;
using AuthDAL.Entities;

namespace MobileDrill.DataBase.Data;

public class AuthDbContextSeedDbSets
{
    public List<Account> Accounts { get; set; } = new();
    public List<Role> Roles { get; set; } = new();
}

public partial class AuthDbContext
{
    /// <summary>
    ///     Seeds database with initial static data
    /// </summary>
    /// <returns></returns>
    /// <exception cref="ApplicationException"></exception>
    public static AuthDbContextSeedDbSets SeedDbSets()
    {
        var result = new AuthDbContextSeedDbSets();

        #region Account

        result.Accounts = new List<Account>
        {
            new()
            {
                Id = 1,
                Email = "xok-s@yandex.ru",
                Password = BCrypt.Net.BCrypt.HashPassword("123"),
                RoleId = 1,
                TwoFactorAuthentication = true,
                CodeTwoFactorAuthentication = new Random().Next(0, 999999),
                UserAuthenticated = true,

            },
            new()
            {
                Id = 2,
                Email = "madjita@mail.ru",
                Password = BCrypt.Net.BCrypt.HashPassword("1234"),
                RoleId = 2,
                TwoFactorAuthentication = false,
                CodeTwoFactorAuthentication = new Random().Next(0, 999999),
                UserAuthenticated = true,

            }
        };

        #endregion

        result.Roles = new List<Role>
        {
            new() { Id = 1, Name = "admin" },
            new() { Id = 2, Name = "guest" }
        };

        return result;
    }
}