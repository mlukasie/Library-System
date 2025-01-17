using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace LibraryApi.Models.DTO
{
    public class ReservationDTO
    {
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        public int BookId { get; set; }

        [Required]
        public DateTimeOffset ReservationDate { get; set; }

        [Required]
        public string BookTitle { get; set; }
        [Required]
        public string UserEmail { get; set; }

    }
}
