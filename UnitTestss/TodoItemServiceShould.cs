using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.InMemory;
using MVC_test_app.Data;
using MVC_test_app.Models;
using MVC_test_app.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace UnitTestss
{
    public class TodoItemServiceShould
    {
        [Fact]
        public async Task AddNewItemAsIncompleteWithDueDate()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "Test_addNewItem")
                .Options;

            //-Set up a context (connection to the DB) fro writing
            using(var context = new ApplicationDbContext(options))
            {
                var service = new TodoItemService(context);

                var fakeUser = new ApplicationUser
                {
                    Id = "fake-000",
                    UserName = "fake@example.com"
                };

                await service.AddItemAsync(new TodoItem
                {
                    Title = "Testing?"
                }, fakeUser);
            }

            //Use a separate context to read data back from the DB
            using(var context = new ApplicationDbContext(options))
            {
                var itemsInDatabase = await context
                        .Items.CountAsync();
                Assert.Equal(1, itemsInDatabase);

                var item = await context.Items.FirstOrDefaultAsync();
                Assert.Equal("Testing?", item.Title);
                Assert.False(item.IsDone);

                //Item should be due 1 day from now

                var difference = DateTimeOffset.Now.AddDays(1) - item.DueAt;
                Assert.True(difference < TimeSpan.FromSeconds(1));
            }
        }
    }
}
