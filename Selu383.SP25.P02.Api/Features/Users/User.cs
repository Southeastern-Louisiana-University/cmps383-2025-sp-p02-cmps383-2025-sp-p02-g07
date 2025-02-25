using Microsoft.AspNetCore.Identity;
using Selu383.SP25.P02.Api.Features.Roles;
using Selu383.SP25.P02.Api.Features.UserRoles;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Selu383.SP25.P02.Api.Features.Users
{
    public class User : IdentityUser<int>
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] 
        public override int Id { get; set; }

        public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    }
}
