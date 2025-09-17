namespace EventManagementSystem.Model
{
    public class User
    { 
        public int Id { get; set;}
        public string Name { get; set; }
        public string Email { get; set; } 
        public string Password { get; set; } 
        public bool isBlocked { get; set; } = false;

        // Navigation Properties
        public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
        public ICollection<UserRegistration> Registrations { get; set;} = new List<UserRegistration>();
        public ICollection<Event> OrganizedEvents { get; set; } = new List<Event>();

    }
}
