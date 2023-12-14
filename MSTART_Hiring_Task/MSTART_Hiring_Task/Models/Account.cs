using MSTART_Hiring_Task.Models.Enum;
using System.ComponentModel.DataAnnotations;

namespace MSTART_Hiring_Task.Models
{
    public class Account :SharedProp
    {
        public int ID { get; set; }
        [Required]
        [RegularExpression(@"^\d{7}$", ErrorMessage = "Account number should be 7 digits only.")]
        public string Account_Number { get; set; }
        [Required(ErrorMessage = "Balance is required.")]
        [DataType(DataType.Currency, ErrorMessage = "Invalid currency format.")]
        [Range(0, double.MaxValue, ErrorMessage = "Please enter a positive value for the balance.")]
        public decimal Balance { get; set; }
        [Required]
        public string Currency { get; set; }
        [Required]
        public AccountStatus Status { get; set; }
        [Required]
        public string? User_ID { get; set; }
        public virtual AppUser User { get; set; }
        public ICollection<Transaction> Transactions { get; set; }
    }
}
