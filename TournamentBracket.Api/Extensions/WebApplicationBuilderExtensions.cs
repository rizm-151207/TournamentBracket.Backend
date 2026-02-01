using System.Reflection;
using System.Text;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using TournamentBracket.Application.Users;
using TournamentBracket.Domain.Users;
using TournamentBracket.Infrastructure.Common;

namespace TournamentBracket.Api.Extensions;

public static class WebApplicationBuilderExtensions
{
    public static WebApplicationBuilder AddTunedCors(this WebApplicationBuilder builder)
    {
        builder.Services.AddCors(c => c.AddPolicy("cors", opt =>
        {
            opt.AllowAnyHeader();
            opt.AllowCredentials();
            opt.AllowAnyMethod();
            opt.AllowAnyOrigin();
        }));

        return builder;
    }

    public static WebApplicationBuilder AddSwaggerWithJwtAuth(this WebApplicationBuilder builder)
    {
        builder.Services.AddSwaggerGen(options =>
        {
            options.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme()
            {
                In = ParameterLocation.Header,
                Description = "Enter token",
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = JwtBearerDefaults.AuthenticationScheme,
                BearerFormat = "JWT"
            });
            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = JwtBearerDefaults.AuthenticationScheme
                        }
                    },
                    []
                }
            });
        });

        return builder;
    }

    public static WebApplicationBuilder AddIdentityUser(this WebApplicationBuilder builder)
    {
        builder.Services.AddIdentity<User, IdentityRole<Guid>>(options =>
            {
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireDigit = false;
            })
            .AddEntityFrameworkStores<AppDbContext>()
            .AddApiEndpoints()
            .AddDefaultTokenProviders();

        return builder;
    }

    public static WebApplicationBuilder AddJwtBearerAuth(this WebApplicationBuilder builder)
    {
        builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateActor = true,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    RequireExpirationTime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = builder.Configuration["Jwt:Issuer"],
                    ValidAudience = builder.Configuration["Jwt:Audience"],
                    ClockSkew = TimeSpan.Zero,
                    IssuerSigningKey =
                        new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(builder.Configuration.GetSection("Jwt:Secret").Value!))
                };

                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        if(context.Request.Cookies.TryGetValue(AuthConstants.AccessTokenName, out var token))
                            context.Token = token;
                        return Task.CompletedTask;
                    }
                };
            });

        builder.Services.AddAuthorization();

        return builder;
    }

    public static WebApplicationBuilder AddFluentValidation(this WebApplicationBuilder builder)
    {
        var assemblies = Assembly.GetCallingAssembly().GetReferencedAssemblies()
            .Select(Assembly.Load)
            .Where(a => a.FullName != null && a.FullName.StartsWith("TournamentBracket.Application"));
        builder.Services.AddValidatorsFromAssemblies(assemblies);
        return builder;
    }
}