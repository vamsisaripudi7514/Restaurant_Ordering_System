using Microsoft.AspNetCore.Mvc;
using ROS.Model.Tables;

namespace ROS.UI.Controllers
{
    [Route("admin")]
    public class AdminController:Controller
    {
        private readonly ApplicationDbContext _context;
        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Index() { return View("AdminLanding"); }

        [HttpGet("menu")]
        public IActionResult Menu() { return View("MenuItems"); }
        
        [HttpGet("menu-edit")]
        public IActionResult MenuEdit() { return View("MenuEdit"); }

        [HttpGet("order")]
        public IActionResult Order() { return View("Orders"); }
        [HttpGet("payment")]
        public IActionResult Payment() { return View("Payments"); }

        [HttpGet("analytics")]
        public IActionResult Analytics() { return View("Analytics"); }

        [HttpGet("customers")]
        public IActionResult Customers() { return View("Customers"); }

        [HttpGet("AddItem")]
        public IActionResult AddItem() { return View("AddItem"); }
    }
}
