using EventManagementSystem.Data;
using EventManagementSystem.DTOs;
using EventManagementSystem.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace EventManagementSystem.Controllers
{
    [ApiController]
    [Route("api/[Controller]")]
    [Authorize(Roles = "Admin")]
    public class ManageUsers : Controller
    {
        private readonly AppDbContext _context;
        public ManageUsers(AppDbContext context)
        {
            _context = context;

        }
        [HttpGet("AllUsers")]
        public async Task< ActionResult<List<UserDTO>>> GetAllUsers()
        {
           
           var users = await _context.Users.Select(u => new UserDTO
            {
                Id = u.Id,
                Name = u.Name,
                Email = u.Email,
                Role = u.UserRoles.FirstOrDefault().Role.Name // Assuming a user has at least one role
            }).ToListAsync();

            if(users == null || users.Count == 0)
            {
                return NotFound("No users found.");
            } 
            return Ok(users);

        }
        [HttpDelete("DeleteUser/{id}")] 
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.Users
            .Include(u => u.Registrations) // Load related registrations
            .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
                return NotFound("User not found");

            // Manual cleanup of related UserRegistrations
            _context.UserRegistrations.RemoveRange(user.Registrations);

            // Now remove the user
            _context.Users.Remove(user);

            await _context.SaveChangesAsync();

            return Ok("User deleted successfully");
        }
        [HttpPost("BlockUser/{id}")]
        public async Task<IActionResult> BlockUser(int id)
        {

            var user = await _context.Users.FindAsync(id);

            if (user == null) return NotFound("User Not Found ");

            user.isBlocked = true; 
            await _context.SaveChangesAsync(); 
            return Ok($"User {user.Name} Has Been Blocked.");

        }

        [HttpPost("UnblockUser/{id}")]
        public async Task<IActionResult> UnblockUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound("User Not Found ");
            user.isBlocked = false;
            await _context.SaveChangesAsync();
            return Ok($"User {user.Name} Has Been Unblocked.");
        }

        [HttpPost("assign-organizer/{userId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AssignOrganizerRole(int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null) return NotFound("User not found");

            var organizerRole = await _context.Roles.FirstOrDefaultAsync(r => r.Name == "Organizer");
            if (organizerRole == null) return NotFound("Organizer role not found");

            var alreadyAssigned = await _context.UserRoles
                .AnyAsync(ur => ur.UserId == userId && ur.RoleId == organizerRole.Id);

            if (alreadyAssigned)
                return BadRequest("User already has Organizer role.");

            _context.UserRoles.Add(new UserRole { UserId = userId, RoleId = organizerRole.Id });
            await _context.SaveChangesAsync();

            return Ok($"User {user.Name} is now an Organizer.");
        }



    }
}
