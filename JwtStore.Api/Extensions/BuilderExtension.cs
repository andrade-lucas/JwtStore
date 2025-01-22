using JwtStore.Core;
using JwtStore.Infra.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace JwtStore.Api.Extensions;

public static class BuilderExtension
{
    public static void AddConfiguration(this WebApplicationBuilder builder)
    {
        Configuration.Database.ConnectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? string.Empty;

        Configuration.Secrets.ApiKey = builder.Configuration.GetValue<string>("") ?? string.Empty;
        Configuration.Secrets.JwtPrivateKey = builder.Configuration.GetValue<string>("JwtPrivateKey") ?? string.Empty;
        Configuration.Secrets.PasswordSaltKey = builder.Configuration.GetValue<string>("PasswordSaltKey") ?? string.Empty;
        Configuration.SendGrid.ApiKey = builder.Configuration.GetValue<string>("SendGridApiKey") ?? string.Empty;
        Configuration.Email.DefaultFromEmail = builder.Configuration.GetValue<string>("Email:DefaultFromEmail") ?? string.Empty;
        Configuration.Email.DefaultFromName = builder.Configuration.GetValue<string>("Email:DefaultFromName") ?? string.Empty;
    }

    public static void AddDatabase(this WebApplicationBuilder builder)
    {
        builder.Services.AddDbContext<AppDbContext>(x => 
            x.UseSqlServer(Configuration.Database.ConnectionString, 
            b => b.MigrationsAssembly("JwtStore.Api")));
    }

    public static void AddJwtAuthentication(this WebApplicationBuilder builder)
    {
        builder.Services
            .AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration.Secrets.JwtPrivateKey)),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });
    }
}
