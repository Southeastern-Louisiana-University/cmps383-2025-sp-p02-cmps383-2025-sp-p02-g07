using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Linq;
using System.Threading.Tasks;
using Selu383.SP25.P02.Api.Features.Users;
using Selu383.SP25.P02.Api.Dtos;
using Selu383.SP25.P02.Api.Features.Roles;

namespace Selu383.SP25.P02.Api.Controllers
{
    [Route("/api/users")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;

        public UsersController(
            UserManager<User> userManager,
            RoleManager<Role> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<UserDto>> CreateUser(CreateUserDto dto)
        {
            // Validate username uniqueness
            var existingUser = await _userManager.FindByNameAsync(dto.UserName);
            if (existingUser != null)
            {
                return BadRequest(new { error = "Username already exists" });
            }

            // Validate roles
            if (dto.Roles == null || !dto.Roles.Any())
            {
                return BadRequest(new { error = "At least one role must be provided" });
            }

            // Verify all roles exist
            foreach (var role in dto.Roles)
            {
                if (!await _roleManager.RoleExistsAsync(role))
                {
                    return BadRequest(new { error = $"Role '{role}' does not exist" });
                }
            }

            // Remove duplicate roles
            dto.Roles = dto.Roles.Distinct().ToArray();

            // Create user
            var user = new User
            {
                UserName = dto.UserName,
            };

            var result = await _userManager.CreateAsync(user, dto.Password);
            if (!result.Succeeded)
            {
                return BadRequest(new { errors = result.Errors });
            }

            // Assign roles
            result = await _userManager.AddToRolesAsync(user, dto.Roles);
            if (!result.Succeeded)
            {
                // Cleanup: delete the user if role assignment fails
                await _userManager.DeleteAsync(user);
                return BadRequest(new { errors = result.Errors });
            }

            var userDto = new UserDto
            {
                Id = user.Id,
                UserName = user.UserName!,
                Roles = (await _userManager.GetRolesAsync(user)).ToArray()
            };

            return Ok(userDto);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UserDto>> GetUserById(int id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null)
            {
                return NotFound();
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