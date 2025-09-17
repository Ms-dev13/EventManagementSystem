namespace EventManagementSystem.Model
{
    public class UserRegistration
    { 
        public int UserId { get; set; }
        public int EventId { get; set; } 
        public DateTime RegistrationDate { get; set; } = DateTime.UtcNow;
        
        // Navigation Properties
        public User User { get; set; } = null!;
        public Event Event { get; set; } = null!;
    }
}
