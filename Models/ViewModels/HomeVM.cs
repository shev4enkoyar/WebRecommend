using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace WebRecommend.Models.ViewModels
{
    public class HomeVM
    {
        public IEnumerable<Article> Articles { get; set; }
        public IEnumerable<Category> Categories { get; set; }
        public IEnumerable<Tag> Tags { get; set; }
    }
}
