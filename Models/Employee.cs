using System.ComponentModel.DataAnnotations;

namespace FarmCentral.Models
{
    public class Employee
    {
        [Key]
        [Required]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
