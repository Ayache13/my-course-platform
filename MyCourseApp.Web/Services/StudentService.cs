using MyCourseApp.Web.Models;
using System.Net.Http.Json;
using MyCourseApp.Shared.Dtos;
using Microsoft.JSInterop;

namespace MyCourseApp.Web.Services
{
    public class StudentService
    {
        private readonly HttpClient _http;
        private readonly IJSRuntime _jsRuntime;

        public StudentService(HttpClient http, IJSRuntime jsRuntime)
        {
            _http = http;
            _jsRuntime = jsRuntime;
        }

        public async Task<List<Student>> GetStudentsAsync()
        {
            return await _http.GetFromJsonAsync<List<Student>>("api/students") ?? new();
        }
        public async Task<StudentDto?> GetStudentAsync()
        {
            
            return await _http.GetFromJsonAsync<StudentDto>("api/students/me");
        }

    }


}
