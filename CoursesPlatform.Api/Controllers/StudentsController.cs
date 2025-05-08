using Microsoft.AspNetCore.Mvc;
using CoursesPlatform.Api.Models;
using CoursesPlatform.Api.Data;
using Microsoft.EntityFrameworkCore;
using MyCourseApp.Shared.Dtos;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace CoursesPlatform.Api.Controllers
{
  
    [ApiController]
    [Route("api/students")]
    public class StudentsController : ControllerBase
    {
        private readonly CoursesPlatformDbContext _context;

        public StudentsController(CoursesPlatformDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Student>>> GetStudents()
        {
            return await _context.Students.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Student>> GetStudent(int id)
        {
            var student = await _context.Students.FindAsync(id);

            if (student == null)
                return NotFound();

            return student;
        }

        [HttpPost]
        public async Task<ActionResult<Student>> CreateStudent(Student student)
        {
            _context.Students.Add(student);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetStudent), new { id = student.Id }, student);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateStudent(int id, Student student)
        {
            if (id != student.Id)
                return BadRequest();

            _context.Entry(student).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Students.Any(e => e.Id == id))
                    return NotFound();
                else
                    throw;
            }

            return NoContent();
        }


       
        [HttpGet("me")]
        public async Task<IActionResult> GetStudentInfo()
        {
            //var email = User.Identity?.Name;
            var email = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;


            if (string.IsNullOrEmpty(email))
                return Unauthorized();

            var student = await _context.Students
                .Where(s => s.Email == email)
                .Select(s => new StudentDto
                {
                    Name = s.Name,
                    Email = s.Email
                })
                .FirstOrDefaultAsync();

            if (student == null)
                return NotFound();

            return Ok(student);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStudent(int id)
        {
            var student = await _context.Students.FindAsync(id);
            if (student == null)
                return NotFound();

            _context.Students.Remove(student);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
