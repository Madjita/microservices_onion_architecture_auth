using System;
using AuthDAL.EF_entities;
using Microsoft.EntityFrameworkCore;

namespace AuthDAL.Data
{
    public class Context : DbContext
    {
        public Context(DbContextOptions<Context> options)
        : base(options)
        {
            //Database.EnsureDeleted();
            //Database.EnsureCreated();
        }



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
          modelBuilder.Entity<Role>().HasData(
          new Role[]
          {
                new Role{Id = 1, Name = "admin"},
                new Role{Id = 2, Name = "guest"}
          });

            modelBuilder.Entity<Account>().HasData(
            new Account[]
            {
                new Account{
                    Id = 1,
                    Email = "xok-s@yandex.ru",
                    Password = BCrypt.Net.BCrypt.HashPassword("123"),
                    RoleId = 1,
                    twoFactorAuthentication = true,
                    Code = new Random().Next(0, 999999)
                }
            });
        }
    }
}

