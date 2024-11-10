using System.ComponentModel.DataAnnotations;

namespace MvcLibrary.Models
{
	public class Book
	{
		public int Id { get; set; }
		public string? Title { get; set; }

		public string? Author { get; set; }
		public string? Publisher { get; set; }
		[DataType(DataType.Date)]
		public DateTime ReleaseDate { get; set; }
		public bool IsAvailable { get; set; }
		public string? Description { get; set; }
		public bool IsVisible { get; set; }
		
	}
}
