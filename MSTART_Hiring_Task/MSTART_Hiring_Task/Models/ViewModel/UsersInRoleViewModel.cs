using System.ComponentModel.DataAnnotations;

namespace MSTART_Hiring_Task.Models.ViewModel
{
    public class UsersInRoleViewModel
    {
        public string UserId { get; set; }
        [Display(Name = "User Name")]
        public string UserName { get; set; }
        public bool IsSelected { get; set; }
    }
}
