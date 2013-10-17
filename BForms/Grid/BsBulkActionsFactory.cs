using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using BForms.Mvc;
using BForms.Utilities;

namespace BForms.Grid
{
    public class BsBulkActionsFactory
    {
        private List<BsBulkAction> actions = new List<BsBulkAction>();
        private List<BsBulkSelector> selectors = new List<BsBulkSelector>();
        private readonly ViewContext viewContext;

        internal List<BsBulkAction> BulkActions
        {
            get
            {
                return this.actions;
            }
        }

        internal List<BsBulkSelector> BulkSelectors
        {
            get
            {
                return this.selectors;
            }
        }

        public BsBulkActionsFactory(ViewContext context)
        {
            this.viewContext = viewContext;
        }

        #region actions
        public BsBulkAction AddAction()
        {
            var bulkAction = new BsBulkAction();
            actions.Add(bulkAction);
            return bulkAction;
        }

        public BsBulkAction AddAction(BsBulkActionType type)
        {
            var bulkAction = new BsBulkAction(type, viewContext);
            this.AddAction(bulkAction);
            return bulkAction;
        }

        public BsBulkAction AddAction(BsBulkAction bulkAction)
        {
            actions.Add(bulkAction);
            return bulkAction;
        }

        public BsBulkAction ForAction(BsBulkActionType type)
        {
            BsBulkAction bulkAction = this.GetBulkAction(type);
            return bulkAction;
        }

        private BsBulkAction GetBulkAction(BsBulkActionType type)
        {
            var bulkAction = this.actions.FirstOrDefault(x => x.Type == type);

            if (bulkAction == null)
            {
                throw new ArgumentException("No bulk action found with the specified type");
            }

            return bulkAction;
        }
        #endregion

        #region selectors
        public BsBulkSelector AddSelector()
        {
            var bulkSelector = new BsBulkSelector();
            selectors.Add(bulkSelector);
            return bulkSelector;
        }

        public BsBulkSelector AddSelector(BsBulkSelectorType type)
        {
            var bulkSelector = new BsBulkSelector(type, viewContext);
            this.AddSelector(bulkSelector);
            return bulkSelector;
        }

        public BsBulkSelector AddSelector(BsBulkSelector bulkSelector)
        {
            selectors.Add(bulkSelector);
            return bulkSelector;
        }

        public BsBulkSelector ForSelector(BsBulkSelectorType type)
        {
            BsBulkSelector bulkSelector = this.GetBulkSelector(type);
            return bulkSelector;
        }

        private BsBulkSelector GetBulkSelector(BsBulkSelectorType type)
        {
            var bulkSelector = this.selectors.FirstOrDefault(x => x.Type == type);

            if (bulkSelector == null)
            {
                throw new ArgumentException("No bulk selector found with the specified type");
            }

            return bulkSelector;
        }
        #endregion
    }
}
