using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MvcLibrary.Controllers
{
    [Authorize]
    public class HelloWorld : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public string Welcome()
        {
         
            return "This is my welcome action...";
        }

        public string UserBooks()
        {
            return "You are user";
        }

        [Authorize(Roles = "Librarian")]
        public string LibrarianBooks()
        {
            return "You are librarian";
        }
    }
}
