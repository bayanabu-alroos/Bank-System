using Microsoft.AspNetCore.Identity;
using MSTART_Hiring_Task.Models.Enum;
using System.ComponentModel.DataAnnotations;

namespace MSTART_Hiring_Task.Models.ViewModel
{
    public class EditUserProfile : EditImageView
    {
        [Required]
        public string UserName { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string First_Name { get; set; }
        [Required]
        public string Last_Name { get; set; }
        [Required]
        public Gender Gender { get; set; }
        [Required]
        public DateTime? Date_Of_Birth { get; set; }
    }
}
