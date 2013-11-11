using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BForms.Docs.Areas.Demo.Models
{
    public class UserProfileModel
    {
        public int Id { get; set; }

        public string Username { get; set; }

        public string Firstname { get; set; }

        public string Lastname { get; set; }

        public string Department { get; set; }

        public string Password { get; set; }

        public string Organization { get; set; }

        public string Mail { get; set; }

        public string Phone { get; set; }

        public DateTime HireDate { get; set; }
    }

}