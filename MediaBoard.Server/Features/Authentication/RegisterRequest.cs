using System.ComponentModel.DataAnnotations;

namespace MediaBoard.Server.Features.Authentication
{
    public class RegisterRequest
    {
        [Required]
        [StringLength(50, MinimumLength = 3)]

        public string Username { get; set; } = null!;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;

        [Required] 
        [MinLength(8)] 
        public string Password { get; set; } = null!;
    }
}
