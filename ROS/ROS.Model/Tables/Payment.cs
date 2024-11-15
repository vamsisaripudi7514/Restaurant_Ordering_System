using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ROS.Model.Tables
{
    public class Payment
    {
        [Key]
        [StringLength(36)]
        public string? Payment_ID { get; set; }

        [Required]
        [StringLength(36)]
        public string? Customer_ID { get; set; }

        [ForeignKey("Customer_ID")]
        public Customer? Customer { get; set; }

        [Required]
        public decimal Total_Amount { get; set; }

        public bool Payment_Status { get; set; }

        public DateTime TimeStamp { get; set; }
    }
}
