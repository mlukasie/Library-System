using MvcLibrary.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Xml.XPath;

namespace MvcLibrary.ViewModels
{
    public class ReservedBookViewModel
    {
            public int ReservationId { get; set; }

            [Display(Name = "Reserved on")]
            public DateTime DateReserved { get; set; }
            public string UserId { get; set; }

            [Display(Name = "Username")]
            public string UserName { get; set; }

            // Book properties
            public int? BookId { get; set; }

            [Display(Name = "Title of the book")]
            public string Title { get; set; }
    }

}
