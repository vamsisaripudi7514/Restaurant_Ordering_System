using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace ROS.Model.Tables
{
    public class Order
    {
        [Key]
        [StringLength(36)]
        public string? Order_ID { get; set; }

        [Required]
        [StringLength(36)]
        public string? Customer_ID { get; set; }

        [ForeignKey("Customer_ID")]
        public Customer? Customer { get; set; }

        [Required]
        [StringLength(36)]
        public string? Item_ID { get; set; }

        [ForeignKey("Item_ID")]
        public Menu? Item { get; set; }

        [Required]
        public int Table_ID { get; set; }

        [ForeignKey("Table_ID")]
        public Restaurant_Table? Restaurant_Table { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than 0.")]
        public int Quantity { get; set; }

        public DateTime TimeCreated { get; set; }
        public bool Prepared { get; set; }
    }
}
