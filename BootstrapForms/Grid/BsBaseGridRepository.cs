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
    /// <summary>
    /// Base repository for constructing grid model.
    /// </summary>
    /// <typeparam name="TEntity">Grid row bse model type. Usually maps on entity set</typeparam>
    /// <typeparam name="TRow">Grid row model type</typeparam>
    /// <typeparam name="TSearch">Grid search model type </typeparam>
    public abstract class BsBaseGridRepository<TEntity, TRow, TSearch> where TEntity : class
    {
        /// <summary>
        /// Basic settings for contructing query. Contains data for query filtering, ordering, paging sent from client
        /// </summary>
        protected BsGridRepositorySettings<TSearch> settings;

        /// <summary>
        /// Builder for generic ordered query
        /// </summary>
        /// <typeparam name="TRow">Grid row model type</typeparam>
        public class OrderedQueryBuilder<TRow>
        {
            List<BsColumnOrder> columnsOrder;
            Dictionary<string, Func<IQueryable<TEntity>, BsOrderType, IOrderedQueryable<TEntity>>> delegateSettings = new Dictionary<string, Func<IQueryable<TEntity>, BsOrderType, IOrderedQueryable<TEntity>>>();
            Dictionary<string, Expression<Func<TEntity, object>>> expressionSettings = new Dictionary<string, Expression<Func<TEntity, object>>>();

            /// <summary>
            /// .ctor
            /// </summary>
            /// <param name="columnsOrder">Columns order list sent from client</param>
            public OrderedQueryBuilder(List<BsColumnOrder> columnsOrder)
            {
                this.columnsOrder = columnsOrder;
            }

            /// <summary>
            /// Stores order expression for row column. Used when row column order is different from db column order 
            /// or row column order is based on multiple columns in db
            /// Ascending or descending order is handled automatically
            /// </summary>
            /// <typeparam name="TKey">Row property type</typeparam>
            /// <param name="columnSelector">Row column selector</param>
            /// <param name="orderDelegate">Order expression</param>
            public void OrderFor<TKey>(Expression<Func<TRow, TKey>> columnSelector, Expression<Func<TEntity, object>> orderDelegate)
            {
                var fullName = ExpressionHelper.GetExpressionText(columnSelector);
                var columnName = fullName.Split('.').Last();
                expressionSettings.Add(columnName, orderDelegate);
            }

            /// <summary>
            /// Stores complex order delegate for row column. Used when row column order is based on multiple columns order in db.
            /// Must handle ascending and descending order
            /// </summary>
            /// <typeparam name="TKey">Row property type</typeparam>
            /// <param name="columnSelector">Row column selector</param>
            /// <param name="orderDelegate">Order delegate</param>
            public void OrderFor<TKey>(Expression<Func<TRow, TKey>> columnSelector, Func<IQueryable<TEntity>, BsOrderType, IOrderedQueryable<TEntity>> orderDelegate)
            {
                var fullName = ExpressionHelper.GetExpressionText(columnSelector);
                var columnName = fullName.Split('.').Last();

                delegateSettings.Add(columnName, orderDelegate);
            }

            /// <summary>
            /// Creates Ordered query based on stored expressions and delegates.
            /// For row columns that have identical names with db columns, the order is made automatically
            /// </summary>
            /// <typeparam name="TKey">Row property type</typeparam>
            /// <param name="query">Query that will be ordered</param>
            /// <param name="defaultOrderProp">Default order row column. The query must be ordered before take/skip</param>
            /// <param name="defaultOrderType">Default order row column type</param>
            /// <returns>Ordered query</returns>
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

        /// <summary>
        /// Ordered query builder var
        /// </summary>
        protected OrderedQueryBuilder<TRow> orderedQueryBuilder;

        /// <summary>
        /// Basic query builder. Recommended to be used for grid count
        /// </summary>
        /// <returns>Basic query</returns>
        public abstract IQueryable<TEntity> Query();

        /// <summary>
        /// Query ordering. To be used before grid paging
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public abstract IOrderedQueryable<TEntity> OrderQuery(IQueryable<TEntity> query);

        /// <summary>
        /// Query mapping. Recommended to be used after grid paging
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public abstract IEnumerable<TRow> MapQuery(IQueryable<TEntity> query);

        /// <summary>
        /// Creates GridModel based on Query, OrderQuery and MapQuery with default sttings
        /// </summary>
        /// <returns></returns>
        public BsGridModel<TRow> ToBsGridViewModel()
        {
            var settings = new BsGridRepositorySettings<TSearch>();
            settings.Page = 1;
            settings.PageSize = 5;

            return this.ToBsGridViewModel(settings);
        }

        /// <summary>
        /// Creates GridModel based on Query, OrderQuery and MapQuery
        /// </summary>
        /// <param name="page">Requested page number</param>
        /// <param name="pageSize">Page size - number of records on requested page</param>
        /// <returns></returns>
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

        public TModel ToBsGridViewModel<TModel>(Expression<Func<TModel, BsGridModel<TRow>>> expression, TRow row) where TModel : new()
        {
            var grid = new BsGridModel<TRow>
            {
                Items = new List<TRow>
                                           {
                                               row
                                           }
            };

            return SetGridProperty(expression, grid);
        }

        public BsGridModel<TRow> ToBsGridViewModel(BsGridRepositorySettings<TSearch> settings, out int count)
        {
            var model = ToBsGridViewModel(settings);

            count = model.Pager.TotalRecords;

            return model;
        }

        /// <summary>
        /// Creates GridModel based on Query, OrderQuery and MapQuery
        /// </summary>
        /// <param name="settings">Requested settings</param>
        /// <returns>Grid model</returns>
        public virtual BsGridModel<TRow> ToBsGridViewModel(BsGridRepositorySettings<TSearch> settings)
        {
            this.settings = settings;

            var result = new BsGridModel<TRow>();

            //creates basic query
            var basicQuery = this.Query();

            //performs count
            var totalRecords = basicQuery.Select(x => false).Count();

            if (totalRecords > 0)
            {
                var pager = new BsPagerModel(totalRecords, this.settings.PageSize, this.settings.Page);

                IEnumerable<TRow> finalQuery = null;

                if (totalRecords > 1)
                {
                    this.orderedQueryBuilder = new OrderedQueryBuilder<TRow>(this.settings.OrderColumns);
                    var orderedQuery = this.OrderQuery(basicQuery);

                    var pagedQuery = orderedQuery.Skip(pager.PageSize * (pager.CurrentPage - 1)).Take(pager.PageSize);

                    finalQuery = this.MapQuery(pagedQuery);
                }
                else
                {
                    finalQuery = this.MapQuery(basicQuery);
                }

                // get items for current page
                result.Items = finalQuery.ToList();

                //sets pager
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