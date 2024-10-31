using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace cst350groupapp.Models
{
    public class RegisterViewModel
    {
        [Required]
        [DisplayName("First Name")]
        public string FirstName { get; set; }

        [Required]
        [DisplayName("Last Name")]
        public string LastName { get; set; }

        [Required]
        [DisplayName("Gender")]
        public int Sex { get; set; }

        [Required]
        [DisplayName("Age")]
        [Range(1, 120)]
        public int Age { get; set; }

        [Required]
        [DisplayName("Email")]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DisplayName("Username")]
        [StringLength(20, MinimumLength = 4)]
        public string Username { get; set; }

        [Required]
        [DisplayName("Password")]
        // at least 8 characters long, contain at least 1 uppercase letter, 1 lowercase letter, and 1 number
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,}$", ErrorMessage = "Password must be 8 characters long and contain at least 1 uppercase letter, 1 lowercase letter, and 1 number")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
