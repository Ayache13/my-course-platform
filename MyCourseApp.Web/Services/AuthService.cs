using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Json;
using System.Security.Claims;
using Blazored.LocalStorage;
using MyCourseApp.Shared.Models;

public class AuthService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _config;

    private readonly ILocalStorageService _localStorage;

    public AuthService(HttpClient httpClient, IConfiguration config, ILocalStorageService localStorage)
    {
        _httpClient = httpClient;
        _config = config;
        _localStorage = localStorage;
    }


    public async Task<LoginResponse> LoginAsync(LoginRequest request)
    {
        var response = await _httpClient.PostAsJsonAsync("api/auth/login", request);

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception("فشل في تسجيل الدخول");
        }

        var loginResponse = await response.Content.ReadFromJsonAsync<LoginResponse>();

        if (loginResponse != null && !string.IsNullOrEmpty(loginResponse.Token))
        {
            await _localStorage.SetItemAsync("jwtToken", loginResponse.Token);
        }

        return loginResponse!;
    }


    public async Task RegisterTeacherAsync(TeacherRegisterRequest request)
    {
        var response = await _httpClient.PostAsJsonAsync("api/auth/register/teacher", request);
        //response.EnsureSuccessStatusCode();
        if (!response.IsSuccessStatusCode)
        {
            var errorMessage = await response.Content.ReadAsStringAsync();
            throw new Exception($"Error from API: {errorMessage}");
        }
    }


    public async Task RegisterStudentAsync(StudentRegisterRequest request)
    {
        var response = await _httpClient.PostAsJsonAsync("api/auth/register/student", request);
        response.EnsureSuccessStatusCode();
    }




    public async Task RegisterAsync(RegisterRequest registerRequest)
    {
        var response = await _httpClient.PostAsJsonAsync("https://your-api-url/api/auth/register", registerRequest);

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception("خطأ في التسجيل");
        }
    }

    public string? GetRoleFromToken(string token)
    {
        var handler = new JwtSecurityTokenHandler();
        var jwt = handler.ReadJwtToken(token);
        var role = jwt.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
        return role;
    }

    public string? GetEmailFromToken(string token)
    {
        var handler = new JwtSecurityTokenHandler();
        var jwt = handler.ReadJwtToken(token);
        return jwt.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
    }
}



public class RegisterRequest
{
    public string FullName { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
}

public class LoginResponse
{
    public string Token { get; set; }
}
