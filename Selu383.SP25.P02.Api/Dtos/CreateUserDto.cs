namespace Selu383.SP25.P02.Api.Dtos
{
    public class CreateUserDto
    {
        
        public required string UserName { get; set; } 
        public required string[] Roles { get; set; }
        public required string Password { get; set; }
    }
}
