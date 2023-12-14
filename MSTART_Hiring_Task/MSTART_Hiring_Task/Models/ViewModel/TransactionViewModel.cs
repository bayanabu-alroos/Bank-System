using MSTART_Hiring_Task.Models.Enum;
using System.ComponentModel.DataAnnotations;

namespace MSTART_Hiring_Task.Models.ViewModel
{
    public class TransactionViewModel
    {
        public int Id { get; set; }
        [Required]
        public decimal? Amount { get; set; }
        [Required]
        public TransactionType? Type { get; set; }
        [Required]
        public TransactionStatus? Status { get; set; } // Enum (Completed, Reversed, etc.)
        [Required]
        public string? User_Id { get; set; }
        [Required]
        public int? Account_Id { get; set; }
    }
}
