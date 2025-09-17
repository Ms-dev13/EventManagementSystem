using System.ComponentModel.DataAnnotations;

namespace EventManagementSystem.DTOs
{
    public class SignUpRequestDTO
    {
        [Required(ErrorMessage = "Full Name is Requied")]
        public string Name { get; set; }

        [Required (ErrorMessage = "Email is Required")]
        [EmailAddress (ErrorMessage = "Enter Valid Email Address")]
        public string Email { get; set; }
        [Required (ErrorMessage = "Password is Required")]
        [MinLength(6,ErrorMessage = "Password Must be Atleast 6 Characters")]
        public string Password { get; set; }

        public string? Otp { get; set; }
        public string Role { get; set; } = "Attendee";

    }
}
