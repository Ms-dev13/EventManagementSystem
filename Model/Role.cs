namespace EventManagementSystem.Model
{
    public class Role
    {  
        public int Id { get; set; }
        public string Name { get; set; } // e.g., "Admin", "Attendee", "Organizer"

        // Navigation Properties
        public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();

    }
}
