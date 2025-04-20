using System.ComponentModel.DataAnnotations;

namespace Learn.Security.App.Models
{
    public class LoginModel
    {
        public string Username { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "A password é obrigatória.")]
        public string Password { get; set; }
    }
}
