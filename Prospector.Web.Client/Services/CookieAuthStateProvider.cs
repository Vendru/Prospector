using System.Net.Http.Json;
using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;
using Prospector.Web.Client.Models;

namespace Prospector.Web.Client.Services;

public class CookieAuthStateProvider(HttpClient http) : AuthenticationStateProvider
{
    private static readonly AuthenticationState Unauthenticated =
        new(new ClaimsPrincipal(new ClaimsIdentity()));

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        try
        {
            var info = await http.GetFromJsonAsync<UserInfoDto>("/api/auth/me");
            if (info?.IsAuthenticated == true)
            {
                var claims = new List<Claim>
                {
                    new(ClaimTypes.Name, info.DisplayName ?? info.Email ?? ""),
                    new(ClaimTypes.Email, info.Email ?? ""),
                };
                if (info.Roles != null)
                    claims.AddRange(info.Roles.Select(r => new Claim(ClaimTypes.Role, r)));

                var identity = new ClaimsIdentity(claims, "Cookie");
                return new AuthenticationState(new ClaimsPrincipal(identity));
            }
        }
        catch { }

        return Unauthenticated;
    }

    public async Task<AuthResult> LoginAsync(LoginRequest request)
    {
        var response = await http.PostAsJsonAsync("/api/auth/login", request);
        if (response.IsSuccessStatusCode)
        {
            var state = await GetAuthenticationStateAsync();
            NotifyAuthenticationStateChanged(Task.FromResult(state));
            return new AuthResult(true, "Login realizado com sucesso!");
        }
        return new AuthResult(false, "Email ou senha incorretos.");
    }

    public async Task<AuthResult> RegisterAsync(RegisterRequest request)
    {
        var response = await http.PostAsJsonAsync("/api/auth/register", request);
        if (response.IsSuccessStatusCode)
        {
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
            return new AuthResult(true, "Conta criada com sucesso!");
        }
        var error = await response.Content.ReadAsStringAsync();
        return new AuthResult(false, "Falha ao criar conta. Tente outro email ou senha mais forte.");
    }

    public async Task LogoutAsync()
    {
        await http.PostAsync("/api/auth/logout", null);
        NotifyAuthenticationStateChanged(Task.FromResult(Unauthenticated));
    }
}
