using System.ComponentModel.DataAnnotations;

namespace HydraSim.Web.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "E-mail is required.")]
        [EmailAddress(ErrorMessage = "Please enter a valid E-mail.")]
        public string Email { get; set; } = "";

        [Required(ErrorMessage = "Password is required.")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = "";
    }
}
