using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
namespace ROS.Model.Tables
{

        public class Customer
        {
            [Key]
            [StringLength(36)]
            public string? Customer_ID { get; set; }

            [Required]
            [StringLength(100)]
            public required string Customer_Name { get; set; }

            [Required]
            [StringLength(15)]
            public required string Customer_Phone { get; set; }

            [StringLength(255)]
            public string? Customer_Mail { get; set; }

            public ICollection<Order>? Orders { get; set; }
            
            [JsonIgnore]
            public ICollection<Payment>? Payments { get; set; }
        }


}
