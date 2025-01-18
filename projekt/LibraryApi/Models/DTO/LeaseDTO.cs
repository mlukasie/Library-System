using System.ComponentModel.DataAnnotations;

namespace LibraryApi.Models.DTO
{
    public class LeaseDTO
    {
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        public int BookId { get; set; }

        [Required]
        public DateTimeOffset LeaseDate { get; set; }

        [Required]
        public bool IsActive { get; set; }

        [Required]
        public string BookTitle { get; set; }
        [Required]
        public string UserEmail { get; set; }

    }
}
