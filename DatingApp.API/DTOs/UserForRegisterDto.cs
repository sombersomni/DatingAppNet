using System.ComponentModel.DataAnnotations;

namespace DatingApp.API.DTOs
{
    public class UserForRegisterDto
    {
        [Required]
        [MaxLength(255)]
        public string UserName { get; set; }
        [Required]
        [MinLength(4, ErrorMessage = "You must specify a password greater than Length 4")]
        public string Password { get; set; }
    }
}