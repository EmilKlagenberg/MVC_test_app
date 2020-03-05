using MVC_test_app.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MVC_test_app.Models
{
    public class ManageUsersViewModel
    {
        public ApplicationUser[] Adminisatrators { get; set; }
        public ApplicationUser[] Everyone { get; set; }
    }
}
