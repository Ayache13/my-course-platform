using MyCourseApp.Web.Models;
using System.Net.Http;
using System.Net.Http.Json;

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

        public async Task<Course?> GetCourseByIdAsync(int id)
        {
            return await _http.GetFromJsonAsync<Course>($"api/courses/{id}"); 
        }

        public async Task AddCourseAsync(Course course)
        {
            await _http.PostAsJsonAsync("api/courses", course);
        }

        public async Task UpdateCourseAsync(Course course)
        {
            await _http.PutAsJsonAsync($"api/courses/{course.Id}", course); 
        }

        public async Task DeleteCourseAsync(int id)
        {
            await _http.DeleteAsync($"api/courses/{id}"); 
        }
    }

}
