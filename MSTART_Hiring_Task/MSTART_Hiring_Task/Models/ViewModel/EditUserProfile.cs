using Microsoft.AspNetCore.Identity;
using MSTART_Hiring_Task.Models.Enum;
using System.ComponentModel.DataAnnotations;

namespace MSTART_Hiring_Task.Models.ViewModel
{
    public class EditUserProfile : EditImageView
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public string First_Name { get; set; }
        [Required]
        public string Last_Name { get; set; }
        public UserStatus Status { get; set; }
        public Gender? Gender { get; set; }
        public DateTime? Date_Of_Birth { get; set; }
    }
}
