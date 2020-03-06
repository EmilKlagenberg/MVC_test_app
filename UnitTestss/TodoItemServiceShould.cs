using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.InMemory;
using MVC_test_app.Data;
using MVC_test_app.Models;
using MVC_test_app.Services;
using System;
using System.Collections.Generic;
using System.Linq;
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
            //Arrange
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "Test_addNewItem")
                .Options;

            //-Set up a context (connection to the DB) fro writing
            using (var context = new ApplicationDbContext(options))
            {
                var service = new TodoItemService(context);

                var fakeUser = new ApplicationUser
                {
                    Id = "fake-000",
                    UserName = "fake@example.com"
                };

                //Act
                await service.AddItemAsync(new TodoItem
                {
                    Title = "New item"
                }, fakeUser);
            }

            //Assert
            //Use a separate context to read data back from the DB
            using (var context = new ApplicationDbContext(options))
            {
                var itemsInDatabase = await context
                        .Items.CountAsync();
                Assert.Equal(1, itemsInDatabase);

                var item = await context.Items.FirstOrDefaultAsync();
                Assert.Equal("New item", item.Title);
                Assert.False(item.IsDone);

                //Item should be due 1 day from now

                var difference = DateTimeOffset.Now.AddDays(1) - item.DueAt;
                Assert.True(difference < TimeSpan.FromSeconds(1));
            }
        }

        [Fact]
        public async Task MarkItemAsDoneBasedOnId()
        {
            //Arrange
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "Test_markItemDone")
                .Options;

            //-Set up a context (connection to the DB) for writing
            using (var context = new ApplicationDbContext(options))
            {
                var service = new TodoItemService(context);

                var fakeUser = new ApplicationUser
                {
                    Id = "fake-001",
                    UserName = "fake@example.com"
                };


                //Act
                await service.AddItemAsync(new TodoItem
                {
                    Title = "To done"
                }, fakeUser);



            }

            //Assert
            //Use a separate context to read data back from the DB
            using (var context = new ApplicationDbContext(options))
            {
                var service = new TodoItemService(context);
                var fakeUser = new ApplicationUser
                {
                    Id = "fake-001",
                    UserName = "fake@example.com"
                };


                var itemsInDatabase = await context.Items.CountAsync();
                Assert.Equal(1, itemsInDatabase);

                var item = await context.Items.FirstOrDefaultAsync();
                Assert.True(await service.MarkDoneAsync(item.Id, fakeUser));
                Assert.False(await service.MarkDoneAsync(new Guid(), fakeUser));
            }
        }

        [Fact]
        public async Task GetIncompleteItemsOnlyFromSpecifiedUser()
        {
            //Arrange
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "Test_getIncompleteItemsForUser")
                .Options;

            //-Set up a context (connection to the DB) for writing
            using (var context = new ApplicationDbContext(options))
            {
                var service = new TodoItemService(context);

                var fakeUser = new ApplicationUser
                {
                    Id = "fake-002",
                    UserName = "fake@example.com"
                };

                var wrongFakeUser = new ApplicationUser
                {
                    Id = "fake-003",
                    UserName = "fake@example.com"
                };


                await service.AddItemAsync(new TodoItem
                {
                    Title = "Incomplete 1"
                }, fakeUser);

                await service.AddItemAsync(new TodoItem
                {
                    Title = "Incomplete 2"
                }, fakeUser);

                await service.AddItemAsync(new TodoItem
                {
                    Title = "Incomplete 3"
                }, fakeUser);

                //Item for wrong user                               
                await service.AddItemAsync(new TodoItem
                {
                    Title = "Incomplete 4"
                }, wrongFakeUser);

                await service.AddItemAsync(new TodoItem
                {
                    Title = "Incomplete 5"
                }, wrongFakeUser);
            }

            //Act
            //Use a separate context to read data back from the DB
            using (var context = new ApplicationDbContext(options))
            {
                var service = new TodoItemService(context);
                var fakeUser = new ApplicationUser
                {
                    Id = "fake-002",
                    UserName = "fake@example.com"
                };
                var wrongFakeUser = new ApplicationUser
                {
                    Id = "fake-003",
                    UserName = "fake@example.com"
                };
                //Assert
                var userItems = await service.GetIncompleteItemAsync(fakeUser);
                Assert.Equal(3, userItems.Length);
                Assert.Single(userItems.Select(x => x.UserId).Distinct());
                foreach (var item in userItems)
                {
                    Assert.False(item.IsDone);
                }
            }
        }

        [Fact]
        public async Task GetCompleteItemsOnlyFromSpecifiedUser()
        {
            //Arrange
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "Test_getCompleteItemsForUser")
                .Options;

            //-Set up a context (connection to the DB) for writing
            using (var context = new ApplicationDbContext(options))
            {
                var service = new TodoItemService(context);

                var fakeUser = new ApplicationUser
                {
                    Id = "fake-004",
                    UserName = "fake@example.com"
                };

                var wrongFakeUser = new ApplicationUser
                {
                    Id = "fake-005",
                    UserName = "fake@example.com"
                };


                await service.AddItemAsync(new TodoItem
                {
                    Title = "Complete 1"
                }, fakeUser);

                await service.AddItemAsync(new TodoItem
                {
                    Title = "Complete 2"
                }, fakeUser);

                await service.AddItemAsync(new TodoItem
                {
                    Title = "Complete 3"
                }, fakeUser);

                //Item for wrong user                               
                await service.AddItemAsync(new TodoItem
                {
                    Title = "Complete 4"
                }, wrongFakeUser);

                await service.AddItemAsync(new TodoItem
                {
                    Title = "Complete 5"
                }, wrongFakeUser);


                var userItems = await service.GetIncompleteItemAsync(fakeUser);
                foreach (var item in userItems)
                {
                    await service.MarkDoneAsync(item.Id, fakeUser);
                }

            }

            //Act
            //Use a separate context to read data back from the DB
            using (var context = new ApplicationDbContext(options))
            {
                var service = new TodoItemService(context);
                var fakeUser = new ApplicationUser
                {
                    Id = "fake-004",
                    UserName = "fake@example.com"
                };
                var wrongFakeUser = new ApplicationUser
                {
                    Id = "fake-005",
                    UserName = "fake@example.com"
                };
                //Assert
                var userItems = await service.GetCompleteItemAsync(fakeUser);
                Assert.Equal(3, userItems.Length);
                Assert.Single(userItems.Select(x => x.UserId).Distinct());
                foreach (var item in userItems)
                {
                    Assert.True(item.IsDone);
                }
            }
        }
    }
}
