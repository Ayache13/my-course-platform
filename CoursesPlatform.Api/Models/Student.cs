using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoursesPlatform.Api.Models
{
    public class Student
    {
        // Use UserId as the foreign key for the relationship
        public int Id { get; set; } // Primary Key

        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public DateTime? DateOfBirth { get; set; }

        // Foreign Key to User
        public int UserId { get; set; }

        // Navigation property to User
        public User User { get; set; } = null!;

        // Relationship with StudentCourse
        public List<StudentCourse> StudentCourses { get; set; } = new();
    }
}
