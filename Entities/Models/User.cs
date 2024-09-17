using Microsoft.AspNetCore.Identity;

namespace Entities.Models;

public class User : IdentityUser<Guid>
{
    public string Name { get; set; }
}