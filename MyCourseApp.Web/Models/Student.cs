namespace MyCourseApp.Web.Models
{
    public class Student
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public DateTime? DateOfBirth { get; set; }

        // relation Many-to-Many with Course cross StudentCourse
        public List<StudentCourse> StudentCourses { get; set; } = new();
    }
}
