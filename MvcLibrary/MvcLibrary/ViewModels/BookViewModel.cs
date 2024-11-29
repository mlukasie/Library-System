using System.ComponentModel.DataAnnotations;

namespace MvcLibrary.ViewModels
{
    public class BookViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Release Date")]
        public DateTime? ReleaseDate { get; set; }

        public bool IsReservedOrLeased { get; set; }
    }
}
