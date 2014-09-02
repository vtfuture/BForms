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
    public class GroupEditorViewModel
    {
        public GroupEditorModel Editor { get; set; }
    }

    public class GroupEditorModel
    {
        [BsEditorTab(Name = "Developers", Id = ContributorType.Developer, Selected = true, Editable = true)]
        public BsEditorTabModel<ContributorRowModel, ContributorSearchModel, ContributorNewModel> Developers { get; set; }

        [BsEditorTab(Name = "Testers", Id = ContributorType.Tester, Selected = false)]
        public ContributorsInheritExample Testers { get; set; }

        [BsEditorGroup(Id = GroupEditorProjects.BForms)]
        public BsEditorGroupModel<ContributorsGroupRowModel, ContributorsRowFormModel> BFormsProject { get; set; }

        [BsEditorGroup(Id = GroupEditorProjects.RequireJs)]
        public BsEditorGroupModel<ContributorsGroupRowModel> RequireJsProject { get; set; }

        public GroupFormModel Form { get; set; }
    }

    public class ContributorsInheritExample : BsEditorTabModel<ContributorRowModel>
    {
        public ContributorSearchModel Search { get; set; }
    }

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
        [Display(Name = "Contributions")]
        [Required]
        [BsControl(BsControlType.TextBox)]
        public string Contributions { get; set; }
    }

    public class GroupFormModel
    {
        [BsControl(BsControlType.TextBox)]
        [Display(Name = "Name", Prompt = "Name", ResourceType = typeof(Resource))]
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(Resource))]
        public string Name { get; set; }
    }
}