using BForms.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BForms.GroupEditor
{
    public class BsEditorTabBuilder<TRow> : BaseComponent where TRow : new()
    {
        #region Properties and Constructor
        private string name { get; set; }
        private string id { get; set; }

        internal string Name
        {
            get { return this.name; }
        }

        public BsEditorTabBuilder(string name, string id)
        {
            this.name = name;
            this.id = id;
        }
        #endregion

        #region Config
        public BsEditorTabBuilder<TRow> DisplayName(string name)
        {
            this.name = name;

            return this;
        }
        #endregion

        #region Render
        public override string Render()
        {
            return this.name + " " + this.id;
        }
        #endregion
    }
}
