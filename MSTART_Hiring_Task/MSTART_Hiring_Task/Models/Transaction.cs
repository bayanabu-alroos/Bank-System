
using MSTART_Hiring_Task.Models.Enum;

namespace MSTART_Hiring_Task.Models
{
    public class Transaction
    {
        public int Id { get; set; }

        public decimal Amount { get; set; }
        public TransactionType Type { get; set; }
        public TransactionStatus Status { get; set; } // Enum (Completed, Reversed, etc.)

        public string User_Id { get; set; }
        public virtual AppUser User { get; set; }
        public int Account_Id { get; set; }

        public virtual Account Account { get; set; }
    }
}
