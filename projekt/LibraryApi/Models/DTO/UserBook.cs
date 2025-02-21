namespace LibraryApi.Models.DTO
{
    public class UserBook
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public bool isAvailable { get; set; }
    }
}
