using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using todo.Server.Data;
using Microsoft.AspNetCore.Authorization;
using todo.Shared;

namespace BlazorApp.Controllers;

[ApiController]
public class UsersController : Controller
{
    private readonly TodoContext _db;

    public UsersController(TodoContext db)
    {
        _db = db;
    }
    private string CreateJWT(User user)
    {
        var secretkey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes("THIS IS THE SECRET KEY"));
        var credentials = new SigningCredentials(secretkey, SecurityAlgorithms.HmacSha256);

        var claims = new[] {
                new Claim(ClaimTypes.Name, user.Email),
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, user.Email)
            };

        var token = new JwtSecurityToken(
            issuer: "domain.com",
            audience: "domain.com",
            claims: claims,
            expires: DateTime.Now.AddMinutes(60),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    [HttpGet]
    [Authorize]
    [Route("api/user")]
    public string Get()
    {
        return User.Identity?.Name ?? string.Empty;
        // return User.Identity.Name ;
    }

    [HttpPost]
    [Route("api/auth/register")]
    public async Task<LoginResult> Post([FromBody] RegModel reg)
    {
        if (reg.password != reg.confirmpwd)
            return new LoginResult { message = "Password and confirm password do not match.", success = false };

        var existingUser = await _db.Users.FindAsync(reg.email);
        if (existingUser != null)
            return new LoginResult { message = "User already exists.", success = false };

        var newuser = new User { Email = reg.email, Password = reg.password };
        await _db.Users.AddAsync(newuser);
        await _db.SaveChangesAsync();

        return new LoginResult { message = "Registration successful.", jwtBearer = CreateJWT(newuser), email = reg.email, success = true };
    }

    [HttpPost]
    [Route("/api/auth/login")]
    public async Task<LoginResult> Post([FromBody] LoginModel log)
    {
        var user = await _db.Users.FindAsync(log.email);

        if (user?.Password == log.password)
            return new LoginResult { message = "Login successful.", jwtBearer = CreateJWT(user), email = log.email, success = true };

        return new LoginResult { message = "User/password not found.", success = false };
    }
}