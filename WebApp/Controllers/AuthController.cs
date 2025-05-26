using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WebApp.DTO;

namespace WebApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(UserManager<IdentityUser> userManager) : ControllerBase
    {

        [HttpGet]
        public async Task<IActionResult> Auth(string username, string password)
        {
            var user = await userManager.FindByEmailAsync(username);

            //var roles = await userManager.GetRolesAsync(user);

            if (await userManager.CheckPasswordAsync(user, password))
            {
                return Ok(await GenerateJwtToken(userManager, user, "123456789", "123456789", "12345678901234567890123456781234567890123456789012345678", 10));
            }
            return BadRequest();

        }


        [HttpGet("users")]
        [Authorize(AuthenticationSchemes = "Bearer", Roles = "admin")]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetUsers()
        {
            var users = await userManager.Users.ToListAsync();

            var userDtos = new List<UserDto>();

            foreach (var user in users)
            {
                userDtos.Add(new UserDto
                {
                    Id = user.Id,
                    Email = user.Email!,
                    Roles = await userManager.GetRolesAsync(user)
                });
            }

            return Ok(userDtos);
        }

        private async Task<string> GenerateJwtToken(UserManager<IdentityUser> userManager, IdentityUser user, string Issuer, string Audience, string Key, int LiveTimeMinutes)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Email!),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email!),
                new Claim(ClaimTypes.NameIdentifier, user.Id)
            };

            try
            {
                foreach (string role in await userManager.GetRolesAsync(user))
                {
                    claims.Add(new Claim(ClaimTypes.Role, role));
                }
            }
            catch { }

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(Convert.ToDouble(LiveTimeMinutes)),
                Issuer = Issuer,
                Audience = Audience,
                SigningCredentials = credentials
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}

    
