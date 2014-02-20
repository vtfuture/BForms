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
    public class ContributorsGroupRowModel : BsEditorGroupItemModel<ContributorsRowFormModel>
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public override object GetUniqueID()
        {
            return this.Id;
        }
    }

    public class ContributorsRowFormModel
    {
        [Display(Name = "Name", ResourceType = typeof(Resource))]
        [Required]
        [BsControl(BsControlType.TextBox)]
        public string Name { get; set; }

        [Display(Name = "Location", Prompt = "PromptLocation", ResourceType = typeof(Resource))]
        [BsControl(BsControlType.DropDownList)]
        public BsSelectList<string> CountriesList { get; set; }
    }

    public class ContributorsOrderModel { }

    public class ContributorsInheritExample : BsEditorTabModel<ContributorRowModel, ContributorSearchModel>
    {
        public ContributorsOrderModel Order { get; set; }
    }

    public class GroupEditorModel
    {
        [BsEditorTab(Name = "Contributors1", Id = YesNoValueTypes.Yes, Selected = true, Editable = true)]
        public ContributorsInheritExample Contributors { get; set; }

        [BsEditorTab(Name = "Contributors2", Id = YesNoValueTypes.No, Selected = false)]
        public BsEditorTabModel<ContributorRowModel> Contributors2 { get; set; }

        [BsEditorTab(Name = "Contributors3", Id = YesNoValueTypes.Both, Selected = false)]
        public BsEditorTabModel<ContributorRowModel, ContributorSearchModel, ContributorNewModel> Contributors3 { get; set; }

        [BsEditorGroup(Id = NotificationType.Daily)]
        public BsEditorGroupModel<ContributorsGroupRowModel, ContributorsRowFormModel> Group1 { get; set; }

        [BsEditorGroup(Id = NotificationType.Monthly)]
        public BsEditorGroupModel<ContributorsGroupRowModel> Group2 { get; set; }

        public GroupFormModel Form { get; set; }
    }

    public class GroupEditorViewModel
    {
        public GroupEditorModel Editor1 { get; set; }
        public GroupEditorModel Editor2 { get; set; }
    }

    public class GroupFormModel
    {
        [BsControl(BsControlType.TextBox)]
        [Display(Name = "Name", Prompt = "Name", ResourceType = typeof(Resource))]
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(Resource))]
        public string Name { get; set; }
    }
}