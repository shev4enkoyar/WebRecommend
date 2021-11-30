using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

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

        public IEnumerable<SelectListItem> UsersSelectList { get; set; }

        public IEnumerable<ArticleTag> ArticleTag { get; set; }

        public string ArticleTagLine { get; set; }

        public IEnumerable<Tag> Tags { get; set; }
    }
}
