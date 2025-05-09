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
    public class TeachersController : ControllerBase
    {
        private readonly CoursesPlatformDbContext _context;

        public TeachersController(CoursesPlatformDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Teacher>>> GetTeachers()
        {
            try
            {
                var teachers = await _context.Teachers
                    .Include(t => t.Courses)
                    .ToListAsync();

                return teachers;
            }
            catch (Exception ex)
            {
               
                return StatusCode(500, $"errorn : {ex.Message}");
            }
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<Teacher>> GetTeacher(int id)
        {
            var teacher = await _context.Teachers.FindAsync(id);

            if (teacher == null)
                return NotFound();

            return teacher;
        }

        [HttpPost]
        public async Task<ActionResult<Teacher>> CreateTeacher(Teacher teacher)
        {
            _context.Teachers.Add(teacher);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetTeacher), new { id = teacher.Id }, teacher);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTeacher(int id, Teacher teacher)
        {
            if (id != teacher.Id)
                return BadRequest();

            _context.Entry(teacher).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Teachers.Any(e => e.Id == id))
                    return NotFound();
                else
                    throw;
            }

            return NoContent();
        }

        [HttpGet("me")]
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> GetTeacherInfo()
        {
            var email = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;

            if (string.IsNullOrEmpty(email))
                return Unauthorized();

            var teacher = await _context.Teachers
                .Where(t => t.Email == email)
                .Select(t => new TeacherDto
                {
                    FullName = t.FullName,
                    Email = t.Email
                    // يمكنك لاحقًا إضافة Biography أو Specialization هنا
                })
                .FirstOrDefaultAsync();

            if (teacher == null)
                return NotFound();

            return Ok(teacher);
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTeacher(int id)
        {
            var teacher = await _context.Teachers.FindAsync(id);
            if (teacher == null)
                return NotFound();

            _context.Teachers.Remove(teacher);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
