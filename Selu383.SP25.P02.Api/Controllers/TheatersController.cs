using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Selu383.SP25.P02.Api.Data;
using Selu383.SP25.P02.Api.Features.Theaters;
using Selu383.SP25.P02.Api.Dtos;
using Microsoft.AspNetCore.Authorization;

namespace Selu383.SP25.P02.Api.Controllers
{
    [Route("api/theaters")]
    [ApiController]
    //[Authorize] // Require authentication for all actions
    public class TheatersController : ControllerBase
    {
        private readonly DbSet<Theater> theaters;
        private readonly DataContext dataContext;

        public TheatersController(DataContext dataContext)
        {
            this.dataContext = dataContext;
            theaters = dataContext.Set<Theater>();
        }

        // Open to all authenticated users (both "User" & "Admin")
        [HttpGet]
        public IQueryable<TheaterDto> GetAllTheaters()
        {
            return GetTheaterDtos(theaters);
        }

        // Open to all authenticated users (both "User" & "Admin")
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

        // Only "Admin" can create a theater
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<TheaterDto>> CreateTheaterAsync(TheaterDto dto)
        {
            if (IsInvalid(dto))
            {
                return BadRequest();
            }

            if (!User.IsInRole("Admin"))
            {
                return Forbid(); // Returns a 403 Forbidden
            }


            var manager = await dataContext.Users.FindAsync(dto.ManagerId.GetValueOrDefault());
            if (manager == null)
            {
                return NotFound();
            }

            var theater = new Theater
            {
                Name = dto.Name,
                Address = dto.Address,
                SeatCount = dto.SeatCount,
                Manager = manager
                
            };
            theaters.Add(theater);

            await dataContext.SaveChangesAsync();

            dto.Id = theater.Id;

            return CreatedAtAction(nameof(GetTheaterById), new { id = dto.Id }, dto);
        }

        // Only "Admin" can update a theater
        [HttpPut]
        [Route("{id}")]
        [Authorize(Roles = "Admin")]
        public ActionResult<TheaterDto> UpdateTheater(int id, TheaterDto dto)
        {
            if (IsInvalid(dto))
            {
                return BadRequest();
            }

            if (!User.IsInRole("Admin"))
            {
                return Forbid(); // Returns a 403 Forbidden
            }

            var theater = theaters.FirstOrDefault(x => x.Id == id);
            if (theater == null)
            {
                return NotFound();
            }

            theater.Name = dto.Name;
            theater.Address = dto.Address;
            theater.SeatCount = dto.SeatCount;
            dataContext.SaveChanges();

            dto.Id = theater.Id;

            return Ok(dto);
        }

        // Only "Admin" can delete a theater
        [HttpDelete]
        [Route("{id}")]
        [Authorize(Roles = "Admin")]
        public ActionResult DeleteTheater(int id)
        {

            if (!User.IsInRole("Admin"))
            {
                return Forbid(); // Returns a 403 Forbidden
            }

            var theater = theaters.FirstOrDefault(x => x.Id == id);
            if (theater == null)
            {
                return NotFound();
            }

            theaters.Remove(theater);

            dataContext.SaveChanges();

            return Ok();
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
