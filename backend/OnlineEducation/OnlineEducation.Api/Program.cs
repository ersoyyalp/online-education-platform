using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using OnlineEducation.Application.Common;
using OnlineEducation.Infrastructure;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Controllers
builder.Services.AddControllers();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// AppSettings / ConnectionStrings
builder.Services.Configure<DatabaseSettings>(
    builder.Configuration.GetSection("ConnectionStrings"));

// Infrastructure (Dapper, DbConnectionFactory, Commands, Queries)
builder.Services.AddInfrastructure();

// 🔐 JWT AUTH
var jwtSection = builder.Configuration.GetSection("Jwt");
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            ValidIssuer = jwtSection["Issuer"],
            ValidAudience = jwtSection["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtSection["Key"]!)
            ),

            // 🔥 EN KRİTİK SATIR
            RoleClaimType = ClaimTypes.Role
        };
    });

builder.Services.AddAuthorization();

// 🔴 CORS – Angular için
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular",
        policy =>
        {
            policy
                .WithOrigins("http://localhost:4200")
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// 🔴 CORS (Authentication’dan ÖNCE olur)
app.UseCors("AllowAngular");

app.UseHttpsRedirection();

// 🔐 AUTH SIRASI ÇOK ÖNEMLİ
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
