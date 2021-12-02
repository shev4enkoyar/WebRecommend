using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace WebRecommend.Models.ViewModels
{
    public class ChangeRoleVM
    {
        public string UserId { get; set; }
        public string UserEmail { get; set; }
        public List<IdentityRole> AllRoles { get; set; }
        public IList<string> UserRoles { get; set; }
        public ChangeRoleVM()
        {
            AllRoles = new List<IdentityRole>();
            UserRoles = new List<string>();
        }
    }
}
