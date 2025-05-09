using MyCourseApp.Web.Models;
using System.Net.Http;
using System.Net.Http.Json;
using MyCourseApp.Shared.Dtos;
namespace MyCourseApp.Web.Services
{
    public class CourseService
    {
        private readonly HttpClient _http;

        public CourseService(HttpClient http)
        {
            _http = http;
        }

        public async Task<List<Course>?> GetAllCoursesAsync()
        {
            return await _http.GetFromJsonAsync<List<Course>>("api/courses");
        }

        public async Task<CourseDto?> GetCourseByIdAsync(int id)
        {
            return await _http.GetFromJsonAsync<CourseDto>($"api/courses/{id}"); 
        }

        public async Task RegisterToCourseAsync(int courseId)
        {
            var response = await _http.PostAsync($"api/courses/register/{courseId}", null);
            response.EnsureSuccessStatusCode();
        }
        public async Task<List<Course>> GetMyCoursesAsync()
        {
            return await _http.GetFromJsonAsync<List<Course>>("api/courses/my") ?? new();
        }

        public async Task UnsubscribeFromCourseAsync(int courseId)
        {
            await _http.DeleteAsync($"api/courses/unsubscribe/{courseId}");
        }

        public async Task<List<CourseDto>> GetCoursesByCurrentTeacherAsync()
        {
            return await _http.GetFromJsonAsync<List<CourseDto>>("api/courses/my-teacher") ?? new();
        }

        public async Task DeleteCourseAsync(int id)
        {
            await _http.DeleteAsync($"api/courses/{id}");
        }


        public async Task AddCourseAsync(CourseDto course)
        {
            await _http.PostAsJsonAsync("api/courses", course);
        }

        public async Task UpdateCourseAsync(Course course)
        {
            await _http.PutAsJsonAsync($"api/courses/{course.Id}", course); 
        }

        
    }

}
