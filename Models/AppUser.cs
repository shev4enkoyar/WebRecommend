using Microsoft.AspNetCore.Identity;

namespace WebRecommend.Models
{
    public class AppUser : IdentityUser
    {
        public string FullName { get; set; }
        public int Reputation { get; set; }
    }
}
