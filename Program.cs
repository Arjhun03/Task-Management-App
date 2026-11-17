using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using Task_Management_App.Repository.RepositoryInterface;
using TaskManagement.Api.Data;
using TaskManagement.Api.Models;
using TaskManagement.API.Repositories;
using TaskManagement.API.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();

// Database configuration
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")
    ));

// Repository registration
builder.Services.AddScoped<IAuthRepository, AuthRepository>();

// Service registration
builder.Services.AddScoped<IAuthService, AuthService>();

// Password hasher registration
builder.Services.AddScoped<IPasswordHasher<Users>, PasswordHasher<Users>>();

// OpenAPI registration
builder.Services.AddOpenApi();

// Build the application
var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
