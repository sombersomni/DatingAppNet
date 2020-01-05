using Microsoft.AspNetCore.Mvc;
using DatingApp.API.Data;
using DatingApp.API.Models;
using DatingApp.API.DTOs;
using System.Threading.Tasks;
namespace DatingApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _repository;
        public AuthController(IAuthRepository repository) {
            _repository = repository;
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
        
    }
}