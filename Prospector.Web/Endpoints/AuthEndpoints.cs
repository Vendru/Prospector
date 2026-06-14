using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Prospector.Web.Client.Models;
using Prospector.Web.Models;

namespace Prospector.Web.Endpoints;

public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/auth").WithTags("Auth");

        group.MapGet("/me", GetMe);
        group.MapPost("/login", Login);
        group.MapPost("/register", Register);
        group.MapPost("/logout", Logout);

        // GET logout for server-side redirect (used by MainLayout)
        app.MapGet("/account/logout-redirect", async (SignInManager<ApplicationUser> signIn) =>
        {
            await signIn.SignOutAsync();
            return Results.Redirect("/account/login");
        }).WithTags("Auth");
    }

    private static IResult GetMe(ClaimsPrincipal user)
    {
        if (user.Identity?.IsAuthenticated != true)
            return Results.Ok(new UserInfoDto(false, null, null, null));

        var email = user.FindFirstValue(ClaimTypes.Email) ?? user.Identity.Name ?? "";
        var name = user.FindFirstValue("DisplayName") ?? email;
        var roles = user.FindAll(ClaimTypes.Role).Select(c => c.Value).ToArray();
        return Results.Ok(new UserInfoDto(true, email, name, roles));
    }

    private static async Task<IResult> Login(
        LoginRequest req,
        SignInManager<ApplicationUser> signInManager,
        UserManager<ApplicationUser> userManager)
    {
        var user = await userManager.FindByEmailAsync(req.Email);
        if (user is null) return Results.Unauthorized();

        var result = await signInManager.PasswordSignInAsync(user, req.Password, req.RememberMe, lockoutOnFailure: false);
        if (!result.Succeeded) return Results.Unauthorized();

        return Results.Ok(new { message = "Login bem-sucedido" });
    }

    private static async Task<IResult> Register(
        RegisterRequest req,
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager)
    {
        var user = new ApplicationUser
        {
            UserName = req.Email,
            Email = req.Email,
            DisplayName = req.DisplayName,
            EmailConfirmed = true
        };

        var result = await userManager.CreateAsync(user, req.Password);
        if (!result.Succeeded)
            return Results.BadRequest(result.Errors.Select(e => e.Description));

        await signInManager.SignInAsync(user, isPersistent: false);
        return Results.Ok(new { message = "Conta criada com sucesso!" });
    }

    private static async Task<IResult> Logout(SignInManager<ApplicationUser> signInManager)
    {
        await signInManager.SignOutAsync();
        return Results.Ok(new { message = "Logout realizado." });
    }
}
