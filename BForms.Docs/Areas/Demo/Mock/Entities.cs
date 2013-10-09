using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using BForms.Docs.Areas.Demo.Models;

namespace BForms.Docs.Areas.Demo.Mock
{
    #region Context
    [Serializable]
    public class BFormsContext
    {
        public BFormsContext()
        {
            #region Contributors
            Contributors = new List<Contributor>()
                    {
                        new Contributor()
                        {
                            Id = 1,
                            Enabled = true,
                            FirstName = "Stefan",
                            LastName = "P.",
                            Country = "Romania",
                            Role = ProjectRole.TeamLeader,
                            StartDate = new DateTime(2013, 8, 1),
                            Url = "http://www.stefanprodan.eu/",
                            Contributions = "concept, api, razor helpers, documentation, c# bug fixing, testing"
                        },
                        new Contributor()
                        {
                            Id = 2,
                            Enabled = true,
                            FirstName = "Ciprian",
                            LastName = "P.",
                            Country = "Romania",
                            Role = ProjectRole.Developer,
                            StartDate = new DateTime(2013, 9, 1),
                            Contributions = "grid component, razor helpers, c# & js bug fixing"
                        },
                        
                        new Contributor()
                        {
                            Id = 3,
                            Enabled = true,
                            FirstName = "Cezar",
                            LastName = "C.",
                            Country = "Romania",
                            Role = ProjectRole.Developer,
                            Languages = new List<string>() { "C#", "Javascript" },
                            StartDate = new DateTime(2013, 8, 15),
                            Contributions = "documentation, razor helpers"
                        },
                        new Contributor()
                        {
                            Id = 4,
                            Enabled = true,
                            FirstName = "Marius",
                            LastName = "C.",
                            Country = "Romania",
                            Role = ProjectRole.Developer,
                            StartDate = new DateTime(2013, 8, 10),
                            Contributions = "bforms js framework, datetime picker, automated tests for js"
                        },
                        new Contributor()
                        {
                            Id = 5,
                            Enabled = true,
                            FirstName = "Cristian",
                            LastName = "P.",
                            Country = "Romania",
                            Role = ProjectRole.Developer,
                            StartDate = new DateTime(2013, 10, 1),
                            Languages = new List<string>() { "C#", "Javascript" },
                            Contributions = "grid component, c# & js bug fixing"
                        },
                        new Contributor()
                        {
                            Id = 6,
                            Enabled = true,
                            FirstName = "Oana",
                            LastName = "M.",
                            Country = "Romania",
                            Role = ProjectRole.Developer,
                            StartDate = new DateTime(2013, 8, 5),
                            Languages = new List<string>() { "CSS", "HTML", "SASS" },
                            Contributions = "UI & UX, css master"
                        },
                    };
            #endregion
        }

        public List<Contributor> Contributors { get; set; }

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

    #region Contributor
    public class Contributor
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public bool Enabled { get; set; }
        public string Url { get; set; }
        public DateTime StartDate { get; set; }
        public string Country { get; set; }
        public ProjectRole Role { get; set; }
        public List<string> Languages { get; set; }
        public string Contributions { get; set; }
    }
    #endregion
}