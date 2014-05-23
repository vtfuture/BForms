using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using BForms.Models;

namespace BForms.FormBuilder
{
    public class FormBuilderControlDisplay : Attribute
    {
        public string DisplayName { get; set; }
        public Glyphicon Glyphicon { get; set; }
        public string Name { get; set; }
    }

    public class FormBuilderControlMetadata : Attribute
    {
        public BsControlType BsControlType { get; set; }
    }

    public class FormBuilderPropertiesTab : Attribute
    {
        public Glyphicon Glyphicon { get; set; }
        public bool Expandable { get; set; }
        public int Order { get; set; }

        public FormBuilderPropertiesTab()
        {
            Expandable = true;
        }
    }
    
    public class FormBuilderControlProperty : Attribute
    {
        public FormBuilderControlType ControlType { get; set; }
    }
    
    public class FormGroup : Attribute
    {
        public FormGroup(ColumnWidth width = ColumnWidth.Large, Glyphicon glyphiconAddon = Glyphicon.Custom)
        {
            Width = width;
            GlyphiconAddon = glyphiconAddon;
        }

        public ColumnWidth Width { get; set; }
        public Glyphicon GlyphiconAddon { get; set; }
    }

    public class BsComponent : Attribute
    {
        public int Order { get; set; }
    }

    public class FormButtons : Attribute
    {
        
    }

    #region Attribute helpers

    public static class BsComponentExtensions
    {
        public static BsComponent GetBsComponentAttribute(object obj)
        {
            var attribute =
                obj.GetType()
                    .GetCustomAttributes(typeof (BsComponent), true)
                    .Select(x => x as BsComponent)
                    .FirstOrDefault();

            return attribute;
        }

        public static FormButtons GetFormButtonsAttribute(object obj)
        {
            var attribute =
                obj.GetType()
                    .GetCustomAttributes(typeof(FormButtons), true)
                    .Select(x => x as FormButtons)
                    .FirstOrDefault();

            return attribute;
        }
    }

    #endregion
}
