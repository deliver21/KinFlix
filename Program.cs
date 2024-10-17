using KinFlix.Service.AuthAPI.Data;
using KinFlix.Service.AuthAPI.Models;
using KinFlix.Service.AuthAPI.Service;
using KinFlix.Service.AuthAPI.Service.IService;
using KinFlix.Service.AuthAPI.Utilities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
SD.UserName = builder.Configuration["InfoBipCredentials:UserName"];
SD.Password = builder.Configuration["InfoBipCredentials:Password"];

//Configure Context accessor for the cookie
builder.Services.AddHttpContextAccessor();

//DataBase Configuration
builder.Services.AddDbContext<AppDbContext>(option =>
option.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);

//Configure jwtOptions for the Token
builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("ApiSettings:jwOptions"));

builder.Services.AddIdentity<ApplicationUser, IdentityRole>().AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//Scoped Services
builder.Services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
builder.Services.AddScoped<IAuthService,AuthService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
