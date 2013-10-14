using BForms.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using BForms.Utilities;

namespace BForms.Grid
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

        public interface IQueryCriteria<TEntity>
        {
            IOrderedQueryable<TEntity> OrderBy(IQueryable<TEntity> query);

            IOrderedQueryable<TEntity> OrderByDescending(IQueryable<TEntity> query);
        }

        public class QueryCriteria<TEntity, TReturn> : IQueryCriteria<TEntity>
        {
            public QueryCriteria(Expression<Func<TEntity, TReturn>> expression)
            {
                storedExpression = expression;
            }
            private static Expression<Func<TEntity, TReturn>> storedExpression { get; set; }
            public IOrderedQueryable<TEntity> OrderBy(IQueryable<TEntity> query)
            {
                return query.OrderBy(storedExpression);
            }

            public IOrderedQueryable<TEntity> OrderByDescending(IQueryable<TEntity> query)
            {
                return query.OrderByDescending(storedExpression);
            }
        }

        /// <summary>
        /// Builder for generic ordered query
        /// </summary>
        /// <typeparam name="TRow">Grid row model type</typeparam>
        public class OrderedQueryBuilder<TRow>
        {
            List<BsColumnOrder> columnsOrder;
            Dictionary<string, Func<IQueryable<TEntity>, BsOrderType, IOrderedQueryable<TEntity>>> delegateSettings = new Dictionary<string, Func<IQueryable<TEntity>, BsOrderType, IOrderedQueryable<TEntity>>>();
            Dictionary<string, IQueryCriteria<TEntity>> expressionSettings = new Dictionary<string, IQueryCriteria<TEntity>>();

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
            public void OrderFor<TKey, TReturn>(Expression<Func<TRow, TKey>> columnSelector, Expression<Func<TEntity, TReturn>> orderDelegate)
            {

                var fullName = ExpressionHelper.GetExpressionText(columnSelector);
                var columnName = fullName.Split('.').Last();

                IQueryCriteria<TEntity> criteria = new QueryCriteria<TEntity, TReturn>(orderDelegate);
                expressionSettings.Add(columnName, criteria);
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
                else if (defaultOrderType == BsOrderType.Descending)
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
                            var criteria = this.expressionSettings[name];

                            if (item.Type == BsOrderType.Ascending)
                            {
                                orderedQuery = criteria.OrderBy(orderedQuery);
                            }
                            else if (item.Type == BsOrderType.Descending)
                            {
                                orderedQuery = criteria.OrderByDescending(orderedQuery);
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
                        else if (item.Type == BsOrderType.Descending)
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
            var gridRepositorySettings = new BsGridRepositorySettings<TSearch>
            {
                Page = 1, 
                PageSize = 5
            };

            return this.ToBsGridViewModel(gridRepositorySettings);
        }

        /// <summary>
        /// Creates GridModel based on Query, OrderQuery and MapQuery
        /// </summary>
        /// <param name="page">Requested page number</param>
        /// <param name="pageSize">Page size - number of records on requested page</param>
        /// <returns></returns>
        public BsGridModel<TRow> ToBsGridViewModel(int page, int pageSize)
        {
            var gridRepositorySettings = new BsGridRepositorySettings<TSearch>
            {
                Page = page, 
                PageSize = pageSize
            };

            return this.ToBsGridViewModel(gridRepositorySettings);
        }

        /// <summary>
        /// Creates GridModel based on Query, OrderQuery and MapQuery and wrapps it for further use
        /// </summary>
        /// <typeparam name="TModel">Wrapper model type</typeparam>
        /// <param name="expression">Grid selector targeted for wrapping</param>
        /// <param name="settings">Settings</param>
        /// <param name="count">Total records</param>
        /// <returns>Wrapper model</returns>
        public TModel ToBsGridViewModel<TModel>(Expression<Func<TModel, BsGridModel<TRow>>> expression, BsGridRepositorySettings<TSearch> settings, out int count) where TModel : new()
        {
            var grid = this.ToBsGridViewModel(settings);

            count = grid.Pager == null ? 0 : grid.Pager.TotalRecords;

            return SetGridProperty(expression, grid);
        }

        /// <summary>
        /// Creates GridModel based on Query, OrderQuery and MapQuery and wrapps it for further use
        /// </summary>
        /// <typeparam name="TModel">Wrapper model type</typeparam>
        /// <param name="expression">Grid selector targeted for wrapping</param>
        /// <param name="settings">Settings</param>
        /// <returns>Wrapper model</returns>
        public TModel ToBsGridViewModel<TModel>(Expression<Func<TModel, BsGridModel<TRow>>> expression, BsGridRepositorySettings<TSearch> settings) where TModel : new()
        {
            var grid = this.ToBsGridViewModel(settings);

            return SetGridProperty(expression, grid);
        }

        /// <summary>
        /// Creates GridModel for added row
        /// </summary>
        /// <typeparam name="TModel">Wrapper model type</typeparam>
        /// <param name="expression">Grid selector targeted for wrapping</param>
        /// <param name="row">Added row</param>
        /// <returns>Wrapper model</returns>
        public TModel ToBsGridViewModel<TModel>(Expression<Func<TModel, BsGridModel<TRow>>> expression, TRow row)
            where TModel : new()
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

        /// <summary>
        /// Creates GridModel based on Query, OrderQuery and MapQuery
        /// </summary>
        /// <param name="settings">Settings</param>
        /// <param name="count">Total records</param>
        /// <returns>Wrapper model</returns>
        public BsGridModel<TRow> ToBsGridViewModel(BsGridRepositorySettings<TSearch> settings, out int count)
        {
            var grid = ToBsGridViewModel(settings);

            count = grid.Pager == null ? 0 : grid.Pager.TotalRecords;

            return grid;
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

        /// <summary>
        /// Helper for wrapping grid
        /// </summary>
        /// <typeparam name="TModel">Wrapper model type</typeparam>
        /// <param name="expression">Grid selector targeted for wrapping</param>
        /// <param name="grid">Grid object</param>
        /// <returns>Wrapper model</returns>
        private static TModel SetGridProperty<TModel>(Expression<Func<TModel, BsGridModel<TRow>>> expression, BsGridModel<TRow> grid) where TModel : new()
        {
            var model = new TModel();
            var gridProp = expression.GetPropertyInfo();
            gridProp.SetValue(model, grid);
            return model;
        }
    }
}