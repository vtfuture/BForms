using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using BForms.Docs.Areas.Demo.Mock;
using BForms.Docs.Resources;
using BForms.Models;
using BForms.Mvc;

namespace BForms.Docs.Areas.Demo.Models
{
    public class ContributorsViewModel
    {
        [BsGrid(HasDetails = true)]
        [Display(Name = "Contributors", ResourceType = typeof(Resource))]
        public BsGridModel<ContributorRowModel> Grid { get; set; }

        [Display(Name = "Contributors", ResourceType = typeof(Resource))]
        public BsToolbarModel<ContributorSearchModel, ContributorNewModel> Toolbar { get; set; }
    }

    public class ContributorDetailsModel : ContributorNewModel
    {
        public ContributorDetailsModel()
            : base() {}

        public int Id { get; set; }
        public bool Enabled { get; set; }

        public string Country { get; set; }

        public ProjectRole Role { get; set; }

        public List<string> Languages { get; set; }

        public DateTime ContributorSince { get; set; }
    }

    public class ContributorModel
    {
        public ContributorModel()
        {
            RoleList = new BsSelectList<ProjectRole?>();
            RoleList.ItemsFromEnum(typeof(ProjectRole));
        }

        [Display(Name = "Web address", Prompt = "http://mysite.com or http://twitter.com/id")]
        [BsControl(BsControlType.Url)]
        public string Url { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(Resource))]
        [Display(Name = "Contributor since")]
        [BsControl(BsControlType.DatePicker)]
        public BsDateTime StartDate { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(Resource))]
        [Display(Name = "Location", Prompt = "Choose your country")]
        [BsControl(BsControlType.DropDownList)]
        public BsSelectList<string> CountriesList { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(Resource))]
        [Display(Name = "Role", Description = "Your main role in project")]
        [BsControl(BsControlType.RadioButtonList)]
        public BsSelectList<ProjectRole?> RoleList { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(Resource))]
        [Display(Name = "Programming languages", Prompt = "Type your favorite programming languages")]
        [BsControl(BsControlType.TagList)]
        public BsSelectList<List<string>> LanguagesList { get; set; }

        [Display(Name = "Contributions")]
        [BsControl(BsControlType.TextArea)]
        public string Contributions { get; set; }
    }

    public class ContributorSearchModel : ContributorModel
    {
        public ContributorSearchModel()
        {
            IsEnabled = new BsSelectList<YesNoValueTypes?>();
            IsEnabled.ItemsFromEnum(typeof(YesNoValueTypes));
            IsEnabled.SelectedValues = YesNoValueTypes.Both;

            RoleList = new BsSelectList<ProjectRole?>();
            RoleList.ItemsFromEnum(typeof(ProjectRole));
        }

        [Display(Name = "ChooseInterval", ResourceType = typeof(Resource))]
        [BsControl(BsControlType.DatePickerRange)]
        public BsRange<DateTime?> StartDateRange { get; set; }

        [Display(Name = "Name", ResourceType = typeof(Resource))]
        [BsControl(BsControlType.TextBox)]
        public string Name { get; set; }

        [BsControl(BsControlType.RadioButtonList)]
        [Display(Name = "IsEnabled", ResourceType = typeof(Resource))]
        public BsSelectList<YesNoValueTypes?> IsEnabled { get; set; }

        [Display(Name = "Location", Prompt = "Choose country")]
        [BsControl(BsControlType.DropDownList)]
        public new BsSelectList<string> CountriesList { get; set; }

        [Display(Name = "Role", Description = "Role in project")]
        [BsControl(BsControlType.RadioButtonList)]
        public new BsSelectList<ProjectRole?> RoleList { get; set; }

        [Display(Name = "Programming languages", Prompt = "Type programming languages")]
        [BsControl(BsControlType.TagList)]
        public new BsSelectList<List<string>> LanguagesList { get; set; }
    }

    public class ContributorNewModel : ContributorModel
    {
        public ContributorNewModel() : base()
        {
            IsEnabled = new BsSelectList<YesNoValueTypes?>();
            IsEnabled.ItemsFromEnum(typeof(YesNoValueTypes), YesNoValueTypes.Both);
            IsEnabled.SelectedValues = YesNoValueTypes.Yes;
        }

        [Required]
        [BsControl(BsControlType.RadioButtonList)]
        [Display(Name = "IsEnabled", ResourceType = typeof(Resource))]
        public BsSelectList<YesNoValueTypes?> IsEnabled { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(Resource))]
        [Display(Name = "FirstName", ResourceType = typeof(Resource))]
        [BsControl(BsControlType.TextBox)]
        public string FirstName { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(Resource))]
        [Display(Name = "LastName", ResourceType = typeof(Resource))]
        [BsControl(BsControlType.TextBox)]
        public string LastName { get; set; }
    }

    public class ContributorRowModel
    {
        public int Id { get; set; }

        public bool Enabled { get; set; }

        [BsGridColumn(Width = 3)]
        public string Name { get; set; }

        [BsGridColumn(Width = 3)]
        public DateTime StartDate { get; set; }

        [BsGridColumn(Width = 3)]
        public ProjectRole Role { get; set; }

        [BsGridColumn(Width = 3)]
        public string Contributions { get; set; }

        public Dictionary<string, object> RowData()
        {
            return new Dictionary<string, object> 
            {
                { "data-objid", Id },
                { "data-active", Enabled }
            };
        }
    }

    public enum ProjectRole
    {
        [Display(Name = "Team leader")]
        TeamLeader = 1,
        [Display(Name = "Developer")]
        Developer = 2,
        [Display(Name = "Tester")]
        Tester = 3
    }

    public enum EditComponents
    {
        Identity = 1,
        ProjectRelated = 2
    }
}