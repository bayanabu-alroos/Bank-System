using MSTART_Hiring_Task.Models.Enum;
using System.ComponentModel.DataAnnotations;

namespace MSTART_Hiring_Task.Models.ViewModel
{
    public class RegisterViewModel 
    {
        [Display(Name = "str-first-name")]
        [Required(ErrorMessage ="str-first-name-error")]
        public string First_Name { get; set; }
        [Display(Name = "str-last-name")]
        [Required(ErrorMessage = "str-last-name-error")]
        public string Last_Name { get; set; }
        [Required(ErrorMessage = "Please Selected a User Status field .")]
        public UserStatus Status { get; set; } = UserStatus.Pending;
        [Display(Name = "str-gender")]
        [Required(ErrorMessage = "str-gender-error")]
        public Gender Gender { get; set; }

        [Display(Name = "str-date-of-birth")]
        [Required(ErrorMessage = "str-date-of-birth-error")]
        [DataType(DataType.Date)]

        public DateTime Date_Of_Birth { get; set; }
        [Display(Name = "str-username")]
        [Required(ErrorMessage = "str-username-error")]
        [RegularExpression("^[a-zA-Z0-9]+$", ErrorMessage = "str-username-regular")]
        [StringLength(50, MinimumLength = 4, ErrorMessage = "str-username-length")]
        public string UserName { get; set; }
        [Display(Name = "str-email")]
        [Required(ErrorMessage = "str-email-error")]
        [EmailAddress]
        public  string Email { get; set; }
        [Display(Name = "str-password")]
        [Required(ErrorMessage = "str-password-error")]
        [DataType(DataType.Password)]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[!@#$%^&*]).{8,}$", ErrorMessage = "str-password-regular")]

        public  string Password { get; set; }
        [Display(Name = "str-password-confirm")]
        [Required(ErrorMessage = "str-password-confirm-error")]
        [Compare("Password", ErrorMessage = "str-password-confirm-compare")]
        [DataType(DataType.Password)]
        public  string ConfirmPassword { get; set; }
        [Display(Name = "str-phone")]
        [Required(ErrorMessage = "str-phone-error")]
        public  string PhoneNumber { get; set; }
    }
}
