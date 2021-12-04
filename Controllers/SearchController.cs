using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using WebRecommend.Data;
using WebRecommend.Models;
using WebRecommend.Models.ViewModels;

namespace WebRecommend.Controllers
{
    public class SearchController : Controller
    {
        private readonly ApplicationDbContext _db;

        public SearchController(ApplicationDbContext db)
        {
            _db = db;
        }

        public IActionResult Index(string search_query)
        {
            SearchVM searchVM = new();
            if (search_query == null)
            {
                return View(searchVM);
            }

            search_query = search_query.Trim();
            if (search_query != null && search_query.Length > 0)
            {
                searchVM.UserQuery = search_query;

                IEnumerable<Article> TitleArt = _db.Articles
                    .Include(u => u.Category)
                    .Include(u => u.User)
                    .Where(x => EF.Functions
                    .Match(x.Title, search_query, MySqlMatchSearchMode.NaturalLanguage));
                IEnumerable<Article> DescriptionArt = _db.Articles
                    .Include(u => u.Category)
                    .Include(u => u.User)
                    .Where(x => EF.Functions
                    .Match(x.Desсription, search_query, MySqlMatchSearchMode.NaturalLanguage));
                IEnumerable<Article> BodyArt = _db.Articles
                    .Include(u => u.Category)
                    .Include(u => u.User)
                    .Where(x => EF.Functions
                    .Match(x.Body, search_query, MySqlMatchSearchMode.NaturalLanguage));

                searchVM.Articles = TitleArt.Union(DescriptionArt.Union(BodyArt));
            }

            return View(searchVM);
        }
    }
}
