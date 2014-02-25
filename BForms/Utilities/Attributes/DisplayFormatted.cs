using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace BForms.Utilities.Attributes
{

    //Used for displaying a localized string with parameters
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class DisplayFormatted : Attribute, IMetadataAware
    {

        // Resource Name, Resource Type, Parameters List  
        #region Constructor

        public DisplayFormatted(string currentName, Type currentResource, params object[] formatParams)
        {
            FormatParameters = formatParams;
            Name = currentName;
            ResourceType = currentResource;
        }

        #endregion

        #region Properties
        protected string Name { get; set; }
        protected object[] FormatParameters { get; set; }
        protected Type ResourceType { get; set; }
        #endregion

        public void OnMetadataCreated(ModelMetadata metadata)
        {
            if (!string.IsNullOrEmpty(Name))
            {
                metadata.DisplayName = GetFormatedName();
            }
        }

        // Return the localized string
        protected string GetFormatedName()
        {

            if (ResourceType == null)
            {
                return Name;
            }

            if (!String.IsNullOrEmpty(Name))
            {
                var propertyInfo = ResourceType.GetProperty(Name);

                if (propertyInfo != null)
                {
                    var localizedString = propertyInfo.GetValue(ResourceType).ToString();

                    if (FormatParameters != null)
                    {
                        try
                        {

                            var localizedFormatedString = String.Format(localizedString, FormatParameters);

                            return localizedFormatedString;
                        }
                        catch
                        {
                            throw new Exception(Name + " :The number of parameters is invalid ");

                        }
                    }

                    return localizedString;
                }

                throw new Exception(Name + " : The given key was not found in the specified resource type");
            }
            return String.Empty;
        }

    }
}
