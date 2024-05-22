using System.ComponentModel.DataAnnotations;

namespace Application.DTOs
{
    public class RegisterUserDto
    {
        [Required]
        public string Name { get; set; } = string.Empty;
        
        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;
        
        [Required]
        public string Password { get; set; } = string.Empty;
        
        [Required, Compare(nameof(Password))]
        public string RepeatPassword { get; set;} = string.Empty;
    }
}
