using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MvcLibrary.Models
{
	public class Book
	{
		public int Id { get; set; }
        [Required]
        public string? Title { get; set; }
        [Required]
        public string? Author { get; set; }
        [Required]
        public string? Publisher { get; set; }
        [Required]
        [DataType(DataType.Date)]
        [Display(Name="Release Date")]
		public DateTime? ReleaseDate { get; set; }
        [Required]
        [Display(Name = "Is Available")]
        public bool IsAvailable { get; set; }
        [Required]
        public string? Description { get; set; }

        [Display(Name = "Deleted?")]
		public bool IsVisible { get; set; }

        [Timestamp]
        public byte[] Timestamp { get; set; }

    }
}
