﻿namespace Selu383.SP25.P02.Api.Dtos
{
    public class UserDto
    {
        public int Id { get; set; }
        public required string UserName { get; set; }
        public required string[] Roles { get; set; }
    }
}
