using System.ComponentModel.DataAnnotations;

namespace ROS.Model.Tables
{
    public class Menu
    {
        [Key]
        [StringLength(36)]
        public string? Item_ID { get; set; }

        [Required]
        [StringLength(100)]
        public string? Item_Name { get; set; }

        public string? Description { get; set; }

        [Required]
        public decimal? Price { get; set; }

        public bool? Availability { get; set; }

        public DateTime? Time_Created { get; set; }
        public ICollection<Order>? Orders { get; set; }
    }
}
