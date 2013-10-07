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
    public class Toolbar<TSearch, TNew>
    {
        public TSearch Search { get; set; }
        public TNew New { get; set; }
    }

    public class UsersViewModel
    {
        [BsGrid(HasDetails = true)]
        [Display(Name = "Users", ResourceType = typeof(Resource))]
        public BsGridModel<UsersGridRowModel> Grid { get; set; }

        [Display(Name = "Users", ResourceType = typeof(Resource))]
        public Toolbar<UsersSearchModel, UsersNewModel> Toolbar { get; set; }
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

        [Display(Name = "FirstName", ResourceType = typeof(Resource))]
        [BsControl(BsControlType.TextBox)]
        public string FirstName { get; set; }

        [Display(Name = "LastName", ResourceType = typeof(Resource))]
        [BsControl(BsControlType.TextBox)]
        public string LastName { get; set; }

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
}