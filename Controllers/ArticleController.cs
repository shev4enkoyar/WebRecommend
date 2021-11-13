using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using WebRecommend.Data;
using WebRecommend.Models;
using WebRecommend.Models.ViewModels;

namespace WebRecommend.Controllers
{
    [Authorize]
    public class ArticleController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ArticleController(ApplicationDbContext db, IHttpContextAccessor httpContextAccessor)
        {
            _db = db;
            _httpContextAccessor = httpContextAccessor;
        }

        public IActionResult Index()
        {
            IEnumerable<Article> articles = _db.Articles.Include(u => u.Category).Include(u => u.User);
            return View(articles);
        }

        public IActionResult Upsert(int? id)
        {
            ArticleVM articleVM = new ArticleVM()
            {
                CategorySelectList = _db.Categories.Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Id.ToString()
                })
            };
            if (id == null)
            {
                return View(articleVM);
            }
            else
            {
                articleVM.Article = _db.Articles.Find(id);
                if (articleVM.Article == null)
                {
                    return NotFound();
                }
                return View(articleVM);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(ArticleVM articleVM)
        {

            if (ModelState.IsValid)
            {
                //Create
                if (articleVM.Article.Id == 0)
                {
                    articleVM.Article.CreateDate = DateTime.UtcNow.Date;
                    articleVM.Article.UserId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
                    _db.Articles.Add(articleVM.Article);
                }
                else
                {
                    var product = _db.Articles.AsNoTracking().FirstOrDefault(u => u.Id == articleVM.Article.Id);
                    _db.Articles.Update(articleVM.Article);
                }
                _db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(articleVM);

        }

        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            //Жадная загрузка с загрузкой категорий
            Article product = _db.Articles.Include(u => u.Category).FirstOrDefault(u => u.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeletePost(int? id)
        {
            var obj = _db.Articles.Find(id);
            if (obj == null)
            {
                return NotFound();
            }
            _db.Articles.Remove(obj);
            _db.SaveChanges();
            return RedirectToAction("Index");

        }
    }
}
