using System.ComponentModel.DataAnnotations;

namespace MSTART_Hiring_Task.Models.ViewModel
{
    public class LoginViewModel
    {
        [Display(Name = "str-email")]
        [Required(ErrorMessage = "str-email-error")]
        [EmailAddress]
        public string Email { get; set; }
        [Display(Name = "str-password")]
        [Required(ErrorMessage = "str-password-error")]
        [DataType(DataType.Password)]

        public string Password { get; set; }
        [Display(Name = "str-rememberme")]
        public bool RememberMe { get; set; }
    }
}
