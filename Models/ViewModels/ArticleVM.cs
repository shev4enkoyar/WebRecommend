using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections;
using System.Collections.Generic;

namespace WebRecommend.Models.ViewModels
{
    public class ArticleVM
    {
        public ArticleVM()
        {
            Article = new Article();
        }
        public Article Article { get; set; }
        public IEnumerable<SelectListItem> CategorySelectList { get; set; }
    }
}
