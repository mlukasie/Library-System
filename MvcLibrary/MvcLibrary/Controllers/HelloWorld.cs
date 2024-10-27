using Microsoft.AspNetCore.Mvc;

namespace MvcLibrary.Controllers
{
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
    }
}
