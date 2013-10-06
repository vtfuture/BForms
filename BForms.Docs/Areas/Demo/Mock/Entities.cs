using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;

namespace BForms.Docs.Areas.Demo.Mock
{
    public class BFormsContext
    {
        public List<User> Users
        {
            get
            {
                return new List<User>()
                           {
                               new User()
                                   {
                                       Id = 1,
                                       FirstName = "John",
                                       LastName = "Doe",
                                       Enabled = true,
                                       RegisterDate = new DateTime(2013, 12, 4)
                                   },
                                new User()
                                   {
                                       Id = 2,
                                       FirstName = "George",
                                       LastName = "Clooney",
                                       Enabled = true,
                                       RegisterDate = new DateTime(2013, 6, 14)
                                   },
                                new User()
                                   {
                                       Id = 3,
                                       FirstName = "Sandra",
                                       LastName = "Bullock",
                                       Enabled = true,
                                       RegisterDate = new DateTime(2013, 2, 24)
                                   },
                                   new User()
                                   {
                                       Id = 4,
                                       FirstName = "Emma",
                                       LastName = "Watson",
                                       Enabled = true,
                                       RegisterDate = new DateTime(2013, 2, 8)
                                   },
                                new User()
                                   {
                                       Id = 5,
                                       FirstName = "Jamie",
                                       LastName = "Chung",
                                       Enabled = true,
                                       RegisterDate = new DateTime(2013, 4, 4)
                                   },
                                new User()
                                   {
                                       Id = 6,
                                       FirstName = "Jeremy",
                                       LastName = "Sisto",
                                       Enabled = false,
                                       RegisterDate = new DateTime(2013, 6, 14)
                                   },
                                   new User()
                                   {
                                       Id = 7,
                                       FirstName = "Ioan",
                                       LastName = "Gruffudd",
                                       Enabled = true,
                                       RegisterDate = new DateTime(2013, 6, 14)
                                   },
                                new User()
                                   {
                                       Id = 8,
                                       FirstName = "Olivia",
                                       LastName = "Thirlby",
                                       Enabled = true,
                                       RegisterDate = new DateTime(2013, 2, 4)
                                   },
                                new User()
                                   {
                                       Id = 3,
                                       FirstName = "Elisabeth",
                                       LastName = "Shue",
                                       Enabled = false,
                                       RegisterDate = new DateTime(2011, 5, 27)
                                   },
                                   new User()
                                   {
                                       Id = 1,
                                       FirstName = "Rachel",
                                       LastName = "McAdams",
                                       Enabled = false,
                                       RegisterDate = new DateTime(2013, 5, 13)
                                   },
                                new User()
                                   {
                                       Id = 2,
                                       FirstName = "Jennifer",
                                       LastName = "Lawrence",
                                       Enabled = true,
                                       RegisterDate = new DateTime(2012, 9, 14)
                                   },
                                new User()
                                   {
                                       Id = 3,
                                       FirstName = "Christian",
                                       LastName = "Bale",
                                       Enabled = true,
                                       RegisterDate = new DateTime(2012, 9, 22)
                                   }

                           };
            }
        }
    }

    public class User
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public bool Enabled { get; set; }
        public DateTime RegisterDate { get; set; }
    }
}