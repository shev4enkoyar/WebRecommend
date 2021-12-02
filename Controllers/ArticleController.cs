using Dropbox.Api;
using Dropbox.Api.Files;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebRecommend.Data;
using WebRecommend.Models;
using WebRecommend.Models.ViewModels;

namespace WebRecommend.Controllers
{
    [Authorize(Roles = "admin,user")]
    public class ArticleController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<AppUser> _userManager;
        public IConfiguration Configuration { get; }

        public ArticleController(ApplicationDbContext db, IConfiguration configuration, UserManager<AppUser> userManager)
        {
            _db = db;
            Configuration = configuration;
            _userManager = userManager;
        }

        [Authorize(Roles = "admin")]
        public IActionResult Index()
        {
            IEnumerable<Article> articles = _db.Articles.Include(u => u.Category).Include(u => u.User);
            return View(articles);
        }

        public IActionResult Change(int? id)
        {
            if (!isConfirmedEmail())
            {
                return NotFound();
            }
            ArticleVM articleVM = GetArticleVM();
            if (id == null)
            {
                return View(articleVM);
            }
            else
            {
                FillArticleVM(articleVM, id);
                if (articleVM.Article == null)
                {
                    return NotFound();
                }
                if (articleVM.Article.UserId == _userManager.GetUserId(User) || User.IsInRole("admin"))
                {
                    return View(articleVM);
                }
                else
                {
                    return NotFound();
                }

            }
        }

        private ArticleVM GetArticleVM()
        {
            ArticleVM articleVM = new()
            {
                Tags = _db.Tags.ToList(),
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
            };
            return articleVM;
        }

        private void FillArticleVM(ArticleVM articleVM, int? id)
        {
            articleVM.Article = _db.Articles.Find(id);
            IEnumerable<ArticleTag> articleTag = _db.ArticleTags.Where(i => i.ArticleId == id).Include(c => c.Tag);
            articleVM.ArticleTagLine = "";
            foreach (var item in articleTag)
            {
                articleVM.ArticleTagLine += item.Tag.Name + " ";
            }
            articleVM.ArticleTagLine = articleVM.ArticleTagLine.Trim();
        }

        [HttpPost]
        public IActionResult Change(ArticleVM articleVM)
        {
            if (ModelState.IsValid)
            {
                if (articleVM.Article.Id == 0)
                {
                    AddArticle(articleVM);
                }
                else
                {
                    UpdateArticle(articleVM);
                }

                ArticleTagChanged(articleVM);
                _db.SaveChanges();

                return Redirect($"~/Home/Details/{articleVM.Article.Id}");
            }
            return View(articleVM);

        }

        private void AddArticle(ArticleVM articleVM)
        {
            articleVM.Article.CreateDate = DateTime.UtcNow;
            if (articleVM.Article.UserId == null)
            {
                articleVM.Article.UserId = _userManager.GetUserId(User);
            }
            _db.Articles.Add(articleVM.Article);
        }

        private void UpdateArticle(ArticleVM articleVM)
        {
            var product = _db.Articles.AsNoTracking().FirstOrDefault(u => u.Id == articleVM.Article.Id);
            _db.Articles.Update(articleVM.Article);
        }

        private void ArticleTagChanged(ArticleVM articleVM)
        {
            if (articleVM.ArticleTagLine != null)
            {
                DeleteOldTags(articleVM);
                var inputTags = articleVM.ArticleTagLine.Split(" ").ToArray();
                var allTags = from u in _db.Tags select u.Name;
                var newTags = inputTags.Except(allTags).ToList();
                AddNewTags(newTags, articleVM);
                var oldTags = inputTags.Intersect(allTags).ToList();
                AddOldTags(oldTags, articleVM);
                _db.SaveChanges();
            }
            else
            {
                DeleteOldTags(articleVM);
            }
        }

        private void DeleteOldTags(ArticleVM articleVM)
        {
            if (articleVM.Article.Id != 0)
            {
                var articleTag = _db.ArticleTags.Where(o => o.ArticleId == articleVM.Article.Id);
                _db.ArticleTags.RemoveRange(articleTag);
                _db.SaveChanges();
            }
        }

        private void AddNewTags(List<string> newTags, ArticleVM articleVM)
        {
            foreach (var obj in newTags)
            {
                if (obj == "")
                {
                    continue;
                }
                Models.Tag tag = new()
                {
                    Name = obj
                };
                _db.Tags.Add(tag);
                ArticleTag articleTag = new() { Article = articleVM.Article, Tag = tag };
                _db.ArticleTags.Add(articleTag);
            }
        }

        private void AddOldTags(List<string> oldTags, ArticleVM articleVM)
        {
            foreach (var obj in oldTags)
            {
                Models.Tag tag = _db.Tags.FirstOrDefault(u => u.Name == obj);
                ArticleTag articleTag = new() { Article = articleVM.Article, Tag = tag };
                _db.ArticleTags.Add(articleTag);
            }
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

            if (product.UserId == _userManager.GetUserId(User) || User.IsInRole("admin"))
            {
                return View(product);
            }

            return NotFound();
        }

        [HttpPost, ActionName("Delete")]
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



        [HttpPost]
        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            try
            {
                if (file != null && file.Length > 0)
                {
                    var relativePath = await UploadFileDropbox(file);
                    return Ok(new { fileUrl = relativePath });
                }
                return BadRequest("Select a file");
            }
            catch (Exception exception)
            {
                return BadRequest(exception.Message);
            }
        }

        private async Task<string> UploadFileDropbox(IFormFile file)
        {
            using (var dropbox = new DropboxClient(Configuration["Dropbox:ClientSecret"]))
            {
                var folder = "/Article/Body";
                var date = DateTime.UtcNow.ToString("MMddyyyy_HHmmssFFF");
                var filename = $"{ date }_{file.FileName}";
                using (var memoryStream = file.OpenReadStream())
                {
                    var updated = await dropbox.Files.UploadAsync(folder + "/" + filename, WriteMode.Overwrite.Instance, body: memoryStream);
                    var sharLink = await dropbox.Sharing.CreateSharedLinkWithSettingsAsync(folder + "/" + filename);
                    return sharLink.Url.ToString().TrimEnd('0') + "1";
                }
            }
        }

        private bool isConfirmedEmail()
        {
            return _userManager.GetUserAsync(User).Result.EmailConfirmed;
        }
    }
}
