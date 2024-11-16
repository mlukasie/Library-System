using System.ComponentModel.DataAnnotations;

namespace MvcLibrary.ViewModels
{
    public class UserViewModel
    {
        [Display(Name = "Username")]
        public string UserName { get; set; }
        [Display(Name = "First name")]
        public string FirstName { get; set; }
        [Display(Name = "Last name")]
        public string LastName { get; set; }
        [Display(Name = "Phone number")]
        public string PhoneNumber { get; set; }

        public string Email { get; set; }


    }
}
