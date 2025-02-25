using System.ComponentModel.DataAnnotations;

namespace Selu383.SP25.P02.Api.Dtos
{
    public class LoginDto
    {
        [Required]
        public string UserName { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
