using System.ComponentModel.DataAnnotations;

namespace CRUD_JWT_Auth.Models
{
    public class User
    {
        
        [Required] 
        public string Name { get; set; }
        [Required]
        public int Age { get; set; }
        [Required]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public string Role { get; set; }

    }
}
