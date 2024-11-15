using Microsoft.AspNetCore.Mvc;
using ROS.Model.Tables; 
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
namespace ROS.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ChefController : Controller
    {
        private readonly ApplicationDbContext _context; 

        public ChefController(ApplicationDbContext context)
        {
            _context = context;
        }


        [HttpGet("pending-orders")]
        public async Task<IActionResult> GetPendingOrders()
        {
            var query = from o in _context.Orders
                        join c in _context.Customers on o.Customer_ID equals c.Customer_ID
                        join m in _context.Menus on o.Item_ID equals m.Item_ID
                        where !o.Prepared 
                        select new
                        {
                            OrderId = o.Order_ID,
                            CustomerName = c.Customer_Name,
                            ItemName = m.Item_Name,
                            Quantity = o.Quantity,
                            Price = o.Quantity * m.Price
                        };

            var pendingOrders = query.ToList();

            if (pendingOrders == null || !pendingOrders.Any())
            {
                return NotFound("No pending orders found.");
            }

            return Ok(pendingOrders);
        }

        [HttpGet("served-orders")]
        public async Task<IActionResult> ServedOrders()
        {
            var orders = from o in _context.Orders
                        join c in _context.Customers on o.Customer_ID equals c.Customer_ID
                        join m in _context.Menus on o.Item_ID equals m.Item_ID
                        where o.Prepared 
                        select new
                        {
                            OrderId = o.Order_ID,
                            CustomerName = c.Customer_Name,
                            ItemName = m.Item_Name,
                            Quantity = o.Quantity,
                            Price = o.Quantity*m.Price
                        };
            if (orders == null || !orders.Any())
            {
                return NotFound("No completed orders found.");
            }
            return Ok(orders);
        }
        [HttpPut("serve-order/{id}")]
        public async Task<IActionResult> ServeOrder(string id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound($"Order with ID {id} not found.");
            }

            order.Prepared = true; 

            await _context.SaveChangesAsync();

            return Ok($"Order with ID {id} has been marked as served.");
        }
    }
}
