using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using DatingApp.API.Data;
using DatingApp.API.Dtos;
using DatingApp.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace DatingApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _repo;
        private readonly IConfiguration _config;
        public AuthController(IAuthRepository repo, IConfiguration config)
        {
            _config = config;
            _repo = repo;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(UserforRegisterDto userforRegisterDto)
        {
            // validate request
            // if (!ModelState.IsValid)
            //     return BadRequest(ModelState);

            userforRegisterDto.Username = userforRegisterDto.Username.ToLower();

            if (await _repo.UserExists(userforRegisterDto.Username))
                return BadRequest("User name already exist");

            var userToCreate = new User
            {
                Username = userforRegisterDto.Username
            };

            var createdUSer = await _repo.Register(userToCreate, userforRegisterDto.Password);

            return StatusCode(201);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(UserforLoginDto userforLoginDto)
        {
            var userFromrepo = await _repo.Login(userforLoginDto.Username.ToLower(), userforLoginDto.Password);

            if (userFromrepo == null)
                return Unauthorized();

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userFromrepo.Id.ToString()),
                new Claim(ClaimTypes.Name, userFromrepo.Username)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8
                .GetBytes(_config.GetSection("AppSettings:Token").Value));

            var creds = new SigningCredentials(key,SecurityAlgorithms.HmacSha512Signature);
            
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = creds

            };

            var tokenHandler = new JwtSecurityTokenHandler();
            
            var token = tokenHandler.CreateToken(tokenDescriptor);
            
            return Ok(new {
                token = tokenHandler.WriteToken(token)
            });
        }

    }
}