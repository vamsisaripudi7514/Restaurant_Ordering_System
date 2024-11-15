using Microsoft.AspNetCore.Mvc;

namespace ROS.UI.Controllers
{
    [Route("customer")]
    public class CustomerController : Controller
    {
        public IActionResult Index()
        {
            return View("CustomerLanding");
        }

        [HttpGet("menu-selection")]
        public IActionResult MenuSelection()
        {
            return View("MenuSelection");
        }

        [HttpGet("orders")]
        public IActionResult Orders()
        {
            return View("Orders");
        }

        [HttpGet("cart")]
        public IActionResult Cart()
        {
            return View("Cart");
        }
        [HttpGet("dummy-payment")]
        public IActionResult DummyPayment()
        {
            return View("dummy-payment");
        }
        [HttpGet("payment-method-selection")]
        public IActionResult PaymentMethod()
        {
            return View("payment-method-selection");
        }
    }
}
