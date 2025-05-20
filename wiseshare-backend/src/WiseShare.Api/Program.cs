using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using DotNetEnv;

using Wiseshare.Api;
using Wiseshare.Application;
using Wiseshare.Infrastructure;
using Wiseshare.Infrastructure.Authentication;
using Wiseshare.Application.Authentication;
using Wiseshare.Application.services.UserServices;

Env.Load();  // load all the KEY=VALUE pairs from .env into Environment

var builder = WebApplication.CreateBuilder(args);

//ensure environment variables override JSON
builder.Configuration
       .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
       .AddEnvironmentVariables();

//  CONFIGURE & READ JwtSettings 
builder.Services.Configure<JwtSettings>(
    builder.Configuration.GetSection(JwtSettings.SectionName));

var jwtSettings = builder.Configuration
    .GetSection(JwtSettings.SectionName)
    .Get<JwtSettings>()!;

//) DATA PROTECTION 
builder.Services
    .AddDataProtection()
    .SetApplicationName("WiseShare");

// AUTHENTICATION & AUTHORIZATION 
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer           = true,
            ValidateAudience         = true,
            ValidateLifetime         = true,
            ValidateIssuerSigningKey = true,

            ValidIssuer    = jwtSettings.Issuer,
            ValidAudience  = jwtSettings.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtSettings.Secret))
        };

        options.Events = new JwtBearerEvents
        {
            OnChallenge = context =>
            {
                context.HandleResponse();
                context.Response.StatusCode  = 401;
                context.Response.ContentType = "application/json";
                var payload = JsonSerializer.Serialize(new {
                  error   = "unauthorized",
                  message = "Authentication token is missing or invalid."
                });
                return context.Response.WriteAsync(payload);
            },
            OnForbidden = context =>
            {
                context.Response.StatusCode  = 403;
                context.Response.ContentType = "application/json";
                var payload = JsonSerializer.Serialize(new {
                  error   = "forbidden",
                  message = "You do not have permission to perform this action."
                });
                return context.Response.WriteAsync(payload);
            }
        };
    });

builder.Services.AddAuthorization();

//  MVC, SWAGGER, CORS & LAYERS 
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", cors =>
        cors.WithOrigins("http://localhost:3000", "https://wisesharedashboard.aliarthur.com")
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials()
    );
});

builder.Services
       .AddPresentation()
       .AddApplication()
       .AddInfrastructure(builder.Configuration);

var app = builder.Build();

// Seed default Admin if missing 
using (var scope = app.Services.CreateScope())
{
    var userService = scope.ServiceProvider.GetRequiredService<IUserService>();
    var authService = scope.ServiceProvider.GetRequiredService<IAuthenticationService>();

    const string adminSecQ = "Who developed the backend";
    const string adminSecA = "Ali";

    var existing = userService
        .GetUserByEmailAsync("wiseshare897@gmail.com")
        .GetAwaiter().GetResult();

    if (existing.IsFailed)
    {
        var reg = authService
            .RegisterAsync("Admin", "wiseshare","wiseshare897@gmail.com", "999999999",
                           "Password12345$", adminSecQ, adminSecA)
            .GetAwaiter().GetResult();

        if (reg.IsSuccess)
        {
            var adminUser = userService
                .GetUserByEmailAsync("wiseshare897@gmail.com")
                .GetAwaiter().GetResult().Value;

            adminUser.Update(
                email:             null,
                phone:             null,
                password:          null,
                role:              "Admin",
                securityQuestion:  adminUser.SecurityQuestion,
                securityAnswer:    adminUser.SecurityAnswer
            );

            userService.UpdateAsync(adminUser).GetAwaiter().GetResult();
            userService.SaveAsync().GetAwaiter().GetResult();
        }
    }
}

//  5) MIDDLEWARE 
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowReactApp");

app.UseAuthentication();
app.UseAuthorization();

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(Directory.GetCurrentDirectory(), "images")),
    RequestPath = "/images"
});

app.MapControllers();
app.Run();
