using Microsoft.AspNetCore.Mvc;

namespace ROS.UI.Controllers
{
    [Route("chef")]
    public class ChefController : Controller
    {
        public IActionResult Dashboard()
        {
            return View("ChefDashboard"); 
        }
    }
}
