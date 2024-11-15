using System.ComponentModel.DataAnnotations;

namespace ROS.Model.Tables
{
    public class Restaurant_Table
    {
        [Key]
        public int Table_ID { get; set; }

        public short Table_Size { get; set; }

        public bool Table_Status { get; set; }

        public ICollection<Order>? Orders { get; set; }
    }
}
