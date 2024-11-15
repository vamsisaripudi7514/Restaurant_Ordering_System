using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ROS.Model.Tables;
namespace ROS.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OwnerController : Controller
    {
        private readonly ApplicationDbContext _context;
        public OwnerController(ApplicationDbContext context)
        {
            _context = context;
        }
        [HttpGet("get-payments")]
        public async Task<IActionResult> GetPayments()
        {
            var data = from Customer in _context.Customers
                       join Payment in _context.Payments on
                       Customer.Customer_ID equals Payment.Customer_ID
                       select new
                       {   Payment_ID = Payment.Payment_ID,
                           Customer_Name = Customer.Customer_Name,
                           Customer_Mail = Customer.Customer_Mail,
                           Total_Amount = Payment.Total_Amount,
                           Payment_Status = Payment.Payment_Status,
                           Date_Time = Payment.TimeStamp
                       };
            var response = data.ToList();
            if (response == null || !response.Any())
            {
                return NotFound("No payments found.");
            }
            return Ok(response);
        }
        [HttpGet("get-paid-payments")]
        public async Task<IActionResult> GetPaidPayments()
        {
            var data = from customer in _context.Customers
                       join payment in _context.Payments on
                       customer.Customer_ID equals payment.Customer_ID
                       where payment.Payment_Status == true 
                       select new
                       {
                           Customer_Name = customer.Customer_Name,
                           Customer_Mail = customer.Customer_Mail,
                           Total_Amount = payment.Total_Amount,
                           Payment_Status = payment.Payment_Status
                       };

            var response = data.ToList();
            if (response == null || !response.Any())
            {
                return NotFound("No paid payments found.");
            }
            return Ok(response);
        }
        [HttpGet("get-pending-payments")]
        public async Task<IActionResult> GetPendingPayments()
        {
            var data = from Customer in _context.Customers
                       join Payment in _context.Payments on
                       Customer.Customer_ID equals Payment.Customer_ID
                       where Payment.Payment_Status == false
                       select new
                       {
                           Payment_ID = Payment.Payment_ID,
                           Customer_Name = Customer.Customer_Name,
                           Customer_Mail = Customer.Customer_Mail,
                           Total_Amount = Payment.Total_Amount,
                           Payment_Status = Payment.Payment_Status,
                           Date_Time = Payment.TimeStamp
                       };
            var response = data.ToList();
            if (response == null || !response.Any())
            {
                return NotFound("No payments found.");
            }
            return Ok(response);
        }
        [HttpPut("mark-payment/{paymentId}")]
        public async Task<IActionResult> MarkPaymentAsPaid(string paymentId)
        {
            var payment = await _context.Payments.FirstOrDefaultAsync(p => p.Payment_ID == paymentId);
            if (payment == null)
            {
                return NotFound("Payment not found.");
            }

            payment.Payment_Status = true; 
            _context.Payments.Update(payment);
            await _context.SaveChangesAsync();

            return Ok(payment);
        }
        [HttpGet("get-customers")]
        public async Task<IActionResult> GetCustomers()
        {
            var data = from c in _context.Customers
                       select new { Customer_Name = c.Customer_Name,
                           Customer_Phone = c.Customer_Phone,
                           Customer_Mail = c.Customer_Mail
                       };
            var response = data.ToList();
            if (response == null || !response.Any())
                return NotFound("No customers found.");
            return Ok(response);
        }

        [HttpGet("get-menu")]
        public async Task<IActionResult> GetMenu()
        {
            var data = _context.Menus.ToList();
            if (data == null || !data.Any())
                return NotFound("No Menu Items found!!");
            return Ok(data);
        }

        [HttpPost("add-item")]
        public async Task<IActionResult> AddItem([FromBody] Menu newItem)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var existingItem = await _context.Menus.FirstOrDefaultAsync(m => m.Item_Name == newItem.Item_Name);
            if (existingItem != null)
            {
                return Conflict("An item with this name already exists.");
            }
            var newRecord = new Menu
            {
                Item_ID = Guid.NewGuid().ToString(),
                Item_Name = newItem.Item_Name,
                Description = newItem.Description,
                Price = newItem.Price,
                Availability = newItem.Availability,
                Time_Created = DateTime.Now
            };
            _context.Menus.Add(newRecord);
            await _context.SaveChangesAsync();
            return Ok(newRecord);
        }

        [HttpPut("update-menu/{item_id}")]
        public async Task<IActionResult> UpdateMenu(string item_id)
        {
           
            var existingItem = await _context.Menus.FirstOrDefaultAsync(m=> m.Item_ID == item_id);

            if (existingItem == null)
            {
                return NotFound($"Menu item with ID {item_id} not found."); 
            }

            
            existingItem.Availability = !existingItem.Availability;

            await _context.SaveChangesAsync();

            return Ok(existingItem);
        }


        [HttpGet("get-tables")]
        public async Task<IActionResult> GetTables()
        {
            var data = _context.Restaurant_Tables.ToList();
            if (data == null || !data.Any())
                return NotFound("No Tables found!!");
            return Ok(data);
        }

        [HttpPost("add-table")]
        public async Task<IActionResult> AddTable([FromBody] Restaurant_Table newTable)
        {
            if (!ModelState.IsValid){
                return BadRequest(ModelState);
            }
            var latest_table = await _context.Restaurant_Tables
                .OrderByDescending(m => m.Table_ID)
                .FirstOrDefaultAsync();
            var new_ID = 1;
            if (latest_table != null) new_ID = latest_table.Table_ID + 1;

            Restaurant_Table newRecord = new Restaurant_Table
            {
                Table_ID = new_ID,
                Table_Size = newTable.Table_Size,
                Table_Status = newTable.Table_Status
            };
            _context.Restaurant_Tables.Add(newRecord);
            await _context.SaveChangesAsync();
            return Ok(newRecord);

        }

       

        [HttpGet("get-analytics")]
        public async Task<IActionResult> GetAnalytics()
        {
            var pending_orders_count = await _context.Orders
                .CountAsync(o => !o.Prepared);
            var served_orders_count = await _context.Orders
                .CountAsync(o => o.Prepared);
            var total_revenue = await _context.Payments
                .Where(p => p.Payment_Status)
                .SumAsync(p => p.Total_Amount);
            var total_transactions = await _context.Payments
                .CountAsync(p=>p.Payment_Status);
            var total_customers = await _context.Customers.CountAsync();
            var topSellingItem = await _context.Orders
                .Join(_context.Menus,
                    o => o.Item_ID,     
                    m => m.Item_ID,     
                    (o, m) => new { o, m }) 
                .GroupBy(x => x.m.Item_ID) 
                .Select(g => new
                {
                    Item_Id = g.Key,
                    Item_Name = g.First().m.Item_Name, 
                    Total_Sold = g.Sum(o => o.o.Quantity) 
                })
                .OrderByDescending(g => g.Total_Sold) 
                .FirstOrDefaultAsync(); 
            var leastSellingItem = await _context.Orders
                .Join(_context.Menus,
                    o => o.Item_ID,     
                    m => m.Item_ID,     
                    (o, m) => new { o, m }) 
                .GroupBy(x => x.m.Item_ID) 
                .Select(g => new
                {
                    Item_Id = g.Key,
                    Item_Name = g.First().m.Item_Name, 
                    Total_Sold = g.Sum(o => o.o.Quantity) 
                })
                .OrderBy(g => g.Total_Sold) 
                .FirstOrDefaultAsync();
            if (topSellingItem != null && leastSellingItem != null)
            {
                var analytics = new Analytics
                {
                    PendingOrdersCount = pending_orders_count,
                    ServedOrdersCount = served_orders_count,
                    TotalRevenue = total_revenue,
                    TotalTransactions = total_transactions,
                    CustomerCount = total_customers,
                    LeastSellingItem = leastSellingItem.Item_Name,
                    TopSellingItem = new TopSellingItem
                    {
                        Item_Id = topSellingItem.Item_Id,
                        Item_Name = topSellingItem.Item_Name,
                        Total_Sold = topSellingItem.Total_Sold
                    } 
                };
                return Ok(analytics);
            }
            return Ok(null);
        }
    }
    public class Analytics
    {
        public int? PendingOrdersCount { get; set; }
        public int? ServedOrdersCount { get; set; }
        public decimal? TotalRevenue { get; set; }
        public int? TotalTransactions { get; set; }
        public int? CustomerCount { get; set; }
        public TopSellingItem? TopSellingItem { get; set; }
        public String? LeastSellingItem { get; set; }
    }

    public class TopSellingItem
    {
        public string? Item_Id { get; set; }
        public string? Item_Name { get; set; }
        public int? Total_Sold { get; set; }
    }
}