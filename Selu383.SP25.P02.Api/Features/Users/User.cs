using Microsoft.AspNetCore.Identity;
using Selu383.SP25.P02.Api.Features.Roles;
using System.Collections.Generic;
using Selu383.SP25.P02.Api.Features.UserRoles;


namespace Selu383.SP25.P02.Api.Features.Users
{
    public class User : IdentityUser<int>
    {
        public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    }
}
