using Azure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ROS.Model.Tables;
using System.ComponentModel.DataAnnotations;

namespace ROS.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CustomerController: Controller
    {
        private readonly ApplicationDbContext _context;
        public CustomerController(ApplicationDbContext context)
        {
            _context = context;
        }

      
        [HttpPost("create-customer")]
        public async Task<IActionResult> CreateCustomer([FromBody] Customer customer)
        {
            if (!ModelState.IsValid)
                {
                   return BadRequest(ModelState);
                }
                var existingItem = await _context.Customers.FirstOrDefaultAsync(m => m.Customer_Mail == customer.Customer_Mail);

            if (existingItem != null)
            {
                var existingCart = await _context.Carts
                    .FirstOrDefaultAsync(c => c.Customer_ID == existingItem.Customer_ID);

                return Ok(new
                {
                    Customer_ID = existingItem.Customer_ID,
                    Cart_ID = existingCart?.Cart_ID ?? Guid.NewGuid().ToString(),
                    Message = "The customer already exists"
                });
            }

            var newRecord = new Customer
            {
                Customer_ID = Guid.NewGuid().ToString(),
                Customer_Name = customer.Customer_Name,
                Customer_Phone = customer.Customer_Phone,
                Customer_Mail = customer.Customer_Mail
            };

            var Cart_ID = Guid.NewGuid().ToString();

            _context.Customers.Add(newRecord);
            await _context.SaveChangesAsync();

           
            return Ok(new
            {
                Customer_ID = newRecord.Customer_ID,
                Customer_Name = newRecord.Customer_Name,
                Customer_Phone = newRecord.Customer_Phone,
                Customer_Mail = newRecord.Customer_Mail,
                Cart_ID = Cart_ID
            });
        }
       

        [HttpGet("get-menu")]
        public async Task<IActionResult> GetMenu()
        {
            var data = _context.Menus
                    .Where(item => item.Availability == true)
                    .Select(item => new
                    {
                        item.Item_ID,
                        item.Item_Name,
                        item.Description,
                        item.Price
                    });
            var response = data.ToList();
            if (response == null || !response.Any())
                return NotFound("No Menu Items found.");
            return Ok(response);

        }


        [HttpPost("place-order")]
        public async Task<IActionResult> PlaceOrder([FromBody] PlaceOrderRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var customer = await _context.Customers.FindAsync(request.Customer_ID);
            if (customer == null)
            {
                return NotFound("Customer not found.");
            }

            var table = await _context.Restaurant_Tables.FindAsync(request.Table_ID);
            if (table == null)
            {
                return NotFound("Table not found.");
            }

            var orderId = Guid.NewGuid().ToString();
            foreach (var item in request.Items)
            {
                var menuItem = await _context.Menus.FindAsync(item.Item_ID);
                if (menuItem == null || !menuItem.Availability.GetValueOrDefault())
                {
                    return BadRequest($"Item with ID {item.Item_ID} is not available.");
                }

                var order = new Order
                {
                    Order_ID = Guid.NewGuid().ToString(),
                    Customer_ID = request.Customer_ID,
                    Item_ID = item.Item_ID,
                    Table_ID = request.Table_ID,
                    Quantity = item.Quantity,
                    TimeCreated = DateTime.UtcNow,
                    Prepared = false
                };

                _context.Orders.Add(order);
            }

            await _context.SaveChangesAsync();

            return Ok("Order placed successfully.");
        }

        

        [HttpGet("cart-items")]
        public async Task<IActionResult> GetCartItems()
        {
            var data = _context.Carts;                    
            var response = data.ToList();
            if (response == null || !response.Any())
                return NotFound("No Cart Items found.");
            return Ok(response);

        }

        [HttpGet("cart-items/{id}")]
        public async Task<IActionResult> GetCartItemsByID(String id)
        {
            var response = await _context.Carts
                        .Where(c => c.Customer_ID == id) 
                        .Include(c => c.Item) 
                        .GroupBy(c => new { c.Customer_ID, c.Cart_ID }) 
                        .Select(group => new
                        {
                            Customer_ID = group.Key.Customer_ID,
                            Cart_ID = group.Key.Cart_ID,
                            Items = group.Select(c => new
                            {
                                c.Item_ID,
                                Item_Name = c.Item.Item_Name,
                                c.Quantity,
                                c.Item.Price
                            }).ToList()
                        })
                        .ToListAsync();
            if (response == null || !response.Any())
                return NotFound("No Cart Items found.");
            return Ok(response);

        }

        [HttpPost("add-to-cart")]
        public async Task<IActionResult> AddToCart([FromBody] Cart cartItem)
        {
            if (cartItem.Customer_ID == null || cartItem.Item_ID == null)
            {
                return BadRequest("Customer ID and Item ID are required.");
            }

            var customer = await _context.Customers.FindAsync(cartItem.Customer_ID);
            var menuItem = await _context.Menus.FindAsync(cartItem.Item_ID);

            if (customer == null)
            {
                return NotFound("Customer not found.");
            }

            if (menuItem == null)
            {
                return NotFound("Menu item not found.");
            }

            if (cartItem.Quantity <= 0)
            {
                return BadRequest("Quantity must be greater than 0.");
            }
            var existingCartItem = await _context.Carts
                    .FromSqlRaw("SELECT * FROM Carts WHERE Cart_ID = {0} AND Customer_ID = {1} AND Item_ID = {2}",
                    cartItem.Cart_ID, cartItem.Customer_ID, cartItem.Item_ID)
                    .FirstOrDefaultAsync();
            if (existingCartItem != null)
            {
                await _context.Database.ExecuteSqlRawAsync(
                    "UPDATE Carts SET Quantity = {0} WHERE Cart_ID = {1} AND Customer_ID = {2} AND Item_ID = {3}",
                    cartItem.Quantity, cartItem.Cart_ID, cartItem.Customer_ID, cartItem.Item_ID);


                await _context.SaveChangesAsync();
                return Ok("Item updated successfully.");
            }
            else
            {
                try
                {
                    if (cartItem.Cart_ID == "" || cartItem.Cart_ID == null)
                        cartItem.Cart_ID = Guid.NewGuid().ToString();
                    _context.Carts.Add(cartItem);
                    await _context.SaveChangesAsync();

                    return Ok("Item added to cart successfully.");
                }
                catch (Exception ex)
                {
                    return StatusCode(500, $"Internal server error: {ex.Message}");
                }
            }
        }
        [HttpPut("remove-from-cart/{Cart_ID}/{Customer_ID}/{Item_ID}")]
        public async Task<IActionResult> RemoveFromCart(string Cart_ID, string Customer_ID, string Item_ID)
        {
            if(Cart_ID == null || Customer_ID == null || Item_ID == null)
            {
                return BadRequest("Input field cannot be null");
            }
            var item = await _context.Carts.Where(cart => cart.Cart_ID == Cart_ID &&
                                cart.Item_ID == Item_ID &&
                                cart.Customer_ID == Customer_ID).FirstOrDefaultAsync();
            if (item == null)
            {
                return BadRequest("Item not found in cart");
            }
            await _context.Database.ExecuteSqlRawAsync(
                    "delete from carts where Cart_ID = {0}  and Customer_ID = {1} and Item_ID = {2}",
                    Cart_ID,Customer_ID,Item_ID);
            await _context.SaveChangesAsync();
            return Ok("Item Removed Successfully");

        }

        [HttpGet("get-orders/{id}")]
        public async Task<IActionResult> GetOrders(string id)
        {
            var orders = await _context.Orders
                    .Where(order => order.Customer_ID == id)
                    .Join(
                        _context.Menus,
                        order => order.Item_ID,
                        menu => menu.Item_ID,
                        (order, menu) => new
                        {
                            Item_Name = menu.Item_Name,
                            Quantity = order.Quantity,
                            Order_Value = Math.Round(order.Quantity * (menu.Price ?? 0), 2),
                            Status = order.Prepared
                        }
                    )
                    .ToListAsync();
            if (orders == null || !orders.Any())
                return NotFound("No Cart Items found.");
            return Ok(orders);

        }

        [HttpPost("add-to-payments/{Customer_ID}/{Total_Amount}/{type}")]
        public async Task<IActionResult> AddToPayments(string Customer_ID, decimal Total_Amount,string type)
        {
            if (Customer_ID == null || Total_Amount == null)
            {
                return BadRequest("Payment request is null.");
            }

            var customer = await _context.Customers.FindAsync(Customer_ID);
            if (customer == null)
            {
                return NotFound("Customer not found.");
            }
            bool paymentType = bool.TryParse(type, out var result) && result;
            var payment = new Payment
            {
                Payment_ID = Guid.NewGuid().ToString(),
                Customer_ID = Customer_ID,
                Total_Amount = Total_Amount,
                Payment_Status = paymentType,
                TimeStamp = DateTime.UtcNow
            };

            try
            {
                _context.Payments.Add(payment);
                await _context.SaveChangesAsync();

                return Ok(new { Message = "Payment successfully added.", Payment = payment });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

    }
    public class PlaceOrderRequest
    {
        [Required]
        public string Customer_ID { get; set; }

        [Required]
        public int Table_ID { get; set; }

        [Required]
        public List<OrderItem> Items { get; set; }
    }

    public class OrderItem
    {
        [Required]
        public string Item_ID { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than 0.")]
        public int Quantity { get; set; }
    }
    public class PaymentRequest
    {
        [Required]
        public string Customer_ID { get; set; }

        [Required]
        public decimal Total_Amount { get; set; }
    }
}
