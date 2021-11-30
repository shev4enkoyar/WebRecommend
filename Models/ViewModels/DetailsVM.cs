using System.Collections;
using System.Collections.Generic;

namespace WebRecommend.Models.ViewModels
{
    public class DetailsVM
    {
        public Article Article { get; set; }

        public IEnumerable<ArticleTag> ArticleTag { get; set; }

        public Rating Rating { get; set; }
        public Reputation Reputation { get; set; }

        public AppUser AppUser { get; set; }

        public IEnumerable<Comment> Comment { get; set; }
    }
}
