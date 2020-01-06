using System;
using System.Threading.Tasks;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using DatingApp.API.Data;
using DatingApp.API.Models;
using DatingApp.API.DTOs;
namespace DatingApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _repository;
        private readonly IConfiguration _config;
        public AuthController(IAuthRepository repository, IConfiguration config) {
            _repository = repository;
            _config = config;
        }

        [HttpPost("register")]
        // ApiController attribute will automatically infer we need FromBody for this dto
        public async Task<IActionResult> Register(UserForRegisterDto dto) {
            // validate request

            string username = dto.UserName.ToLower();

            if (await _repository.UserExists(username)) {
                return BadRequest("Username is already taken");
            }

            var newUser = new User {
                UserName = username
            };

            var createdUser = await _repository.Register(newUser, dto.Password);
            return StatusCode(201);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(UserForLoginDto dto) {
            // login for user
            var userFromRepo = await _repository.Login(dto.UserName.ToLower(), dto.Password);
            if (userFromRepo == null) {
                return Unauthorized();
            }
            // starting to build our token
            // This holds the info of our user
            // we hold the userId and the userName
            // these claims must both be strings
            var claims = new[] {
                new Claim(ClaimTypes.NameIdentifier, userFromRepo.UserId.ToString()),
                new Claim(ClaimTypes.Name, userFromRepo.UserName)
            };

            // we need to make sure the token is valid
            // so we are creating the security key for later validation
            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_config.GetSection("AppSettings:Token").Value)
            );

            // we encrypt the security key with Sha512 algorithm
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            // we give the token the user info (claims)
            // we give it an expiration of 1 day
            // we set the signing creditionals (the encrypted key)
            var tokenDescriptor = new SecurityTokenDescriptor {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = creds
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            // we create a token with an instance of a token handler
            var token = tokenHandler.CreateToken(tokenDescriptor);
            // we write the token to the response header
            return Ok(new { 
                token = tokenHandler.WriteToken(token)
            });
        }
        
    }
}