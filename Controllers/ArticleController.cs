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
                }),
                UsersSelectList = _db.ApplicationUsers.Select(i => new SelectListItem
                {
                    Text = $"{i.FullName}:{i.UserName}",
                    Value = i.Id.ToString()
                }),
                Tags = _db.Tags
            };
            if (id == null)
            {
                return View(articleVM);
            }
            else
            {
                articleVM.Article = _db.Articles.Find(id);

                IEnumerable<ArticleTag> articleTag = _db.ArticleTags.Where(i => i.ArticleId == id).Include(c => c.Tag);

                articleVM.ArticleTagLine = "";
                foreach (var item in articleTag)
                {
                    articleVM.ArticleTagLine += item.Tag.Name + " ";
                }
                articleVM.ArticleTagLine = articleVM.ArticleTagLine.Trim();
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
                if (articleVM.Article.Id == 0)
                {
                    articleVM.Article.CreateDate = DateTime.UtcNow;
                    if (articleVM.Article.UserId == null)
                    {
                        articleVM.Article.UserId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
                    }
                    _db.Articles.Add(articleVM.Article);
                }
                else
                {
                    var product = _db.Articles.AsNoTracking().FirstOrDefault(u => u.Id == articleVM.Article.Id);
                    _db.Articles.Update(articleVM.Article);
                }

                //Добавляем новые теги
                if (articleVM.ArticleTagLine != null)
                {
                    var allTags = from u in _db.Tags select u.Name;

                    var inputTags = articleVM.ArticleTagLine.Split(" ").ToArray();

                    var newTags = inputTags.Except(allTags).ToList();

                    var oldTags = inputTags.Intersect(allTags).ToList();

                    foreach (var obj in newTags)
                    {
                        if (obj == "")
                        {
                            continue;
                        }
                        Tag tag = new Tag()
                        {
                            Name = obj
                        };
                        _db.Tags.Add(tag);
                        ArticleTag articleTag = new ArticleTag { Article = articleVM.Article, Tag = tag };
                        _db.ArticleTags.Add(articleTag);
                    }

                    foreach (var obj in oldTags)
                    {
                        Tag tag = _db.Tags.FirstOrDefault(u => u.Name == obj);
                        ArticleTag articleTag = new ArticleTag { Article = articleVM.Article, Tag = tag };
                        _db.ArticleTags.Add(articleTag);
                    }

                    _db.SaveChanges();
                }
                _db.SaveChanges();

                return Redirect($"~/Home/Details/{articleVM.Article.Id}");
            }
            return View(articleVM);

        }

        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
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
            return Redirect("~/Home/Index");

        }
    }
}
