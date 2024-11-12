using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace MvcLibrary.Models
{
    public class User : IdentityUser
    {
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        [Phone]
        public string PhoneNumber { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public bool IsLibrarian { get; set; }
    }
}
