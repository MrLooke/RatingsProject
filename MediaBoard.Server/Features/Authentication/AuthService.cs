using MediaBoard.Server.Exceptions;
using MediaBoard.Server.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.IdentityModel.Tokens.Jwt;

namespace MediaBoard.Server.Features.Authentication
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _dbContext;
        private readonly IPasswordHasher<AppUser> _passwordHasher;
        private readonly IConfiguration _configuration;
        public AuthService(AppDbContext dbContext, IPasswordHasher<AppUser> passwordHasher, IConfiguration config)
        {
            _dbContext = dbContext;
            _passwordHasher = passwordHasher;
            _configuration = config;
        }

        public async Task<RegisterResult> RegisterUserAsync(RegisterRequest request)
        {
            bool exists = await _dbContext.AppUsers.AnyAsync(u => u.Email == request.Email || u.Username == request.Username);
            if(exists)
            {
                throw new ConflictException("A user with that email or username already exists.");
            }

            var user = new AppUser
            {
                Username = request.Username,
                Email = request.Email
            };

            user.PasswordHash = _passwordHasher.HashPassword(user, request.Password);

            _dbContext.Add(user);
            await _dbContext.SaveChangesAsync();

            var userResult = new RegisterResult { 
                UserId = user.UserId,
                Username = user.Username,
                Email = user.Email
            };

            return userResult;
        }

        public async Task<LoginResult> LoginUserAsync(LoginRequest request)
        {
            AppUser? user = await _dbContext.AppUsers.FirstOrDefaultAsync(u => u.Email == request.Email);

            if(user is null)
            {
                throw new UnauthorizedException("Invalid email or password");
            }

            var passwordResult = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);

            if(passwordResult == PasswordVerificationResult.Failed)
            {
                throw new UnauthorizedException("Invalid email or password");
            }

            if(passwordResult == PasswordVerificationResult.SuccessRehashNeeded)
            {
                user.PasswordHash = _passwordHasher.HashPassword(user, request.Password);
                await _dbContext.SaveChangesAsync();
            }

            var loginResult = new LoginResult
            {
                UserId = user.UserId,
                Username = user.Username,
                Email = user.Email
            };

            return loginResult;
        }

        public string GenerateToken(LoginResult user)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Email, user.Email)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(double.Parse(_configuration["Jwt:ExpiryMinutes"]!)),
                signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
