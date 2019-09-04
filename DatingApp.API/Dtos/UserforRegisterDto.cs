using System.ComponentModel.DataAnnotations;

namespace DatingApp.API.Dtos
{
    public class UserforRegisterDto
    {
        [Required]
        public string  Username { get; set; }
        [Required]
        [StringLength(8,MinimumLength=4,ErrorMessage="Password Must be between 4 and 8 letters")]        
        public string Password { get; set; }

    }
}