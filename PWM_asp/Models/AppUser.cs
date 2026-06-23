using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace PWM_asp.Models
{
    public class AppUser : IdentityUser
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;

        public ICollection<SavedPWD>? SavedPWD { get; set; } = new List<SavedPWD>();

        public ICollection<ArchivedPWD>? ArchivedPWD { get; set; } = new List<ArchivedPWD>();

    }
}
