namespace EventManagementSystem.Model
{
    public class Event
    { 
        public int Id { get; set; }
        public string EventName { get; set; } 
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string? Location { get; set; }

        // Organizer Information 
        public int OrganizerId { get; set; }
        public User Organizer { get; set; }

        // Navigation Properties 
        public ICollection<UserRegistration> Registrations { get; set; } = new List<UserRegistration>();
        
    }
}
