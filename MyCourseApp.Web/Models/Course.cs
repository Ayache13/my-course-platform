using System.ComponentModel.DataAnnotations;

namespace MyCourseApp.Web.Models
{
    public class Course
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        // Foreign Key
        public int TeacherId { get; set; }

        // Navigation Property
        public Teacher? Teacher { get; set; }

        public double Rate { get; set; }

        public List<StudentCourse> StudentCourses { get; set; } = new();
    }
}
