using EventManagementSystem.Data;
using EventManagementSystem.DTOs;
using EventManagementSystem.Model;
using EventManagementSystem.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens; // Use for Token Validation or Algorithm using System.IdentityModel.Tokens.Jwt; // Use for JWT Token
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims; // Use for Key Generation
using System.Text; // Use for Encoding

namespace EventManagementSystem.Controllers
{   
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly EmailService _emailService;
        private readonly OtpService _otpService;
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthController(EmailService emailService, OtpService otpService, AppDbContext context, IConfiguration configuration)
        {
            _emailService = emailService;
            _otpService = otpService;
            _context = context;
            _configuration = configuration;
        } 

        [HttpPost("send-otp")]
        public async Task<IActionResult> SendOtp([FromBody] OtpEmail email)
        {
            var existUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == email.Email);
            if (existUser != null)
                return Conflict(new { message = "Email already exists" });

            var otp = _otpService.GenerateOtp(email.Email);
            await _emailService.SendEmailAsync(email.Email, "Your OTP Code For EvenZaa Registration", $"Your OTP is {otp}");
            return Ok(new { message = "OTP sent successfully." });
        }

        /*  [HttpPost("verify-otp")]
          public IActionResult VerifyOtp([FromBody] OtpRequest request)
          {
              bool isValid = _otpService.ValidateOtp(request.Email, request.Otp);
              return isValid ? Ok(new { message = "OTP Verified!" }) : BadRequest("Invalid or expired OTP.");
          }
        */
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {   
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _context.Users.
                Include(u => u.UserRoles).
                ThenInclude(ur => ur.Role).
                FirstOrDefaultAsync(u => u.Email == request.Email);

            if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.Password))
            {
                return Unauthorized(new { message = "Invalid email or password" });
            }

            var Roles = user.UserRoles.Select(u=>u.Role.Name).ToList();

            var token = GenerateJwtToken(user, Roles);
            var name = user.Name;
                
            return Ok(new { token,name});
        }
        private string GenerateJwtToken(User user, List<string> roles)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim (ClaimTypes.Email, user.Email)
            };

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            } 

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("Dholakpur123456789isMySuperSecreteKeyOfLengthLongerthanBefore"));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);


            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:Issuer"],
                audience: _configuration["JWT:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(1),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);

        }

    } 
   

    public class OtpRequest
    {
        public string? Email { get; set; }
        public string? Otp { get; set; }
    }

}
