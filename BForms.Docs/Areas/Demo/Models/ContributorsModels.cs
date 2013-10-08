using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using BForms.Docs.Areas.Demo.Mock;
using BForms.Docs.Resources;
using BootstrapForms.Models;
using BootstrapForms.Mvc;

namespace BForms.Docs.Areas.Demo.Models
{
    public class UsersViewModel
    {
        [BsGrid(HasDetails = true)]
        [Display(Name = "Users", ResourceType = typeof(Resource))]
        public BsGridModel<UsersGridRowModel> Grid { get; set; }

        [Display(Name = "Users", ResourceType = typeof(Resource))]
        public BsToolbarModel<UsersSearchModel, UsersNewModel> Toolbar { get; set; }
    }

    public class UsersGridRowModel
    {
        public int Id { get; set; }

        [BsGridColumn(Width = 5)]
        public string Name { get; set; }

        [BsGridColumn(Width = 4)]
        public DateTime RegisterDate { get; set; }

        [BsGridColumn(Width = 3, IsEditable = false)]
        public bool Enabled { get; set; }

        public Dictionary<string, object> RowData()
        {
            return new Dictionary<string, object> 
            {
                { "data-objid", this.Id },
                { "data-active", this.Enabled }
            };
        }
    }

    public class UsersSearchModel
    {
        public UsersSearchModel()
        {
            IsEnabled = new BsSelectList<YesNoValueTypes?>();
            IsEnabled.ItemsFromEnum(typeof(YesNoValueTypes));
            IsEnabled.SelectedValues = YesNoValueTypes.Both;
        }

        [Display(Name = "Name", ResourceType = typeof(Resource))]
        [BsControl(BsControlType.TextBox)]
        public string Name { get; set; }
        
        [Display(Name = "ChooseInterval", ResourceType = typeof(Resource))]
        [BsControl(BsControlType.DatePickerRange)]
        public BsRange<DateTime?> RegisterDate { get; set; }

        [BsControl(BsControlType.RadioButtonList)]
        [Display(Name = "IsEnabled", ResourceType = typeof(Resource))]
        public BsSelectList<YesNoValueTypes?> IsEnabled { get; set; }

        [Display(Name = "Job", ResourceType = typeof(Resource), Prompt = "Choose")]
        [BsControl(BsControlType.DropDownList)]
        public BsSelectList<int?> Jobs { get; set; }
    }

    public class UsersNewModel
    {
        public UsersNewModel()
        {
            IsEnabled = new BsSelectList<YesNoValueTypes?>();
            IsEnabled.ItemsFromEnum(typeof(YesNoValueTypes), YesNoValueTypes.Both);
            IsEnabled.SelectedValues = YesNoValueTypes.Yes;
        }

        [Required]
        [Display(Name = "FirstName", ResourceType = typeof(Resource))]
        [BsControl(BsControlType.TextBox)]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "LastName", ResourceType = typeof(Resource))]
        [BsControl(BsControlType.TextBox)]
        public string LastName { get; set; }

        [Required]
        [BsControl(BsControlType.RadioButtonList)]
        [Display(Name = "IsEnabled", ResourceType = typeof(Resource))]
        public BsSelectList<YesNoValueTypes?> IsEnabled { get; set; }

        [Display(Name = "Job", ResourceType = typeof(Resource), Prompt = "Choose")]
        [BsControl(BsControlType.DropDownList)]
        public BsSelectList<int?> Jobs { get; set; }
    }

    public class UsersDetailsModel
    {
        public int Id { get; set; }
        public int? IdJob { get; set; }
        public string Job { get; set; }
        public bool Enabled { get; set; }

        [Display(Name = "Job", ResourceType = typeof(Resource), Prompt = "Choose")]
        [BsControl(BsControlType.DropDownList)]
        public BsSelectList<int?> Jobs { get; set; }
    }

    public class ContributorModel
    {
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(Resource))]
        [Display(Name = "FirstName", ResourceType = typeof(Resource))]
        [BsControl(BsControlType.TextBox)]
        public string FirstName { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(Resource))]
        [Display(Name = "LastName", ResourceType = typeof(Resource))]
        [BsControl(BsControlType.TextBox)]
        public string LastName { get; set; }

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

    public enum ProjectRole
    {
        [Display(Name = "Team leader")]
        TeamLeader = 1,
        [Display(Name = "Developer")]
        Developer = 2,
        [Display(Name = "Tester")]
        Tester = 3
    }
}