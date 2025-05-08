using System.ComponentModel.DataAnnotations;

namespace MyCourseApp.Web.Models
{
    public class Teacher
    {
        public int Id { get; set; }

        [Required]
        public string FullName { get; set; } = string.Empty;

        public string? Email { get; set; }

        // Navigation Property
        public ICollection<Course> Courses { get; set; } = new List<Course>();
    }
}
