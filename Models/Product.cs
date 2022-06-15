using System.ComponentModel.DataAnnotations;

namespace FarmCentral.Models
{
    public class Product
    {
        [Key]
        [Required]
        public string Name { get; set; }
        [Required]
        public string Type { get; set; }
        public double Quantity { get; set; }
    }
}
