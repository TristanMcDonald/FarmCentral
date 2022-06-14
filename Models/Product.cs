using System.ComponentModel.DataAnnotations;

namespace FarmCentral.Models
{
    public class Product
    {
        [Key]
        [Required]
        public string Name { get; set; }
    }
}
