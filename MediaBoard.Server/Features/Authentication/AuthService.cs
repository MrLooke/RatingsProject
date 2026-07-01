using MediaBoard.Server.Exceptions;
using MediaBoard.Server.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using Microsoft.Extensions.Options;

namespace MediaBoard.Server.Features.Authentication
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _dbContext;
        private readonly IPasswordHasher<AppUser> _passwordHasher;
        private readonly IConfiguration _configuration;
        private readonly JwtSettings _jwtSettings;

        public AuthService(AppDbContext dbContext, IPasswordHasher<AppUser> passwordHasher, IConfiguration config, IOptions<JwtSettings> jwtSettings)
        {
            _dbContext = dbContext;
            _passwordHasher = passwordHasher;
            _configuration = config;
            _jwtSettings = jwtSettings.Value;
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
            }

            string accessToken = GenerateToken(user.UserId, user.Username, user.Email);
            string refreshToken = GenerateRefreshToken();
            RefreshToken tokenRecord = new RefreshToken
            {
                Token = refreshToken,
                UserId = user.UserId,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddDays(_jwtSettings.RefreshExpiryDays),
                IsRevoked = false
            };

            _dbContext.RefreshTokens.Add(tokenRecord);
            await _dbContext.SaveChangesAsync();

            var loginResult = new LoginResult
            {
                UserId = user.UserId,
                Username = user.Username,
                Email = user.Email,
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };

            return loginResult;
        }

        public async Task<RefreshResult> RefreshAsync(string refreshToken)
        {
            RefreshToken? token = await _dbContext.RefreshTokens
                .Include(r => r.User)
                .FirstOrDefaultAsync(r => r.Token == refreshToken);

            if(token == null || token.IsRevoked || DateTime.UtcNow > token.ExpiresAt)
            {
                throw new UnauthorizedException("Invalid or expired refresh token.");
            }

            var accessToken = GenerateToken(token.User.UserId, token.User.Username, token.User.Email);

            return new RefreshResult { AccessToken = accessToken };
        }

        public async Task LogoutAsync(string refreshToken)
        {
            RefreshToken? token = await _dbContext.RefreshTokens.FirstOrDefaultAsync(r => r.Token == refreshToken);

            if (token == null) return;

            token.IsRevoked = true;
            await _dbContext.SaveChangesAsync();
        }

        public string GenerateToken(int userId, string username, string email)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim(ClaimTypes.Name, username),
                new Claim(ClaimTypes.Email, email)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));

            var token = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_jwtSettings.AccessExpiryMinutes),
                signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string GenerateRefreshToken()
        {
            var bytes = new byte[64];
            RandomNumberGenerator.Fill(bytes);
            return Convert.ToBase64String(bytes);
        }
    }
}
