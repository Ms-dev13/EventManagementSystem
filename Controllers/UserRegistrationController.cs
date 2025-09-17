using EventManagementSystem.Data;
using EventManagementSystem.DTOs;
using EventManagementSystem.Model;
using EventManagementSystem.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace EventManagementSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserRegistrationController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly OtpService _otpService;
        private readonly JwtService _jwtService;
        public UserRegistrationController(AppDbContext context, OtpService optService, JwtService jwtService)
        {
            _context = context;
            _otpService = optService;
            _jwtService = jwtService;
        }

        [HttpPost("signup")]
        public async Task<IActionResult> SignUp([FromBody]SignUpRequestDTO requestDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Console.WriteLine(requestDTO.Name, requestDTO.Email);

          /*  bool isOtpValid = _otpService.ValidateOtp(requestDTO.Email, requestDTO.Otp);
            if (!isOtpValid)
            {
                return BadRequest(new { message = "Invalid or expired OTP." });
            }*/
            // Checking if the Email Already Exists?
          /*  var existuser = await _context.Users.
                FirstOrDefaultAsync(u => u.Email == requestDTO.Email);
            if(existuser != null)
            {
                return Conflict(new { message = "Email Already Exist" });
            } */
            // Checking for OTP Validation if Role is Attendee
            if(requestDTO.Role.ToLower() == "attendee")
            {       
              
                if (!_otpService.ValidateOtp(requestDTO.Email, requestDTO.Otp))
                {
                    return BadRequest(new { message = "Invalid or Expired OTP" });
                }
                else {
                   Console.WriteLine("OTP Validated Successfully");
                }
            }


            // Check if Role Exists, if not create it
            var role = await _context.Roles
                .FirstOrDefaultAsync(r => r.Name == requestDTO.Role);

            if(role == null)
            {
                role = new Role { Name = requestDTO.Role };
                _context.Roles.Add(role);
                await _context.SaveChangesAsync();
            }

                //Map DTO to Entity
                var user = new User
            {
                Name = requestDTO.Name,
                Email = requestDTO.Email,
                Password = BCrypt.Net.BCrypt.HashPassword(requestDTO.Password)
            }; 
                

            user.UserRoles.Add(new UserRole { RoleId = role.Id });

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var userWithRoles = _context.Users
            .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
            .FirstOrDefault(u => u.Id == user.Id);

            var Roles = userWithRoles.UserRoles.Select(ur => ur.Role.Name).ToList();

            var token = _jwtService.GenerateJwtToken(user, Roles);
            var name = user.Name;

            return Ok(new {token,name});

        }

    }
}
