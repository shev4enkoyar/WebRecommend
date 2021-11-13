using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace WebRecommend.Models.ViewModels
{
    public class ProfileVM
    {
        public IEnumerable<Article> Articles { get; set; }
        public AppUser AppUser { get; set; }
        public AppUser CurrUser { get; set; }

    }
}
