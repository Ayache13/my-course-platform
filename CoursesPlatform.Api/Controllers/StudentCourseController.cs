using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CoursesPlatform.Api.Data;
using CoursesPlatform.Api.Models;

namespace CoursesPlatform.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentCourseController : ControllerBase
    {
        private readonly CoursesPlatformDbContext _context;

        public StudentCourseController(CoursesPlatformDbContext context)
        {
            _context = context;
        }

        // 1. تسجيل الطالب في كورس
        [HttpPost("enroll")]
        public async Task<IActionResult> EnrollStudent(int studentId, int courseId)
        {
            var studentExists = await _context.Students.AnyAsync(s => s.Id == studentId);
            var courseExists = await _context.Courses.AnyAsync(c => c.Id == courseId);

            if (!studentExists || !courseExists)
            {
                return NotFound("Student or Course not found.");
            }

            var enrollment = new StudentCourse
            {
                StudentId = studentId,
                CourseId = courseId
            };

            _context.StudentCourses.Add(enrollment);
            await _context.SaveChangesAsync();

            return Ok("Student enrolled successfully!");
        }

        // 2. عرض الكورسات الخاصة بالطالب
        [HttpGet("student/{studentId}/courses")]
        public async Task<IActionResult> GetStudentCourses(int studentId)
        {
            var studentCourses = await _context.StudentCourses
                .Where(sc => sc.StudentId == studentId)
                .Include(sc => sc.Course)
                .Select(sc => new
                {
                    sc.CourseId,
                    sc.Course.Title,
                    sc.Course.Description,
                    sc.Course.StartDate,
                    sc.Course.EndDate
                })
                .ToListAsync();

            if (studentCourses.Count == 0)
            {
                return NotFound("No courses found for this student.");
            }

            return Ok(studentCourses);
        }

        // 3. view students courses

        [HttpGet("course/{courseId}/students")]
        public async Task<IActionResult> GetCourseStudents(int courseId)
        {
            var courseStudents = await _context.StudentCourses
                .Where(sc => sc.CourseId == courseId)
                .Include(sc => sc.Student)
                .Select(sc => new
                {
                    sc.StudentId,
                    sc.Student.Name,
                    sc.Student.Email,
                    sc.Student.DateOfBirth
                })
                .ToListAsync();

            if (courseStudents.Count == 0)
            {
                return NotFound("No students found for this course.");
            }

            return Ok(courseStudents);
        }
    }
}
