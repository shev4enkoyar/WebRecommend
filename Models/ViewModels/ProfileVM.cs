﻿using System.Collections.Generic;

namespace WebRecommend.Models.ViewModels
{
    public class ProfileVM
    {
        public IEnumerable<Article> Article { get; set; }
        public AppUser AppUser { get; set; }
        public AppUser CurrUser { get; set; }

    }
}
