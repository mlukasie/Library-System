using System.ComponentModel.DataAnnotations;

namespace LibraryApi.Models.DTO
{
    public class LoginUser
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
