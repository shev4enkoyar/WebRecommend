using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using WebRecommend.Data;
using WebRecommend.Models;
using WebRecommend.Models.ViewModels;

namespace WebRecommend.Controllers
{
    public class ProfileController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<AppUser> _userManager;

        public ProfileController(ApplicationDbContext db, UserManager<AppUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        public IActionResult Index(string id)
        {
            ProfileVM profileVM = GetProfileVM(id);
            return View(profileVM);
        }

        private ProfileVM GetProfileVM(string userId)
        {
            ProfileVM profileVM = new()
            {
                Article = _db.Articles.Include(u => u.Category).Include(u => u.User).Where(u => u.User.Id == userId),
                CurrUser = _userManager.GetUserAsync(User).Result,
                AppUser = _userManager.FindByIdAsync(userId).Result
            };
            return profileVM;
        }
    }
}