using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FarmCentral.Models
{
    public class Farmer_Product
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [ForeignKey("FarmerUsername")]
        public Farmer Farmer { get; set; }
        [Required]
        public string FarmerUsername { get; set; }

        [ForeignKey("ProductName")]
        public Product Product { get; set; }
        [Required]
        public string ProductName { get; set; }

        public double Quantity { get; set; }

    }
}
