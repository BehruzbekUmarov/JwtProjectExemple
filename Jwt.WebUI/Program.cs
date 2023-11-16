using Jwt.Application.Interfaces;
using Jwt.Application.Options;
using Jwt.Domain.Data;
using Jwt.Infrastucture.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

var connection = builder.Configuration.GetConnectionString("DbContext");

builder.Services.AddAuthentication(d =>
{
    d.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    d.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    d.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters()
        {
            ValidateAudience = true,
            ValidateIssuer = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            ValidAudience = builder.Configuration["JWT:Audience"],
            ValidIssuer = builder.Configuration["JWT:Issuer"],
            IssuerSigningKey = new SymmetricSecurityKey
            (Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
        };
    });

builder.Services.AddAuthorization(op =>
{
    op.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin").RequireRole("name"));
});

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlite(connection);
});

builder.Services.AddScoped<ITokenManager, TokenManager>()
    .AddScoped<IPasswordManager, PasswordManager>()
    .AddScoped<IUserManager, UserManager>();

builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("JWT"));

builder.Services.AddRouting(b =>
{
    b.LowercaseUrls = true;
});

builder.Services.AddControllers();


var app = builder.Build();

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
