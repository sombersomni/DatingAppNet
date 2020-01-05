using System.ComponentModel.DataAnnotations;
namespace DatingApp.API.Models
{
    public class User
    {
        public int UserId { get; set; }
        [Required]
        [MaxLength(255)]
        public string UserName { get; set; }
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }
    }
}