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

        [BsToolbar]
        [Display(Name = "Contributors", ResourceType = typeof(Resource))]
        public BsToolbarModel<ContributorSearchModel, ContributorNewModel, List<ContributorOrderModel>> Toolbar { get; set; }
    }

    #region Readonly
    public class ContributorDetailsReadonly
    {
        [BsPanel(Id = EditComponents.Identity, Expandable = false, Editable = true)]
        [Display(Name = "Identity")]
        public ContributorIdentityModel Identity { get; set; }

        [BsPanel(Id = EditComponents.ProjectRelated, Expandable = false, Editable = true)]
        [Display(Name = "Project Related")]
        public ContributorProjectRelatedModel ProjectRelated { get; set; }

        public int Id { get; set; }
        public bool Enabled { get; set; }
    }

    public class ContributorIdentityModel
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Url { get; set; }

        public string Country { get; set; }

        public int? Age { get; set; }
    }

    public class ContributorProjectRelatedModel
    {
        public ProjectRole Role { get; set; }

        public DateTime ContributorSince { get; set; }

        public List<string> Languages { get; set; }

        public string Contributions { get; set; }
    }
    #endregion

    #region Editable
    public class ContributorDetailsEditable
    {
        public ContributorIdentityEditableModel Identity { get; set; }

        public ContributorProjectEditableRelatedModel ProjectRelated { get; set; }

        public int Id { get; set; }
    }

    public class ContributorIdentityEditableModel
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
        [Display(Name = "Location", Prompt = "Choose your country")]
        [BsControl(BsControlType.DropDownList)]
        public BsSelectList<string> CountriesList { get; set; }

        [Display(Name = "Age")]
        [BsControl(BsControlType.Number)]
        public BsRangeItem<int?> Age { get; set; }
    }

    public class ContributorProjectEditableRelatedModel
    {
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(Resource))]
        [Display(Name = "Contributor since")]
        [BsControl(BsControlType.DatePicker)]
        public BsDateTime StartDate { get; set; }

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

        public ContributorProjectEditableRelatedModel()
        {
            RoleList = new BsSelectList<ProjectRole?>();
            RoleList.ItemsFromEnum(typeof(ProjectRole));
        }
    }
    #endregion

    public class ContributorDetailsModel : ContributorNewModel
    {
        public ContributorDetailsModel()
            : base() { }

        public int Id { get; set; }
        public bool Enabled { get; set; }

        public string Country { get; set; }

        public ProjectRole Role { get; set; }

        public List<string> Languages { get; set; }

        public DateTime ContributorSince { get; set; }
    }

    public class ContributorModel
    {
        [Display(Name = "Web address", Prompt = "http://mysite.com or http://twitter.com/id")]
        [BsControl(BsControlType.Url)]
        public string Url { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(Resource))]
        [Display(Name = "Contributor since")]
        [BsControl(BsControlType.DateTimePicker)]
        public BsDateTime StartDate { get; set; }

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
        [Display(Name = "ChooseInterval", ResourceType = typeof(Resource))]
        [BsControl(BsControlType.DatePickerRange)]
        public BsRange<DateTime?> StartDateRange { get; set; }

        [Display(Name = "ChooseInterval", ResourceType = typeof(Resource))]
        [BsControl(BsControlType.NumberRange)]
        public BsRange<int?> AgeRange { get; set; }

        [Display(Name = "Name", ResourceType = typeof(Resource))]
        [BsControl(BsControlType.TextBox)]
        public string Name { get; set; }

        [BsControl(BsControlType.RadioButtonList)]
        [Display(Name = "IsEnabled", ResourceType = typeof(Resource))]
        public BsSelectList<YesNoValueTypes?> IsEnabled { get; set; }

        [Display(Name = "Location", Prompt = "PromptLocation", ResourceType = typeof(Resource))]
        [BsControl(BsControlType.DropDownList)]
        public new BsSelectList<string> CountriesList { get; set; }

        [Display(Name = "Role", Description = "Role in current project")]
        [BsControl(BsControlType.RadioButtonList)]
        public new BsSelectList<ProjectRole?> RoleList { get; set; }

        [Display(Name = "Programming in", Prompt = "Type languages")]
        [BsControl(BsControlType.TagList)]
        public new BsSelectList<List<string>> LanguagesList { get; set; }

        public IEnumerable<ProjectRole> RolesFilter { get; set; }
    }

    public class ContributorNewModel : ContributorModel
    {
        public ContributorNewModel()
            : base()
        {
            IsEnabled = new BsSelectList<YesNoValueTypes?>();
            IsEnabled.ItemsFromEnum(typeof(YesNoValueTypes), YesNoValueTypes.Both);
            IsEnabled.SelectedValues = YesNoValueTypes.Yes;

            RoleList = new BsSelectList<ProjectRole?>();
            RoleList.ItemsFromEnum(typeof(ProjectRole));
            RoleList.SelectedValues = ProjectRole.Developer;
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

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(Resource))]
        [Display(Name = "Age", ResourceType = typeof(Resource))]
        [BsControl(BsControlType.NumberInline)]
        public BsRangeItem<int?> Age { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(Resource))]
        [Display(Name = "Location", Prompt = "Choose your country")]
        [BsControl(BsControlType.DropDownList)]
        public BsSelectList<string> CountriesList { get; set; }
    }

    public class ContributorOrderModel
    {
        public int Id { get; set; }
        public int Order { get; set; }
        public string Name { get; set; }
        public ProjectRole Role { get; set; }
        public int Depth { get; set; }
        public int? ParentId { get; set; }

        [BsControl(BsControlType.SortableList)]
        public IEnumerable<ContributorOrderModel> Subordinates { get; set; }
    }

    public class ContributorRowModel : BsGridRowModel<ContributorDetailsReadonly>
    {
        public int Id { get; set; }

        [BsGridColumn(Width = 2, MediumWidth = 2, IsEditable = true)]
        public BsGridColumnValue<string, string> Name { get; set; }

        [BsGridColumn(Width = 3, MediumWidth = 3)]
        public BsGridColumnValue<ProjectRole, ProjectRole> Role { get; set; }

        [BsGridColumn(Width = 3, MediumWidth = 3)]
        public DateTime StartDate { get; set; }

        [BsGridColumn(Width = 3, MediumWidth = 3)]
        public BsGridColumnValue<string, string> Contributions { get; set; }

        [BsGridColumn(Width = 1, MediumWidth = 1, Usage = BsGridColumnUsage.Html)]
        public string Action { get; set; }

        [BsGridColumn(Width = 1, Usage = BsGridColumnUsage.Excel)]
        public bool Enabled { get; set; }

        public override object GetUniqueID()
        {
            return this.Id;
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