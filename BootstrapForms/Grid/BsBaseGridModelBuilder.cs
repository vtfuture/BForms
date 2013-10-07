using BootstrapForms.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using BootstrapForms.Utilities;

namespace BootstrapForms.Grid
{
    public abstract class BsBaseGridModelBuilder<TEntity, TRow, TSearch> where TEntity : class
    {
        protected BsGridRepositorySettings<TSearch> settings;
        
        public class OrderedQueryBuilder<TRow>
        {
            List<BsColumnOrder> columnsOrder;
            Dictionary<string, Func<IQueryable<TEntity>, BsOrderType, IOrderedQueryable<TEntity>>> delegateSettings = new Dictionary<string, Func<IQueryable<TEntity>, BsOrderType, IOrderedQueryable<TEntity>>>();
            Dictionary<string, Expression<Func<TEntity, object>>> expressionSettings = new Dictionary<string,Expression<Func<TEntity,object>>>();
            
            public OrderedQueryBuilder(List<BsColumnOrder> columnsOrder)
            {
                this.columnsOrder = columnsOrder;
            }

            public void OrderFor<TKey>(Expression<Func<TRow, TKey>> columnSelector, Expression<Func<TEntity, object>> orderDelegate)
            {
                var fullName = ExpressionHelper.GetExpressionText(columnSelector);
                var columnName = fullName.Split('.').Last();
                expressionSettings.Add(columnName, orderDelegate);
            }

            public void OrderFor<TKey>(Expression<Func<TRow, TKey>> columnSelector, Func<IQueryable<TEntity>, BsOrderType, IOrderedQueryable<TEntity>> orderDelegate)
            {
                var fullName = ExpressionHelper.GetExpressionText(columnSelector);
                var columnName = fullName.Split('.').Last();

                delegateSettings.Add(columnName, orderDelegate);
            }

            public IOrderedQueryable<TEntity> Order<TKey>(IQueryable<TEntity> query, Expression<Func<TEntity, TKey>> defaultOrderProp, BsOrderType defaultOrderType)
            {
                //throw exception if defaultOrder is not asc or desc
                if (defaultOrderType == BsOrderType.Default)
                {
                    throw new Exception("default order must be ascending or descending");
                }

                IOrderedQueryable<TEntity> orderedQuery = null;

                //get default property name to apply default order
                var defaultPropName = ExpressionHelper.GetExpressionText(defaultOrderProp);
                defaultPropName = defaultPropName.Split('.').Last();

                //apply default order based on order type
                if (defaultOrderType == BsOrderType.Ascending)
                {
                    orderedQuery = query.OrderBy(defaultPropName);
                }
                else
                {
                    orderedQuery = query.OrderByDescending(defaultPropName);
                }

                if (this.columnsOrder != null && this.columnsOrder.Any())
                {
                    this.columnsOrder.Reverse();

                    //order dict desc and order query based on Order priority
                    foreach (var item in this.columnsOrder)
                    {
                        var name = item.Name;

                        if (this.expressionSettings.ContainsKey(name))
                        {
                            var expression = this.expressionSettings[name];

                            if (item.Type == BsOrderType.Ascending)
                            {
                                orderedQuery = orderedQuery.OrderBy(expression);
                            }
                            else
                            {
                                orderedQuery = orderedQuery.OrderByDescending(expression);
                            }

                            continue;
                        }

                        if (this.delegateSettings.ContainsKey(name))
                        {
                            orderedQuery = this.delegateSettings[name](orderedQuery, item.Type);

                            continue;
                        }

                        if (item.Type == BsOrderType.Ascending)
                        {
                            orderedQuery = orderedQuery.OrderBy(name);
                        }
                        else
                        {
                            orderedQuery = orderedQuery.OrderByDescending(name);
                        }
                    }
                }

                return orderedQuery;
            }
        }

        protected OrderedQueryBuilder<TRow> orderedQueryBuilder;
        
        public abstract IQueryable<TEntity> Query();

        public abstract IOrderedQueryable<TEntity> OrderQuery(IQueryable<TEntity> query);

        public abstract IEnumerable<TRow> MapQuery(IQueryable<TEntity> query);

        public BsGridModel<TRow> ToBsGridViewModel()
        {
            var settings = new BsGridRepositorySettings<TSearch>();
            settings.Page = 1;
            settings.PageSize = 5;

            return this.ToBsGridViewModel(settings);
        }

        public BsGridModel<TRow> ToBsGridViewModel(int page, int pageSize)
        {
            var settings = new BsGridRepositorySettings<TSearch>();
            settings.Page = page;
            settings.PageSize = pageSize;

            return this.ToBsGridViewModel(settings);
        }

        public TModel ToBsGridViewModel<TModel>(Expression<Func<TModel, BsGridModel<TRow>>> expression, BsGridRepositorySettings<TSearch> settings, out int count) where TModel : new()
        {
            var grid = this.ToBsGridViewModel(settings);

            count = grid.Pager.TotalRecords;

            return SetGridProperty(expression, grid);
        }

        public TModel ToBsGridViewModel<TModel>(Expression<Func<TModel, BsGridModel<TRow>>> expression, BsGridRepositorySettings<TSearch> settings) where TModel : new()
        {
            var grid = this.ToBsGridViewModel(settings);

            return SetGridProperty(expression, grid);
        }

        public virtual BsGridModel<TRow> ToBsGridViewModel(BsGridRepositorySettings<TSearch> settings, out int count)
        {
            var model = ToBsGridViewModel(settings);

            count = model.Pager.TotalRecords;

            return model;
        }

        public virtual BsGridModel<TRow> ToBsGridViewModel(BsGridRepositorySettings<TSearch> settings)
        {
            this.settings = settings;

            var result = new BsGridModel<TRow>();

            var basicQuery = this.Query();
            
            var totalRecords = basicQuery.Select(x => false).Count();

            if (totalRecords > 0)
            {
                var pager = new BsPagerModel(totalRecords, this.settings.PageSize, this.settings.Page);

                this.orderedQueryBuilder = new OrderedQueryBuilder<TRow>(this.settings.OrderColumns);
                var orderedQuery = this.OrderQuery(basicQuery);

                var pagedQuery = orderedQuery.Skip(pager.PageSize * (pager.CurrentPage - 1)).Take(pager.PageSize);

                var finalQuery = this.MapQuery(pagedQuery);

                result.Items = finalQuery.ToList();

                pager.CurrentPageRecords = result.Items.Count();
                result.Pager = pager;
            }
            else
            {
                result.Items = new List<TRow>();
            }

            return result;
        }

        public virtual BsGridModel<TRow> ToBsGridViewModel(IQueryable<TEntity> basicQuery, IOrderedQueryable<TRow> finalQuery)
        {
            var result = new BsGridModel<TRow>();

            var totalRecords = basicQuery.Select(x => false).Count();

            if (totalRecords > 0)
            {
                var pager = new BsPagerModel(totalRecords, this.settings.PageSize, this.settings.Page);
                var pagedQuery = basicQuery.Skip(pager.PageSize * (pager.CurrentPage - 1)).Take(pager.PageSize);

                result.Items = finalQuery.ToList();
                result.Pager.CurrentPageRecords = result.Items.Count();
            }
            else
            {
                result.Items = new List<TRow>();
            }

            return result;
        }

        private TModel SetGridProperty<TModel>(Expression<Func<TModel, BsGridModel<TRow>>> expression, BsGridModel<TRow> grid) where TModel : new()
        {
            var model = new TModel();
            var gridProp = expression.GetPropertyInfo();
            gridProp.SetValue(model, grid);
            return model;
        }
    }
}