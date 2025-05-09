using Microsoft.AspNetCore.Mvc;
using CoursesPlatform.Api.Models;
using CoursesPlatform.Api.Data; 
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using MyCourseApp.Shared.Dtos;
using System.Security.Claims;

namespace CoursesPlatform.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CoursesController : ControllerBase
    {
        private readonly CoursesPlatformDbContext _context;

        public CoursesController(CoursesPlatformDbContext context)
        {
            _context = context;
        }

        // GET: api/courses
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Course>>> GetCourses()
        {
            return await _context.Courses.ToListAsync();
        }

        [Authorize(Roles = "Teacher")]
        [HttpPost]
        public async Task<IActionResult> AddCourse([FromBody] CourseDto courseDto)
        {
            var email = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;

            if (string.IsNullOrEmpty(email))
                return Unauthorized();

            var teacher = await _context.Teachers.FirstOrDefaultAsync(t => t.Email == email);
            if (teacher == null)
                return NotFound("Teacher not found");

            var course = new Course
            {
                Title = courseDto.Title,
                Description = courseDto.Description,
                StartDate = courseDto.StartDate,
                EndDate = courseDto.EndDate,
                Rate = courseDto.Rate,
                TeacherId = teacher.Id
            };

            _context.Courses.Add(course);
            await _context.SaveChangesAsync();

            return Ok();
        }



        /* [HttpGet("teacher")]
         public async Task<ActionResult<IEnumerable<Course>>> GetCoursesByTeacher()
         {

             var userId = GetUserIdFromToken(); 

             var teacher = await _context.Teachers
                 .FirstOrDefaultAsync(t => t.UserId == userId); 

             if (teacher == null)
             {
                 return BadRequest("المعلم غير موجود.");
             }


             var courses = await _context.Courses
                 .Where(c => c.TeacherId == teacher.Id) 
                 .ToListAsync();

             return Ok(courses);
         }


         // GET: api/courses/5
         [HttpGet("{id}")]
         public async Task<ActionResult<Course>> GetCourse(int id)
         {
             var course = await _context.Courses.FindAsync(id);

             if (course == null)
                 return NotFound();

             return course;
         }

         // POST: api/courses
         [HttpPost]
         public async Task<ActionResult<Course>> CreateCourse(Course course)
         {
             _context.Courses.Add(course);
             await _context.SaveChangesAsync();

             return CreatedAtAction(nameof(GetCourse), new { id = course.Id }, course);
         }*/

        [HttpGet("{id}")]
        public async Task<ActionResult<CourseDto>> GetCourseById(int id)
        {
            var course = await _context.Courses
                .Where(c => c.Id == id)
                .Select(c => new CourseDto
                {
                    Id = c.Id,
                    Title = c.Title,
                    Description = c.Description,
                    StartDate = c.StartDate,
                    EndDate = c.EndDate,
                    Rate = c.Rate
                })
                .FirstOrDefaultAsync();

            if (course == null)
                return NotFound();

            return Ok(course);
        }

        [Authorize(Roles = "Student")]
        [HttpPost("register/{courseId}")]
        public async Task<IActionResult> RegisterToCourse(int courseId)
        {
            var email = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(email))
                return Unauthorized();

            var student = await _context.Students.FirstOrDefaultAsync(s => s.Email == email);
            if (student == null)
                return NotFound("Student not found");

            var course = await _context.Courses.FindAsync(courseId);
            if (course == null)
                return NotFound("Course not found");

            var alreadyRegistered = await _context.StudentCourses
                .AnyAsync(sc => sc.StudentId == student.Id && sc.CourseId == courseId);
            if (alreadyRegistered)
                return BadRequest("Already registered");

            var registration = new StudentCourse
            {
                StudentId = student.Id,
                CourseId = courseId
            };

            _context.StudentCourses.Add(registration);
            await _context.SaveChangesAsync();

            return Ok("Registered successfully");
        }


        [HttpGet("my")]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> GetMyCourses()
        {
            var email = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(email))
                return Unauthorized();

            var student = await _context.Students
                .Include(s => s.StudentCourses)
                .ThenInclude(sc => sc.Course)
                .FirstOrDefaultAsync(s => s.Email == email);

            if (student == null)
                return NotFound("Student not found");

            var courses = student.StudentCourses.Select(sc => new CourseDto
            {
                Id = sc.Course.Id,
                Title = sc.Course.Title,
                Description = sc.Course.Description,
                StartDate = sc.Course.StartDate,
                EndDate = sc.Course.EndDate,
                Rate = sc.Course.Rate
            }).ToList();

            return Ok(courses);
        }

        [HttpGet("my-teacher")]
        [Authorize(Roles = "Teacher")]
        public async Task<ActionResult<List<CourseDto>>> GetMyTeacherCourses()
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            var teacher = await _context.Teachers.FirstOrDefaultAsync(t => t.Email == email);

            if (teacher == null)
                return NotFound("Teacher not found");

            var courses = await _context.Courses
                .Where(c => c.TeacherId == teacher.Id)
                .Select(c => new CourseDto
                {
                    Id = c.Id,
                    Title = c.Title,
                    Description = c.Description,
                    StartDate = c.StartDate,
                    EndDate = c.EndDate,
                    Rate = c.Rate
                })
                .ToListAsync();

            return courses;
        }



        [HttpDelete("unsubscribe/{courseId}")]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> UnsubscribeFromCourse(int courseId)
        {
            var email = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(email))
                return Unauthorized();

            var student = await _context.Students
                .Include(s => s.StudentCourses)
                .FirstOrDefaultAsync(s => s.Email == email);

            if (student == null)
                return NotFound("Student not found");

            var subscription = student.StudentCourses.FirstOrDefault(sc => sc.CourseId == courseId);
            if (subscription == null)
                return NotFound("Subscription not found");

            _context.StudentCourses.Remove(subscription);
            await _context.SaveChangesAsync();

            return NoContent();
        }



        // DELETE: api/courses/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> DeleteCourse(int id)
        {
            var course = await _context.Courses
                .Include(c => c.StudentCourses) // Include العلاقات المرتبطة
                .FirstOrDefaultAsync(c => c.Id == id);

            if (course == null)
                return NotFound();

            // حذف العلاقات المرتبطة أولاً
            _context.StudentCourses.RemoveRange(course.StudentCourses);

            _context.Courses.Remove(course);
            await _context.SaveChangesAsync();

            return NoContent();
        }



    }
}
