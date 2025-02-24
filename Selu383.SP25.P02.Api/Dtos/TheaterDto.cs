using System.ComponentModel.DataAnnotations;

namespace Selu383.SP25.P02.Api.Dtos
{
    public class TheaterDto
    {
        public int Id { get; set; }
        [Required]
        [MaxLength(120)]
        public string Name { get; set; }
        [Required]
        public string Address { get; set; }
        public int SeatCount { get; set; }
        public int? ManagerId { get; set; }
    }

    public class CreateTheaterDto
    {
        [Required]
        [MaxLength(120, ErrorMessage = "Name cannot be longer than 120 characters.")]
        public string Name { get; set; }

        [Required]
        public string Address { get; set; }
        public int SeatCount { get; set; }
        public int? ManagerId { get; set; } 
    }

    public class UpdateTheaterDto
    {
        [Required]
        [MaxLength(120, ErrorMessage = "Name cannot be longer than 120 characters.")]
        public string Name { get; set; }

        [Required]
        public string Address { get; set; }
        public int SeatCount { get; set; }
        public int? ManagerId { get; set; } 
    }
}
