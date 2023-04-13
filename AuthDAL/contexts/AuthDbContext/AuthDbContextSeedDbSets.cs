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
                Email = "xok-s@yandex.ru",
                Password = BCrypt.Net.BCrypt.HashPassword("123"),
                RoleId = 1,
                twoFactorAuthentication = true,
                Code = new Random().Next(0, 999999)
            }
        };

        #endregion

        result.Roles = new List<Role>
        {
            new() { Name = "admin" },
            new() { Name = "guest" }
        };

        return result;
    }
}