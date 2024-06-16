using MSTART_Hiring_Task.Models.Enum;
using System.ComponentModel.DataAnnotations;

namespace MSTART_Hiring_Task.Models.ViewModel
{
    public class UserViewModel
    {
        public string Id { get; set; }
        [Required]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; }
        [Required]
        [RegularExpression("^[a-zA-Z0-9]+$", ErrorMessage = "Only English letters and numbers are allowed")]
        [StringLength(50, MinimumLength = 4, ErrorMessage = "Username must be between 4 and 50 characters long.")]
        public string UserName { get; set; }
        [Required]
        public string First_Name { get; set; }
        [Required]
        public string Last_Name { get; set; }
        [Required(ErrorMessage = "Please enter a Date Of Birth ")]
        [DataType(DataType.Date)]
        public DateTime? Date_Of_Birth { get; set; }
        [Required(ErrorMessage = "Please Selected a User Gender field .")]
        public Gender? Gender { get; set; }
        [Required(ErrorMessage = "Please Selected a User Status field .")]
        public UserStatus Status { get; set; }
        [Display(Name = "Phone Number")]
        [Required]
        public required string PhoneNumber { get; set; }
    }
}
