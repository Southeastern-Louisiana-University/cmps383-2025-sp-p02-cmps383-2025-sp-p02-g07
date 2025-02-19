namespace Selu383.SP25.P02.Api.Features.Logins
{
    public class LoginDto
    {
        public int Id { get; set; }
        public required string UserName { get; set; }
        public required string Password { get; set; }
    } 
}
