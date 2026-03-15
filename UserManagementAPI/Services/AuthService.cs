using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using UserManagementAPI.Data;
using UserManagementAPI.DTOs;
using UserManagementAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace UserManagementAPI.Services;

public class AuthService : IAuthService
{
    private readonly ApplicationDbContext _context;
    private readonly IConfiguration _config;

    private readonly TimeSpan _accessTokenLife = TimeSpan.FromMinutes(30);
    private readonly TimeSpan _refreshTokenLife = TimeSpan.FromHours(1);

    public AuthService(ApplicationDbContext context, IConfiguration config)
    {
        _context = context;
        _config = config;
    }

    private string GenerateJwtToken(User user, TimeSpan expiry)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_config["Jwt:Key"]!);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role.ToString()),
                new Claim("Id", user.Id.ToString())
            }),
            Expires = DateTime.UtcNow.Add(expiry),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    public async Task<User?> RegisterAsync(RequestRegisterDto request)
    {
        var existingUser = await _context.Users.AnyAsync(u => u.Email == request.Email);
        if (existingUser) return null;

        var user = new User
        {
            Email = request.Email,
            Role = UserRole.User,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password)
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return user;
    }

    public async Task<ResponseAuthDto?> LoginAsync(RequestLoginDto request)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Email == request.Email);

        if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            return null;

        var session = new SessionLog
        {
            UserId = user.Id,
            StartDate = DateTime.UtcNow
        };
        _context.SessionLogs.Add(session);
        await _context.SaveChangesAsync();

        return new ResponseAuthDto
        {
            Token = GenerateJwtToken(user, _accessTokenLife),
            RefreshToken = GenerateJwtToken(user, _refreshTokenLife),
            Email = user.Email,
            Role = user.Role.ToString()
        };
    }

    public async Task<ResponseAuthDto?> RefreshTokenAsync(string refreshToken)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_config["Jwt:Key"]!);

        try
        {
            var principal = tokenHandler.ValidateToken(refreshToken, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false,
                ClockSkew = TimeSpan.Zero
            }, out SecurityToken validatedToken);

            var email = principal.FindFirstValue(ClaimTypes.Email);
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);

            if (user == null) return null;

            return new ResponseAuthDto
            {
                Token = GenerateJwtToken(user, _accessTokenLife),
                RefreshToken = refreshToken,
                Email = user.Email,
                Role = user.Role.ToString()
            };
        }
        catch
        {
            return null;
        }
    }

    public async Task LogoutAsync(int userId)
    {
        var lastSession = await _context.SessionLogs
            .Where(s => s.UserId == userId && s.EndDate == null)
            .OrderByDescending(s => s.StartDate)
            .FirstOrDefaultAsync();

        if (lastSession != null)
        {
            lastSession.EndDate = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> UpdatePasswordAsync(UpdatePasswordRequest request, bool isAdmin)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
        if (user == null) return false;

        if (!isAdmin && !BCrypt.Net.BCrypt.Verify(request.CurrentPassword, user.PasswordHash))
        {
            return false;
        }

        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
        _context.Users.Update(user);
        await _context.SaveChangesAsync();

        return true;
    }
}
