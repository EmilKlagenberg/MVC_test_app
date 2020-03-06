using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MVC_test_app.Models
{
    public class TodoViewModel
    {
        public TodoItem[] incompleteItems { get; set; }

        public TodoItem[] completeItems { get; set; }
    }
}
