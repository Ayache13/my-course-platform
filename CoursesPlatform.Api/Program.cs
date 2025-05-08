using System;
using System.Security.Claims;
using System.Text;
using System.Text.Json.Serialization;
using CoursesPlatform.Api.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace CoursesPlatform.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container
            builder.Services.AddDbContext<CoursesPlatformDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddControllers().AddJsonOptions(opts =>
            {
                opts.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
            });
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // Move CORS setup here, before app is built
                builder.Services.AddCors(options =>
                {
                    options.AddPolicy("AllowBlazor",
                        policy => policy.WithOrigins("http://localhost:5015")
                                        .AllowAnyHeader()
                                        .AllowAnyMethod());
                });

            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = builder.Configuration["Jwt:Issuer"],
                    ValidAudience = builder.Configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])),

                    RoleClaimType = "http://schemas.microsoft.com/ws/2008/06/identity/claims/role",
                    NameClaimType = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress"
                };
            });



            // Build the app AFTER registering services
            var app = builder.Build();

            // Use HTTPS Redirection
            app.UseHttpsRedirection();

            // Use CORS before authentication and authorization
            app.UseCors("AllowBlazor");

            // Authentication and Authorization
            app.UseAuthentication();
            app.UseAuthorization();

            // Map Controllers to the route
            app.MapControllers();

            // Swagger setup for development
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            // Custom middleware to log Authorization header
            app.Use(async (context, next) =>
            {
                var authHeader = context.Request.Headers["Authorization"].ToString();
                Console.WriteLine($"Authorization Header: {authHeader}");
                Console.WriteLine($"Is Authenticated: {context.User?.Identity?.IsAuthenticated}");
                await next();
            });

            // Run the application
            app.Run();
        }
    }
}
