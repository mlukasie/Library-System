namespace LibraryApi.Models.DTO
{
    public class LibrarianBook
    {
        public int Id { get; set; }
        public string Author { get; set; }
        public string Title { get; set; }
        public bool IsVisible { get; set; }
    }
}
