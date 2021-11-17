using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Linq;

namespace WebRecommend.Models.ViewModels
{
    public class ProfileVM
    {
        public IEnumerable<Article> Article { get; set; }
        public AppUser AppUser { get; set; }
        public AppUser CurrUser { get; set; }

    }
}
