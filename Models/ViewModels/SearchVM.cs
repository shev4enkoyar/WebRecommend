using System.Collections.Generic;

namespace WebRecommend.Models.ViewModels
{
    public class SearchVM
    {
        public IEnumerable<Article> Articles { get; set; }
        public string UserQuery { get; set; }
    }
}
