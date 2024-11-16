using System.ComponentModel.DataAnnotations.Schema;

namespace MvcLibrary.Models
{
    public class Lease
    {
        public int Id { get; set; }

        public string UserId { get; set; }

        public int BookId { get; set; }

        public DateTime LeaseDate { get; set; }

        public bool IsActive { get; set; }

        [ForeignKey("UserId")]
        public virtual User User { get; set; }

        [ForeignKey("BookId")]
        public virtual Book Book { get; set; }
    }
}
