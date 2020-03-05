using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MVC_test_app.Data;
using MVC_test_app.Models;

namespace MVC_test_app.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class ManageUsersController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        public ManageUsersController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }
        public async Task<IActionResult> Index()
        {
            var admins = (await _userManager
                .GetUsersInRoleAsync(Constants.AdministratorRole))
                .ToArray();
            var everyone = await _userManager.Users
                .ToArrayAsync();

            var model = new ManageUsersViewModel
            {
                Adminisatrators = admins,
                Everyone = everyone
            };
            return View(model);
        }
    }
}