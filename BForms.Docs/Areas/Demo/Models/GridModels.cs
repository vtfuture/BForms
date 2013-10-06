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
        [BsGrid(HasDetails = false)]
        [Display(Name = "Users", ResourceType = typeof(Resource))]
        public BsGridModel<UsersGridRowModel> Grid { get; set; }

        [Display(Name = "Users", ResourceType = typeof(Resource))]
        public Toolbar<UsersSearchModel, UsersNewModel> Toolbar { get; set; }
    }

    public class UsersGridRowModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime RegisterDate { get; set; }
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
    }

    public class UsersNewModel
    {
        public UsersNewModel()
        {
            IsEnabled = new BsSelectList<YesNoValueTypes?>();
            IsEnabled.ItemsFromEnum(typeof(YesNoValueTypes), YesNoValueTypes.Both);
            IsEnabled.SelectedValues = YesNoValueTypes.Yes;
        }

        [Display(Name = "Name", ResourceType = typeof(Resource))]
        [BsControl(BsControlType.TextBox)]
        public string FirstName { get; set; }

        [Display(Name = "Name", ResourceType = typeof(Resource))]
        [BsControl(BsControlType.TextBox)]
        public string LastName { get; set; }

        [BsControl(BsControlType.RadioButtonList)]
        [Display(Name = "IsEnabled", ResourceType = typeof(Resource))]
        public BsSelectList<YesNoValueTypes?> IsEnabled { get; set; }
    }
}