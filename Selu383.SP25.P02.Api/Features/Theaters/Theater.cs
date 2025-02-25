using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Selu383.SP25.P02.Api.Features.Theaters
{
    public class Theater
    {
        public int Id { get; set; }
        [MaxLength(120)]
        public required string Name { get; set; }
        public required string Address { get; set; }
        public int SeatCount { get; set; }
        public int? ManagerId { get; set; }

        [ForeignKey(nameof(ManagerId))]     
        public Users.User? Manager { get; set; }
    }
}
