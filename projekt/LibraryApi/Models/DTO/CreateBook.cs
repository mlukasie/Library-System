using System.ComponentModel.DataAnnotations;

namespace LibraryApi.Models.DTO
{
    public class CreateBook
    {
        [Required]
        public string Title { get; set; }
        [Required]
        public string Author { get; set; }
        [Required]
        public string Publisher { get; set; }
        [Required]
        [DataType(DataType.Date)]
        public DateTime ReleaseDate { get; set; }
    }
}
