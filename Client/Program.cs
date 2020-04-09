using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using BlazorMovies.Client.Helpers;
using Blazor.FileReader;
using BlazorMovies.Client.Repository;
using Microsoft.AspNetCore.Components.Authorization;
using BlazorMovies.Client.Auth;

namespace BlazorMovies.Client
{
    public static class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("app");

            builder.Services.AddBaseAddressHttpClient();
            ConfigureServices(builder.Services);

            await builder.Build().RunAsync();
        }
        private static void ConfigureServices(IServiceCollection services)
        {
            services.AddOptions();

            services.AddTransient<IRepository, RepositoryInMemory>();
            services.AddScoped<IHttpService, HttpService>();
            services.AddScoped<IGenreRepository, GenreRepository>();
            services.AddScoped<IPersonRepository, PersonRepository>();
            services.AddScoped<IMoviesRepository, MoviesRepository>();
            services.AddScoped<IAccountsRepository, AccountsRepository>();
            services.AddFileReaderService(options => options.InitializeOnFirstCall = true);
            services.AddAuthorizationCore();

            //Need to use single state of JWT Auth State, so that each service is pulling from the same one.            
            services.AddScoped<JWTAuthenticationStateProvider>();
            services.AddScoped<AuthenticationStateProvider, JWTAuthenticationStateProvider>(
                provider => provider.GetRequiredService<JWTAuthenticationStateProvider>()
                );//pulls from line already registered provider
            services.AddScoped<ILoginService, JWTAuthenticationStateProvider>(
                provider => provider.GetRequiredService<JWTAuthenticationStateProvider>()
                );//pulls from line already registered provider
        }
    }
}
