using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebRecommend.Data;
using WebRecommend.Models;
using WebRecommend.Models.ViewModels;

namespace WebRecommend.Controllers
{
    [Authorize(Roles = "admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<AppUser> _userManager;

        public AdminController(ApplicationDbContext db, RoleManager<IdentityRole> roleManager, UserManager<AppUser> userManager)
        {
            _db = db;
            _roleManager = roleManager;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Categories()
        {
            IEnumerable<Category> category = _db.Categories;
            return View(category);
        }

        public IActionResult CategoryCreate()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CategoryCreate(Category category)
        {
            if (ModelState.IsValid)
            {
                _db.Categories.Add(category);
                _db.SaveChanges();
                return RedirectToAction("Categories");
            }
            return View(category);
        }

        public IActionResult CategoryEdit(int? id)
        {
            if (id == null || id == 0)
                return NotFound();
            Category category = _db.Categories.Find(id);
            if (category == null)
                return NotFound();
            return View(category);
        }

        [HttpPost]
        public IActionResult CategoryEdit(Category category)
        {
            if (ModelState.IsValid)
            {
                _db.Categories.Update(category);
                _db.SaveChanges();
                return RedirectToAction("Categories");
            }
            return View(category);
        }

        public IActionResult CategoryDelete(int? id)
        {
            if (id == null || id == 0)
                return NotFound();
            Category category = _db.Categories.Find(id);
            if (category == null)
                return NotFound();
            return View(category);
        }

        [HttpPost]
        public IActionResult CategoryDeletePost(int? id)
        {
            Category category = _db.Categories.Find(id);
            if (category == null)
                return NotFound();
            _db.Categories.Remove(category);
            _db.SaveChanges();
            return RedirectToAction("Categories");
        }

        public IActionResult Users()
        {
            IEnumerable<AppUser> appUsers = _db.ApplicationUsers;
            return View(appUsers);
        }

        public async Task<IActionResult> UsersEditRole(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                var userRoles = await _userManager.GetRolesAsync(user);
                var allRoles = _roleManager.Roles.ToList();
                ChangeRoleVM model = GetChangeRoleVM(user, userRoles, allRoles);
                return View(model);
            }
            return NotFound();
        }

        private static ChangeRoleVM GetChangeRoleVM(AppUser user, IList<string> userRoles, List<IdentityRole> allRoles)
        {
            ChangeRoleVM model = new()
            {
                UserId = user.Id,
                UserEmail = user.Email,
                UserRoles = userRoles,
                AllRoles = allRoles
            };
            return model;
        }

        [HttpPost]
        public async Task<IActionResult> UsersEditRole(string userId, List<string> roles)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                var userRoles = await _userManager.GetRolesAsync(user);
                var addedRoles = roles.Except(userRoles);
                var removedRoles = userRoles.Except(roles);
                await _userManager.AddToRolesAsync(user, addedRoles);
                await _userManager.RemoveFromRolesAsync(user, removedRoles);
                return RedirectToAction("Users");
            }
            return NotFound();
        }

        public async Task<IActionResult> UsersDelete(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user.LockoutEnabled)
            {
                user.LockoutEnabled = false;
                user.LockoutEnd = null;
            }
            else
            {
                user.LockoutEnabled = true;
                user.LockoutEnd = DateTime.Now.AddYears(100);
            }
            await _userManager.UpdateAsync(user);
            return RedirectToAction("Users");
        }
    }
}
