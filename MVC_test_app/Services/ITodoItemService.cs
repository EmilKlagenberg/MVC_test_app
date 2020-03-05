using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MVC_test_app.Data;
using MVC_test_app.Models;

namespace MVC_test_app.Services
{
    public interface ITodoItemService
    {
        Task<TodoItem[]> GetIncompleteItemAsync(ApplicationUser user);

        Task<bool> AddItemAsync(TodoItem newItem, ApplicationUser user);

        Task<bool> MarkDoneAsync(Guid id, ApplicationUser user);
    }
}
