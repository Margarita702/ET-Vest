using Microsoft.AspNetCore.Identity;

namespace ET_Vest.Models
{
    public class ApplicationUser:IdentityUser
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Name { get; set; }
    }
}
