using LibraryApi.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace LibraryApi.Models
{
    public class Lease
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

        [ForeignKey("UserId")]
        public virtual User User { get; set; }

        [ForeignKey("BookId")]
        public virtual Book Book { get; set; }
    }
}
