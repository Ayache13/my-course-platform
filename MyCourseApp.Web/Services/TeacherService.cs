using MyCourseApp.Web.Models;
using System.Net.Http.Json;

namespace MyCourseApp.Web.Services
{
    public class TeacherService
    {
        private readonly HttpClient _http;

        public TeacherService(HttpClient http)
        {
            _http = http;
        }

        public async Task<List<Teacher>> GetAllTeachersAsync()
        {
            return await _http.GetFromJsonAsync<List<Teacher>>("api/teachers") ?? new List<Teacher>();
        }

    }
}
