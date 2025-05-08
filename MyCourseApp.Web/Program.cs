using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MyCourseApp.Web.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Blazored.LocalStorage;
using MyCourseApp.Web.Services.Auth;
using Microsoft.Extensions.DependencyInjection;





namespace MyCourseApp.Web
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");
            builder.RootComponents.Add<HeadOutlet>("head::after");

            // ????? Blazored LocalStorage
            builder.Services.AddBlazoredLocalStorage();

            // ????? CustomAuthenticationStateProvider
            builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthenticationStateProvider>();
            builder.Services.AddAuthorizationCore();

            // ????? AuthMessageHandler
            builder.Services.AddTransient<AuthMessageHandler>();

            // ????? HttpClient ?? AuthMessageHandler (??????? ???? ????? ????)
            builder.Services.AddHttpClient("AuthorizedClient", client =>
            {
                client.BaseAddress = new Uri("http://localhost:5026");
            }).AddHttpMessageHandler<AuthMessageHandler>();

            // ????? HttpClient ???? ???? (?? login/register)
            builder.Services.AddHttpClient("PublicClient", client =>
            {
                client.BaseAddress = new Uri("http://localhost:5026");
            });

            
            builder.Services.AddScoped(sp =>
            {
                var factory = sp.GetRequiredService<IHttpClientFactory>();
                return factory.CreateClient("AuthorizedClient");
            });



            builder.Services.AddAuthorizationCore();
            builder.Services.AddScoped<CustomAuthenticationStateProvider>();
            builder.Services.AddScoped<AuthenticationStateProvider>(provider => provider.GetRequiredService<CustomAuthenticationStateProvider>());

            // ????? ???????
            builder.Services.AddScoped<AuthService>();
            builder.Services.AddScoped<StudentService>();
            builder.Services.AddScoped<CourseService>();
            builder.Services.AddScoped<TeacherService>();

            await builder.Build().RunAsync();
        }
    }
}
