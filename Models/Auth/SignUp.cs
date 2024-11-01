using System.ComponentModel.DataAnnotations;

namespace CentaureaTest.Models
{

    public sealed class SignUp
    {
        // [Required]
        // [EmailAddress]
        // public string Email { get; set; } = string.Empty;

        [Required]
        public string Username { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;
    }

}