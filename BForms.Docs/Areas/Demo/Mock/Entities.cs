using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;

namespace BForms.Docs.Areas.Demo.Mock
{
    #region Context
    [Serializable]
    public class BFormsContext
    {
        public BFormsContext()
        {
            #region Users
            Users = new List<User>()
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
                                       RegisterDate = new DateTime(2013, 6, 14),
                                       IdJob = 1
                                   },
                                new User()
                                   {
                                       Id = 3,
                                       FirstName = "Scott",
                                       LastName = "Guthrie",
                                       Enabled = true,
                                       RegisterDate = new DateTime(2013, 2, 24),
                                       IdJob = 3
                                   },
                                   new User()
                                   {
                                       Id = 4,
                                       FirstName = "Emma",
                                       LastName = "Watson",
                                       Enabled = true,
                                       RegisterDate = new DateTime(2013, 2, 8),
                                       IdJob = 1
                                   },
                                new User()
                                   {
                                       Id = 5,
                                       FirstName = "Sheldon",
                                       LastName = "Cooper",
                                       Enabled = true,
                                       RegisterDate = new DateTime(2013, 4, 4),
                                       IdJob = 2
                                   },
                                new User()
                                   {
                                       Id = 6,
                                       FirstName = "Jeremy",
                                       LastName = "Sisto",
                                       Enabled = false,
                                       RegisterDate = new DateTime(2013, 6, 14),
                                       IdJob = 1
                                   },
                                   new User()
                                   {
                                       Id = 7,
                                       FirstName = "Steve",
                                       LastName = "Jobs",
                                       Enabled = true,
                                       RegisterDate = new DateTime(2013, 6, 14)
                                   },
                                new User()
                                   {
                                       Id = 8,
                                       FirstName = "Scott",
                                       LastName = "Hanselman",
                                       Enabled = true,
                                       RegisterDate = new DateTime(2013, 2, 4),
                                       IdJob = 3
                                   },
                                new User()
                                   {
                                       Id = 9,
                                       FirstName = "Elisabeth",
                                       LastName = "Shue",
                                       Enabled = false,
                                       RegisterDate = new DateTime(2011, 5, 27),
                                       IdJob = 1
                                   },
                                   new User()
                                   {
                                       Id = 10,
                                       FirstName = "Rachel",
                                       LastName = "McAdams",
                                       Enabled = false,
                                       RegisterDate = new DateTime(2013, 5, 13),
                                       IdJob = 1
                                   },
                                new User()
                                   {
                                       Id = 11,
                                       FirstName = "Jennifer",
                                       LastName = "Lawrence",
                                       Enabled = true,
                                       RegisterDate = new DateTime(2012, 9, 14),
                                       IdJob = 1
                                   },
                                new User()
                                   {
                                       Id = 12,
                                       FirstName = "Christian",
                                       LastName = "Bale",
                                       Enabled = true,
                                       RegisterDate = new DateTime(2012, 9, 22),
                                       IdJob = 4
                                   }

                           };
            #endregion

            #region Jobs
            Jobs = new List<Job>()
                {
                    new Job()
                    {
                        Id = 1,
                        Name = "Actor"
                    },
                    new Job()
                    {
                        Id = 2,
                        Name = "Scientist"
                    },
                    new Job()
                    {
                        Id = 3,
                        Name = "Programmer"
                    },
                    new Job()
                    {
                        Id = 4,
                        Name = "Batman"
                    }
                };
            #endregion
        }

        public List<User> Users { get; set; }

        public List<Job> Jobs { get; set; }

        public void SaveChanges()
        {
            HttpContext.Current.Session["DbContext"] = this;
        }

        public static BFormsContext Get()
        {
            var sessionContext = (BFormsContext)HttpContext.Current.Session["DbContext"];

            if (sessionContext == null)
            {
                sessionContext = new BFormsContext();

                HttpContext.Current.Session["DbContext"] = sessionContext;
            }

            return sessionContext;
        }
    }
    #endregion

    #region User
    public class User
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public bool Enabled { get; set; }
        public DateTime RegisterDate { get; set; }
        public int? IdJob { get; set; }

        public Job Job
        {
            get
            {
                return IdJob.HasValue ? BFormsContext.Get().Jobs.FirstOrDefault(x => x.Id == IdJob) : null;
            }
        }
    }
    #endregion

    #region Job
    public class Job
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
    #endregion
}