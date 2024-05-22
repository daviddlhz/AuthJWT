using Application.Contracts;
using Application.DTOs;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Infrastructure.Repo
{
    public class UserRepo : IUser
    {
        private readonly AppDbContext _AppDbContext;
        private readonly IConfiguration _configuration;

        public UserRepo(AppDbContext appDbContext, IConfiguration configuration) 
        {
            _AppDbContext = appDbContext;
            _configuration = configuration;
        }
        public async Task<LoginResponse> LoginUserAsync(LoginDTO loginDTO)
        {
            var getUser = await findUserByEmail(loginDTO.Email);
            if (getUser == null) return new LoginResponse(false, "User not found, sorry");

            bool checkPassword = BCrypt.Net.BCrypt.Verify(loginDTO.Password, getUser.Password);
            if (checkPassword)
            {
                return new LoginResponse(true, "Login Succesfully", GenerateJWTToken(getUser));
            }
            else
            {
                return new LoginResponse(false, "Invalid Credentials");
            }
        }

        public async Task<RegistrationResponse> RegisterUserAsync(RegisterUserDto registerUserDto)
        {
            var getUser = await findUserByEmail(registerUserDto.Email);
            if (getUser != null)
            {
                return new RegistrationResponse(false, "User already exists");
            }

            _AppDbContext.Users.Add(new ApplicationUser()
            {
                Name = registerUserDto.Name,
                Email = registerUserDto.Email,
                Password = BCrypt.Net.BCrypt.HashPassword(registerUserDto.Password)
            });
            await _AppDbContext.SaveChangesAsync();
            return new RegistrationResponse(true, "Registration completed");
        }

        private async Task<ApplicationUser> findUserByEmail(string email) => await _AppDbContext.Users.FirstOrDefaultAsync(user => user.Email == email);

        private string GenerateJWTToken(ApplicationUser user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var userClaims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Name!),
                new Claim(ClaimTypes.Email, user.Email!)
            };
            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: userClaims,
                expires: DateTime.Now.AddDays(5),
                signingCredentials: credentials
                );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
