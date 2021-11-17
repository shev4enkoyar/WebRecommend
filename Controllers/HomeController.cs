using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using WebRecommend.Data;
using WebRecommend.Models;
using WebRecommend.Models.ViewModels;

namespace WebRecommend.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _db;

        public HomeController(ApplicationDbContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            HomeVM homeVM = new()
            {
                Articles = _db.Articles.Include(u => u.Category).Include(u => u.User),
                Categories = _db.Categories,
                Tags = _db.Tags
            };

            return View(homeVM);
        }

        public IActionResult Details(int id)
        {
            DetailsVM detailsVM = new()
            {
                Article = _db.Articles
                .Include(u => u.Category)
                .Include(u => u.User)
                .Where(u => u.Id == id).FirstOrDefault(),
                ArticleTag = _db.ArticleTags.Include(u => u.Tag).Where(u => u.ArticleId == id)
            };
            return View(detailsVM);
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
