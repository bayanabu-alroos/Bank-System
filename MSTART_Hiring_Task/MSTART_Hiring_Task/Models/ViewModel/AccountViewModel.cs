using MSTART_Hiring_Task.Models.Enum;
using System.ComponentModel.DataAnnotations;

namespace MSTART_Hiring_Task.Models.ViewModel
{
    public class AccountViewModel
    {
        public int ID { get; set; }
        [Required]
        [RegularExpression(@"^\d{7}$", ErrorMessage = "Account number should be 7 digits only.")]
        public string Account_Number { get; set; }
        [Required(ErrorMessage = "The Balance is required.")]
        [DataType(DataType.Currency, ErrorMessage = "Invalid currency format.")]
        [Range(0, double.MaxValue, ErrorMessage = "Please enter a positive value for the balance.")]
        public decimal? Balance { get; set; }
        [Required(ErrorMessage = "Please Selected Currency is required. ")]
        public string Currency { get; set; }
        [Required(ErrorMessage = "Please Selected Account Status is required. ")]
        public AccountStatus? Status { get; set; }
        [Required(ErrorMessage = "Please Selected User is required. ")]
        public string? User_ID { get; set; }
    }
}
