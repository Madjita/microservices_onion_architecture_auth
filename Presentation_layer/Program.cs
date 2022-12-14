using System.Net;
using System.Net.Mail;
using Data_layer.Data;
using Infrastructure_layer.Bearer.Auth.Auth.JWT;
using Infrastructure_layer.Services.Repository;
using Infrastructure_layer.Services.SmtpEmailSender;
using Infrastructure_layer.Services.User;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var authOptionsConfiguration = builder.Configuration.GetSection("Auth");
builder.Services.Configure<AuthOptions>(authOptionsConfiguration);

builder.Services.AddDbContext<Context>(opt => opt.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"), b=> b.MigrationsAssembly("Data_layer")));

builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddTransient<IUserService, UserService>();

builder.Services.AddControllers();


var smtpClient = new SmtpClient("smtp.mail.ru")
{
    Host = "smtp.mail.ru",
    Port = 587,
    Credentials = new NetworkCredential("madjita@mail.ru", "BPJ61yceJQBXpidNiWPA"),
    EnableSsl = true,
    UseDefaultCredentials = false,

};
builder.Services.AddSingleton(smtpClient);
builder.Services.AddTransient<IEmailSender, SmtpEmailSender>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

