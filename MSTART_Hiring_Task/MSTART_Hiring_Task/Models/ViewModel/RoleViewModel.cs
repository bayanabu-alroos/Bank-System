using System.ComponentModel.DataAnnotations;

namespace MSTART_Hiring_Task.Models.ViewModel
{
    public class RoleViewModel
    {
        [Required]
        [Display(Name = "Role Name")]
        public required string RoleName { get; set; }
    }
}
