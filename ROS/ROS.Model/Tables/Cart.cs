using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ROS.Model.Tables
{
    public class Cart
    {
        [Key]
        [StringLength(36)]
        public string? Cart_ID { get; set; } = Guid.NewGuid().ToString();

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

        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than 0.")]
        public int Quantity { get; set; }

    }
}
