using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using CoursesPlatform.Api.Models;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using CoursesPlatform.Api.Data;
using rq = MyCourseApp.Shared.Models;
using MyCourseApp.Shared.Models;



namespace CoursesPlatform.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly CoursesPlatformDbContext _context;

        public AuthController(CoursesPlatformDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] rq.LoginRequest request)
        {
            try
            {
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.Email == request.Email);

                if (user == null)
                {
                    return Unauthorized("Wrong login info");
                }

                var hasher = new PasswordHasher<User>();
                var result = hasher.VerifyHashedPassword(user, user.Password, request.Password);

                if (result != PasswordVerificationResult.Success)
                {
                    return Unauthorized("Wrong login info");
                }

                var token = GenerateJwtToken(user);
                return Ok(new { token });
            }
            catch (Exception ex)
            {
                

                return StatusCode(500, "Internal server error occurred. Please try again later.");
            }
        }



        //[HttpPost("register")]
        //public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        //{

        //    var existingUser = await _context.Users
        //        .FirstOrDefaultAsync(u => u.Email == request.Email);

        //    if (existingUser != null)
        //    {
        //        return BadRequest("User already exist");
        //    }


        //    var newUser = new User
        //    {
        //        FullName = request.FullName,
        //        Email = request.Email,
        //        Password = new PasswordHasher<User>().HashPassword(null, request.Password),
        //        Role = request.Role,  
        //        CreatedAt = DateTime.UtcNow
        //    };

        //    _context.Users.Add(newUser);
        //    await _context.SaveChangesAsync();

        //    return Ok("registring succes");
        //}

        [HttpPost("register/student")]
        public async Task<IActionResult> RegisterStudent([FromBody] StudentRegisterRequest request)
        {
            try
            {
                
                if (await _context.Users.AnyAsync(u => u.Email == request.Email))
                    return BadRequest("البريد الإلكتروني مستخدم بالفعل.");

                // 
                var hasher = new PasswordHasher<User>();
                var hashedPassword = hasher.HashPassword(null, request.Password);

                // 
                var user = new User
                {
                    Email = request.Email,
                    Password = hashedPassword,
                    Role = "Student",
                    CreatedAt = DateTime.UtcNow
                };

                
                _context.Users.Add(user);
                await _context.SaveChangesAsync();  

                
                var student = new Student
                {
                    Name = request.FullName,
                    Email = request.Email,
                    UserId = user.Id  
                };

                
                _context.Students.Add(student);
                await _context.SaveChangesAsync();  

                return Ok("تم تسجيل الطالب بنجاح.");
            }
            catch (DbUpdateException dbEx)
            {
                var message = dbEx.Message;
                return BadRequest($"خطأ في قاعدة البيانات: {dbEx.Message}");
            }
            catch (Exception ex)
            {
                var message = ex.Message;
                return BadRequest($"حدث خطأ غير متوقع أثناء التسجيل: {ex.Message}");
            }
        }


        [HttpPost("register/teacher")]
        public async Task<IActionResult> RegisterTeacher([FromBody] TeacherRegisterRequest request)
        {
            if (request.Password != request.ConfirmPassword)
                return BadRequest("كلمة المرور وتأكيدها غير متطابقتين.");

            if (await _context.Users.AnyAsync(u => u.Email == request.Email))
                return BadRequest("البريد الإلكتروني مستخدم بالفعل.");

            var hasher = new PasswordHasher<User>();
            var hashedPassword = hasher.HashPassword(null, request.Password);

            var user = new User
            {
                Email = request.Email,
                Password = hashedPassword,
                Role = "Teacher",
                FullName = request.FullName,
                CreatedAt = DateTime.UtcNow
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var teacher = new Teacher
            {
                FullName = request.FullName,
                Email = request.Email,
                UserId = user.Id
                // 
                // Specialization = request.Specialization
            };

            _context.Teachers.Add(teacher);
            await _context.SaveChangesAsync();

            return Ok("تم تسجيل المعلم بنجاح.");
        }












        private string GenerateJwtToken(User user)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, user.FullName),
                new Claim(ClaimTypes.Role, user.Role),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email) // ← هذا هو السطر الجديد
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }


    }

}
