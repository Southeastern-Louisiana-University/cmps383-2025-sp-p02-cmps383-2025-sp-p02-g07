using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Selu383.SP25.P02.Api.Data;
using Selu383.SP25.P02.Api.Dtos;
using Microsoft.AspNetCore.Authorization;
using System.Linq;
using System.Threading.Tasks;
using Selu383.SP25.P02.Api.Features.Users;

namespace Selu383.SP25.P02.Api.Controllers
{
    [Route("/api/users")]
    [ApiController]
    public class UsersController : ControllerBase //wrap in class; very important
    {
        private readonly DataContext _context;

        public UsersController(DataContext context) //necesarry constructor
        {
            _context = context;
        }

        // Only "Admin" can create a user
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<UserDto>> CreateUser(UserDto dto)
        {
            var validRoles = new[] { "Admin", "User" };

            if (!validRoles.Contains(dto.Roles))
            {
                return BadRequest(new { message = "Invalid Role" }); //returns 400
            }

            if (!await IsUsernameUnique(dto.UserName))
            {
                return BadRequest(new { message = "Username Taken!" }); //returns 400
            }

            var user = new User
            {
                UserName = dto.UserName,
                Roles = dto.Roles
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            dto.Id = user.Id;

            return CreatedAtAction(nameof(GetUserById), new { id = dto.Id }, dto);
        }

        private async Task<bool> IsUsernameUnique(string username)
        {
            return !await _context.Users.AnyAsync(u => u.UserName == username);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UserDto>> GetUserById(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound(); //returns 404
            }
            return new UserDto
            {
                Id = user.Id,
                UserName = user.UserName,
                Roles = user.Roles
            };
        }
    }
}