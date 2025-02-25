using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Selu383.SP25.P02.Api.Data;
using Selu383.SP25.P02.Api.Features.Theaters;
using Selu383.SP25.P02.Api.Dtos;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace Selu383.SP25.P02.Api.Controllers
{
    [Route("api/theaters")]
    [ApiController]
    public class TheatersController : ControllerBase
    {
        private readonly DbSet<Theater> theaters;
        private readonly DataContext dataContext;

        public TheatersController(DataContext dataContext)
        {
            this.dataContext = dataContext;
            theaters = dataContext.Set<Theater>();
        }

        [HttpGet]
        public IQueryable<TheaterDto> GetAllTheaters()
        {
            return GetTheaterDtos(theaters);
        }

        [HttpGet]
        [Route("{id}")]
        public ActionResult<TheaterDto> GetTheaterById(int id)
        {
            var result = GetTheaterDtos(theaters.Where(x => x.Id == id)).FirstOrDefault();
            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<TheaterDto>> CreateTheaterAsync(TheaterDto dto)
        {
            if (IsInvalid(dto))
            {
                return BadRequest(new { message = "Invalid theater data." });
            }

            var manager = dto.ManagerId.HasValue
                ? await dataContext.Users.FindAsync(dto.ManagerId.Value)
                : null;

            var theater = new Theater
            {
                Name = dto.Name,
                Address = dto.Address,
                SeatCount = dto.SeatCount,
                Manager = manager
            };

            theaters.Add(theater);
            await dataContext.SaveChangesAsync();

            var createdTheater = new TheaterDto
            {
                Id = theater.Id,
                Name = theater.Name,
                Address = theater.Address,
                SeatCount = theater.SeatCount,
                ManagerId = dto.ManagerId
            };

            return CreatedAtAction(nameof(GetTheaterById), new { id = theater.Id }, createdTheater);
        }

        [HttpPut]
        [Route("{id}")]
        [Authorize]
        public async Task<ActionResult<TheaterDto>> UpdateTheater(int id, TheaterDto dto)
        {
            if (IsInvalid(dto))
            {
                return BadRequest();
            }

            var theater = await theaters.Include(t => t.Manager).FirstOrDefaultAsync(x => x.Id == id);
            if (theater == null)
            {
                return NotFound();
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!User.IsInRole("Admin") && (theater.Manager == null || theater.Manager.Id.ToString() != userId))
            {
                return Forbid();
            }

            theater.Name = dto.Name;
            theater.Address = dto.Address;
            theater.SeatCount = dto.SeatCount;

            if (dto.ManagerId.HasValue)
            {
                var manager = await dataContext.Users.FindAsync(dto.ManagerId.Value);
                if (manager == null)
                {
                    return BadRequest(new { message = "Invalid ManagerId." });
                }
                theater.Manager = manager;
            }
            else
            {
                theater.Manager = null;
            }

            await dataContext.SaveChangesAsync();

            var updatedDto = new TheaterDto
            {
                Id = theater.Id,
                Name = theater.Name,
                Address = theater.Address,
                SeatCount = theater.SeatCount,
                ManagerId = theater.ManagerId
            };

            return Ok(updatedDto);
        }

        [HttpDelete]
        [Route("{id}")]
        [Authorize(Roles = "Admin")]
        public ActionResult DeleteTheater(int id)
        {
            if (!User.IsInRole("Admin"))
            {
                return Forbid();
            }

            var theater = theaters.FirstOrDefault(x => x.Id == id);
            if (theater == null)
            {
                return NotFound(new { message = "No such item exists." });
            }

            theaters.Remove(theater);
            dataContext.SaveChanges();

            return Ok(new { message = "Theater successfully deleted.", id = id });
        }

        private static bool IsInvalid(TheaterDto dto)
        {
            return string.IsNullOrWhiteSpace(dto.Name) ||
                   dto.Name.Length > 120 ||
                   string.IsNullOrWhiteSpace(dto.Address) ||
                   dto.SeatCount <= 0;
        }

        private static IQueryable<TheaterDto> GetTheaterDtos(IQueryable<Theater> theaters)
        {
            return theaters
                .Select(x => new TheaterDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    Address = x.Address,
                    SeatCount = x.SeatCount,
                    ManagerId = x.ManagerId,
                });
        }
    }
}