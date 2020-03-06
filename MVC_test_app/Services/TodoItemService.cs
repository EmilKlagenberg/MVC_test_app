using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MVC_test_app.Models;
using MVC_test_app.Data;
using Microsoft.EntityFrameworkCore;


namespace MVC_test_app.Services
{
    
    public class TodoItemService : ITodoItemService
    {
        private readonly ApplicationDbContext _context;

        public TodoItemService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<TodoItem[]> GetIncompleteItemAsync(ApplicationUser user)
        {
            return await _context.Items
                .Where(x => x.IsDone == false && x.UserId == user.Id)
                .ToArrayAsync();
        }
        public async Task<TodoItem[]> GetCompleteItemAsync(ApplicationUser user)
        {
            return await _context.Items
                .Where(x => x.IsDone == true && x.UserId == user.Id)
                .ToArrayAsync();
        }


        public async Task<bool> AddItemAsync(TodoItem newItem, ApplicationUser user)
        {
            newItem.UserId = user.Id;
            newItem.Id = Guid.NewGuid();
            newItem.IsDone = false;
            //newItem.DueAt = DateTimeOffset.Now.AddDays(1);

            _context.Items.Add(newItem);

            var saveResult = await _context.SaveChangesAsync();
            return saveResult == 1;
        }

        public async Task<bool> MarkDoneAsync(Guid id, ApplicationUser user)
        {
            var item = await _context.Items
                .Where(x => x.Id == id && x.UserId == user.Id)
                .SingleOrDefaultAsync();

            if (item == null)
            {
                return false;
            }

            item.IsDone = true;

            var saveResult = await _context.SaveChangesAsync();
            return saveResult == 1;
        }
    }
}
