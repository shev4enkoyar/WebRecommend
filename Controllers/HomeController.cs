using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Diagnostics;
using System.Linq;
using WebRecommend.Data;
using WebRecommend.Models;
using WebRecommend.Models.ViewModels;
using static WebRecommend.WebConst;

namespace WebRecommend.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<AppUser> _userManager;

        public HomeController(ApplicationDbContext db, UserManager<AppUser> userManager)
        {
            _userManager = userManager;
            _db = db;
        }

        [HttpPost]
        public IActionResult SetLanguage(string culture, string returnUrl)
        {
            Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1), IsEssential = true }
            );
            return LocalRedirect(returnUrl);
        }

        public IActionResult ChangeTheme(string returnUrl)
        {
            string userTheme = Request.Cookies["theme"];
            userTheme = ChangeUserTheme(userTheme);
            Response.Cookies.Append("theme", userTheme);
            return LocalRedirect(returnUrl);
        }

        private static string ChangeUserTheme(string theme)
        {
            switch (theme)
            {
                case "light":
                    return "dark";
                case "dark":
                    return "light";
                default:
                    goto case "light";
            }
        }

        public IActionResult Index(SortState sortOn = SortState.RatingDesc)
        {
            DeleteUnusedTags();
            HomeVM homeVM = new()
            {
                Categories = _db.Categories,
                Tags = _db.Tags
            };
            ViewBag.RatingSort = sortOn == SortState.RatingDesc ? SortState.RatingAsc : SortState.RatingDesc;
            ViewBag.DateSort = sortOn == SortState.DateDesc ? SortState.DateAsc : SortState.DateDesc;
            var articles = _db.Articles.Include(u => u.Category).Include(u => u.User).OrderByDescending(u => u.RatingAverage);
            homeVM.Articles = SortArticles(sortOn, articles);
            return View(homeVM);
        }

        private static IOrderedQueryable<Article> SortArticles(SortState sortState, IOrderedQueryable<Article> articles)
        {
            switch (sortState)
            {
                case SortState.RatingAsc:
                    return articles.OrderBy(u => u.RatingAverage);
                case SortState.RatingDesc:
                    return articles.OrderByDescending(u => u.RatingAverage);
                case SortState.DateAsc:
                    return articles.OrderBy(u => u.CreateDate);
                case SortState.DateDesc:
                    return articles.OrderByDescending(u => u.CreateDate);
                default:
                    goto case SortState.RatingAsc;
            }
        }

        private void DeleteUnusedTags()
        {
            _db.Database.ExecuteSqlRaw("DELETE FROM tags WHERE Id NOT IN (SELECT TagId FROM articletags)");
        }

        public IActionResult Details(int id)
        {
            DetailsVM detailsVM = GetDetailsVM(id);
            GetInfoForAuthUser(id, detailsVM);
            return View(detailsVM);
        }

        private DetailsVM GetDetailsVM(int articleId)
        {
            DetailsVM detailsVM = new()
            {
                Article = _db.Articles
                .Include(u => u.Category)
                .Include(u => u.User)
                .Where(u => u.Id == articleId).FirstOrDefault(),
                ArticleTag = _db.ArticleTags.Include(u => u.Tag).Where(u => u.ArticleId == articleId)
            };
            return detailsVM;
        }

        private void GetInfoForAuthUser(int id, DetailsVM detailsVM)
        {
            if (User.Identity.IsAuthenticated)
            {
                var currUserId = _userManager.GetUserAsync(User).Result.Id;
                FillReputation(id, detailsVM, currUserId);
                FillRating(id, detailsVM, currUserId);
            }
        }

        private void FillReputation(int id, DetailsVM detailsVM, string currUserId)
        {
            if (_db.Reputation.Any(u => u.ArticleId == id))
            {
                detailsVM.Reputation = _db.Reputation.Where(u => u.ArticleId == id && u.UserId == currUserId).FirstOrDefault();
            }
        }

        private void FillRating(int id, DetailsVM detailsVM, string currUserId)
        {
            if (_db.Rating.Any(u => u.ArticleId == id))
            {
                detailsVM.Rating = _db.Rating.Where(u => u.ArticleId == id && u.UserId == currUserId).FirstOrDefault();
            }
        }

        [HttpPost]
        [Authorize(Roles = "admin,user")]
        public IActionResult DetailsReputation(DetailsVM detailsVM)
        {
            var articleId = detailsVM.Article.Id;
            var currUserId = _userManager.GetUserAsync(User).Result.Id;
            Reputation reputation = GetReputation(detailsVM, currUserId);
            if (!_db.Reputation.Any(i => i.UserId == currUserId && i.ArticleId == articleId))
            {
                AddReputation(reputation, detailsVM);
            }
            return Redirect($"~/Home/Details/{articleId}");
        }

        private static Reputation GetReputation(DetailsVM detailsVM, string currUserId)
        {
            Reputation reputation = new()
            {
                ArticleId = detailsVM.Article.Id,
                UserId = currUserId,
                AuthorId = detailsVM.Article.UserId
            };
            return reputation;
        }

        private void AddReputation(Reputation reputation, DetailsVM detailsVM)
        {
            _db.Reputation.Add(reputation);
            _db.SaveChanges();
            var articleUser = _db.ApplicationUsers.Where(u => u.Id == detailsVM.Article.UserId).FirstOrDefault();
            articleUser.Reputation = _db.Reputation.Count(u => u.AuthorId == detailsVM.Article.UserId);
            _db.ApplicationUsers.Update(articleUser);
            _db.SaveChanges();
        }

        [HttpPost]
        [Authorize(Roles = "admin,user")]
        public IActionResult DetailsRating(DetailsVM detailsVM)
        {
            var articleId = detailsVM.Article.Id;
            if (detailsVM.Rating == null)
            {
                return Redirect($"~/Home/Details/{articleId}");
            }
            var currUserId = _userManager.GetUserAsync(User).Result.Id;
            Rating rating = GetRating(detailsVM, currUserId);
            AddOrUpdateRating(detailsVM, currUserId, rating);
            _db.SaveChanges();
            var article = _db.Articles.Where(u => u.Id == articleId).FirstOrDefault();
            article.RatingAverage = Math.Round(_db.Rating.Where(u => u.ArticleId == articleId).Average(u => u.Grade), 2);
            _db.SaveChanges();
            return Redirect($"~/Home/Details/{articleId}");
        }

        private static Rating GetRating(DetailsVM detailsVM, string currUserId)
        {
            Rating rating = new()
            {
                ArticleId = detailsVM.Article.Id,
                UserId = currUserId,
                Grade = detailsVM.Rating.Grade
            };
            return rating;
        }

        private void AddOrUpdateRating(DetailsVM detailsVM, string currUserId, Rating rating)
        {
            if (!_db.Rating.Any(i => i.UserId == currUserId && i.ArticleId == detailsVM.Article.Id))
            {
                _db.Rating.Add(rating);
            }
            else
            {
                _db.Rating.Update(rating);
            }
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
