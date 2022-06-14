using System.ComponentModel.DataAnnotations;

namespace FarmCentral.Models
{
    public class Farmer
    {
        [Key]
        [Required]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
