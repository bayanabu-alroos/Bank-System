using Elfie.Serialization;
using Microsoft.AspNetCore.Identity;
using MSTART_Hiring_Task.Models.Enum;
using System.ComponentModel.DataAnnotations;


namespace MSTART_Hiring_Task.Models
{
    public class AppUser : IdentityUser
    {
        [Required]
        public string First_Name { get; set; }
        [Required]
        public string Last_Name { get; set; }
        public UserStatus Status { get; set; }
        public Gender? Gender { get; set; }
        [DataType(DataType.Date)]
        public DateTime? Date_Of_Birth { get; set; }
        public string? ImageProfile { get; set; }
        public DateTime Server_DateTime { get; set; } = DateTime.Now;
        public DateTime DateTime_UTC { get; set; } = DateTime.Now;
        public DateTime Update_DateTime_UTC { get; set; } = DateTime.Now;
        public ICollection<Account> Accounts { get; set; }
        public ICollection<Transaction> Transactions { get; set; }

    }
}
