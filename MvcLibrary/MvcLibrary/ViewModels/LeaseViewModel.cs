using System.ComponentModel.DataAnnotations;

namespace MvcLibrary.ViewModels
{
    public class LeaseViewModel
    {
            public int LeaseId { get; set; }

            [Display(Name = "Leased on")]
            public DateTime DateLease { get; set; }
            public string UserId { get; set; }

            [Display(Name = "Username")]
            public string UserName { get; set; }

            // Book properties
            public int? BookId { get; set; }

            [Display(Name = "Title of the book")]
            public string Title { get; set; }

            [Display(Name = "Is active?")]
            public bool IsActive { get; set; }
        }

    }
