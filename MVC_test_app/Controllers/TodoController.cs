using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MVC_test_app.Data;
using MVC_test_app.Models;
using MVC_test_app.Services;

namespace MVC_test_app.Controllers
{
    [Authorize]
    public class TodoController : Controller
    {
        private readonly ITodoItemService _todoItemService;
        private readonly UserManager<ApplicationUser> _userManager;
        public TodoController(ITodoItemService todoItemService, UserManager<ApplicationUser> userManager)
        {
            _todoItemService = todoItemService;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            //Get current user
            var curentUser = await _userManager.GetUserAsync(User);
            if (curentUser == null) return Challenge();
            //Get To-do items from database
            var items = await _todoItemService
                .GetIncompleteItemAsync(curentUser);
            //Put items into a model
            var model = new TodoViewModel()
            {
                Items = items
            };
            //Render view using the model
            return View(model);
        }

        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddItem(TodoItem newItem)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("Index");
            }
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null) return Challenge();

            var successful = await _todoItemService.AddItemAsync(newItem, currentUser);
            if (!successful)
            {
                return BadRequest("Colud not add item");
            }
            return RedirectToAction("Index");
        }

        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkDone(Guid id)
        {
            if(id == Guid.Empty)
            {
                return RedirectToAction("Index");
            }
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null) return Challenge();

            var successful = await _todoItemService.MarkDoneAsync(id, currentUser);
            if (!successful)
            {
                return BadRequest("Could not mark item as done.");
            }
            return RedirectToAction("Index");
        }
    }
}
