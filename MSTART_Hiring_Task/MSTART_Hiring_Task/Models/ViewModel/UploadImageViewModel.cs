using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace MSTART_Hiring_Task.Models.ViewModel
{
    public class UploadImageViewModel: IdentityUser
    {
        [Display(Name = "Picture")]
        public IFormFile PictureProfile { get; set; }
    }
}
