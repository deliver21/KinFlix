using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace KinFlix.Service.AuthAPI.Models
{
    public class ApplicationUser:IdentityUser
    {
        public string Name { get; set; }
        [NotMapped]
        public string Message { get; set; }
    }
}
