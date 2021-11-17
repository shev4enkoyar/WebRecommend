﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using WebRecommend.Data;
using WebRecommend.Models;
using WebRecommend.Models.ViewModels;

namespace WebRecommend.Controllers
{
    [Authorize]
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

            var currUser = _userManager.GetUserAsync(HttpContext.User).Result;
            ProfileVM profileVM = new ProfileVM()
            {
                Article = _db.Articles
                .Include(u => u.Category)
                .Include(u => u.User)
                .Where(u => u.User.Id == id),
                CurrUser = currUser,
                AppUser = _userManager.FindByIdAsync(id).Result
            };

            return View(profileVM);
        }
    }
}