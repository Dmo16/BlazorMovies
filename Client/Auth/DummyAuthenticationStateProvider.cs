using Microsoft.AspNetCore.Components.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BlazorMovies.Client.Auth
{
    public class DummyAuthenticationStateProvider : AuthenticationStateProvider
    {
        public async override Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var anonymous = new ClaimsIdentity( new List<Claim>() {
                new Claim("testkey1", "value1"),
                new Claim(ClaimTypes.Name, "Test User"),
                new Claim(ClaimTypes.Role, "Admin")
            }, "test");
            return await Task.FromResult(new AuthenticationState(new ClaimsPrincipal(anonymous)));

             
               

        }
    }
}
