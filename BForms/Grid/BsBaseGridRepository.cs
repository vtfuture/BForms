using BForms.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using BForms.Utilities;
using DocumentFormat.OpenXml.Office2010.PowerPoint;

namespace BForms.Grid
{
    /// <summary>
    /// Base repository for constructing grid model.
    /// </summary>
    /// <typeparam name="TEntity">Grid row bse model type. Usually maps on entity set</typeparam>
    /// <typeparam name="TRow">Grid row model type</typeparam>
    /// <typeparam name="TSearch">Grid search model type </typeparam>
    public abstract class BsBaseGridRepository<TEntity, TRow> where TEntity : class
    {
        /// <summary>
        /// Basic settings for contructing query. Contains data for query filtering, ordering, paging sent from client
        /// </summary>
        protected BsGridBaseRepositorySettings settings;

        public interface IQueryCriteria<TEntity>
        {
            IOrderedQueryable<TEntity> OrderBy(IQueryable<TEntity> query);

            IOrderedQueryable<TEntity> OrderByDescending(IQueryable<TEntity> query);
        }

        public class QueryCriteria<TEntity, TReturn> : IQueryCriteria<TEntity>
        {
            private Expression<Func<TEntity, TReturn>> storedExpression { get; set; }

            public QueryCriteria(Expression<Func<TEntity, TReturn>> expression)
            {
                storedExpression = expression;
            }

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
            /// <param name="query">Query that will be ordered</param>
            /// <param name="defaultOrderFunc">Default order row column. The query must be ordered before take/skip</param>
            /// <returns>Ordered query</returns>
            public IOrderedQueryable<TEntity> Order(IQueryable<TEntity> query, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> defaultOrderFunc)
            {
                //throw exception if defaultOrder is not asc or desc
                if (defaultOrderFunc == null)
                {
                    throw new Exception("you must implement a func for default order");
                }

                IOrderedQueryable<TEntity> orderedQuery = defaultOrderFunc(query);

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
        /// Fills row with details, must be implemented if settings.GetDetails is true
        /// </summary>
        /// <param name="row"></param>
        public virtual void FillDetails(TRow row)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Creates GridModel based on Query, OrderQuery and MapQuery with default sttings
        /// </summary>
        /// <returns></returns>
        public BsGridModel<TRow> ToBsGridViewModel()
        {
            var gridRepositorySettings = new BsGridBaseRepositorySettings
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
            var gridRepositorySettings = new BsGridBaseRepositorySettings
            {
                Page = page,
                PageSize = pageSize
            };

            return this.ToBsGridViewModel(gridRepositorySettings);
        }

        /// <summary>
        /// Creates GridModel for added row
        /// </summary>
        /// <typeparam name="TModel">Wrapper model type</typeparam>
        /// <param name="expression">Grid selector targeted for wrapping</param>
        /// <param name="row">Added row</param>
        /// <returns>Wrapper model</returns>
        public BsGridModel<TRow> ToBsGridViewModel(TRow row)
        {
            var grid = new BsGridModel<TRow>
            {
                Items = new List<TRow>
                {
                    row
                }
            };

            return grid;
        }

        /// <summary>
        /// Creates GridModel for added row
        /// </summary>
        /// <typeparam name="TModel">Wrapper model type</typeparam>
        /// <param name="expression">Grid selector targeted for wrapping</param>
        /// <param name="rows">Added rows</param>
        /// <returns>Wrapper model</returns>
        public BsGridModel<TRow> ToBsGridViewModel(List<TRow> rows)
        {
            var grid = new BsGridModel<TRow>
            {
                Items = rows
            };

            return grid;
        }

        public BsGridModel<TRow> ToBsGridViewModel<TValue>(BsGridBaseRepositorySettings baseSettings, List<TRow> rows, Expression<Func<TRow, TValue>> rowExpression, List<BsGridRowData<TValue>> rowsSettings)
        {
            this.settings = baseSettings;

            return ToBsGridViewModel<TValue>(rows, rowExpression, rowsSettings);
        }

        public BsGridModel<TRow> ToBsGridViewModel<TValue>(List<TRow> rows, Expression<Func<TRow, TValue>> rowExpression, List<BsGridRowData<TValue>> rowsSettings)
        {
            foreach (var row in rows)
            {
                var rowProp = rowExpression.GetPropertyInfo();
                var rowIdentifier = (TValue)rowProp.GetValue(row);
                if (rowsSettings.Any(x => x.Id.Equals(rowIdentifier) && x.GetDetails))
                {
                    this.FillDetails(row);
                }
            }

            var grid = new BsGridModel<TRow>
            {
                Items = rows
            };

            return grid;
        }

        public BsGridModel<TRow> ToBsGridViewModel(BsGridBaseRepositorySettings baseSettings, TRow row, bool getDetails)
        {
            this.settings = baseSettings;

            return ToBsGridViewModel(row, getDetails);
        }

        public BsGridModel<TRow> ToBsGridViewModel(TRow row, bool getDetails)
        {
            if (getDetails)
            {
                this.FillDetails(row);
            }

            var grid = new BsGridModel<TRow>
            {
                Items = new List<TRow> { row }
            };

            return grid;
        }

        /// <summary>
        /// Creates GridModel based on Query, OrderQuery and MapQuery
        /// </summary>
        /// <param name="settings">Settings</param>
        /// <param name="count">Total records</param>
        /// <returns>Wrapper model</returns>
        public BsGridModel<TRow> ToBsGridViewModel(BsGridBaseRepositorySettings settings, out int count)
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
        public virtual BsGridModel<TRow> ToBsGridViewModel(BsGridBaseRepositorySettings settings)
        {
            this.settings = settings;

            var result = new BsGridModel<TRow>();

            //creates basic query
            var basicQuery = this.Query();

            //performs count
            var totalRecords = basicQuery.Select(x => false).Count();

            //add column order
            result.BaseSettings.OrderColumns = settings.OrderColumns;

            //add orderable columns
            result.BaseSettings.OrderableColumns = settings.OrderableColumns;

            if (totalRecords > 0)
            {
                var pager = new BsPagerModel(totalRecords, this.settings.PageSize, this.settings.Page);

                IEnumerable<TRow> finalQuery = null;

                if (totalRecords > 1)
                {
                    this.orderedQueryBuilder = new OrderedQueryBuilder<TRow>(this.settings.OrderableColumns);
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

                if (settings.DetailsAll || settings.DetailsCount > 0)
                {
                    for (var i = 0; i < pager.CurrentPageRecords; i++)
                    {
                        if (settings.HasDetails(i))
                        {
                            var row = result.Items.ElementAt(i);
                            this.FillDetails(row);
                        }
                    }
                }
            }
            else
            {
                result.Items = new List<TRow>();
            }

            //sets base settings
            result.BaseSettings = this.settings.GetBase();

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