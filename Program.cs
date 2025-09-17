using EventManagementSystem.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using EventManagementSystem.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();

//Adding InMeory Cache Support
builder.Services.AddMemoryCache();

// Otp Service
 builder.Services.AddSingleton<OtpService>();

// Email Service
 builder.Services.AddTransient<EmailService>();

// JWT Token Generation Service
 builder.Services.AddTransient<JwtService>();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Add services for DataBase Context

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configuring CORS policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("MyCorsPolicy", policy =>
    {
        policy
          .AllowAnyOrigin()
          .AllowAnyMethod()
          .AllowAnyHeader();
    });
});

// Creating JWT Authentication
var jwtSetttings = builder.Configuration.GetSection("JWT");
    // Creating Key
  

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSetttings["Issuer"],
        ValidAudience = jwtSetttings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes("Dholakpur123456789isMySuperSecreteKeyOfLengthLongerthanBefore"))
    };

});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

Console.WriteLine("Your Hash Password is: "+BCrypt.Net.BCrypt.HashPassword("Admin@123"));


app.UseHttpsRedirection();

app.UseCors("MyCorsPolicy"); 

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
