using System.ComponentModel.DataAnnotations;

namespace LibraryApi.Models
{
    public class Book
    {
        public int Id { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string Author { get; set; }
        [Required]
        public string Publisher { get; set; }
        [Required]
        [DataType(DataType.Date)]
        public DateOnly ReleaseDate { get; set; }
        [Required]
        public bool IsVisible { get; set; }

    }

}
