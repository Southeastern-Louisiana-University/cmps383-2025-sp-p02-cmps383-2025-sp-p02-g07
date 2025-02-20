using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Selu383.SP25.P02.Api.Data;
using Selu383.SP25.P02.Api.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Linq;
using System.Threading.Tasks;
using Selu383.SP25.P02.Api.Features.Users;

namespace Selu383.SP25.P02.Api.Controllers
{
    [Route("/api/users")]
    [ApiController]
    public class UsersController : ControllerBase //wrap in class; very important
    {
        private readonly UserManager<User> _userManager;

        public UsersController(UserManager<User> userManager) //necesarry constructor
        {
            _userManager = userManager;
        }

        // Only "Admin" can create a user
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<CreateUserDto>> CreateUser(CreateUserDto dto)
        {

            var existingUser = await _userManager.FindByNameAsync(dto.UserName);

            var user = new User
            {
                UserName = dto.UserName,
            };


            await _userManager.CreateAsync(user,dto.Password);
            var newUser = new UserDto {
                Id = user.Id,
                UserName = user.UserName!,
                Roles = (await _userManager.GetRolesAsync(user)).ToArray()
            };

            await _userManager.AddToRolesAsync(user, dto.Roles);
            return CreatedAtAction(nameof(GetUserById), new { id = user.Id }, newUser);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UserDto>> GetUserById(int id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null)
            {
                return NotFound(); //returns 404
            }
            return new UserDto
            {
                Id = user.Id,
                UserName = user.UserName!,
                Roles = (await _userManager.GetRolesAsync(user)).ToArray()
            };
        }
    }
}