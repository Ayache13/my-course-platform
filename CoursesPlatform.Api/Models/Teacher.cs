using System.ComponentModel.DataAnnotations;

namespace CoursesPlatform.Api.Models
{
    public class Teacher
    {
        public int Id { get; set; }

        [Required]
        public string FullName { get; set; } = string.Empty;

        public string? Email { get; set; }

        public int UserId { get; set; } // Foreign Key

        // Navigation Property
        public User User { get; set; } = null!;


        // Navigation Property
        public ICollection<Course> Courses { get; set; } = new List<Course>();
    }
}
