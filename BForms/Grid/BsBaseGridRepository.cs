using System.Web.UI.HtmlControls;
using BForms.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Mvc;
using BForms.Mvc;
using BForms.Utilities;
using DocumentFormat.OpenXml.Spreadsheet;

namespace BForms.Grid
{
    /// <summary>
    /// Base repository for constructing grid model.
    /// </summary>
    /// <typeparam name="TEntity">Grid row bse model type. Usually maps on entity set</typeparam>
    /// <typeparam name="TRow">Grid row model type</typeparam>
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

            Expression GetExpression();

            Expression GetBody();
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

            public Expression GetExpression()
            {
                return this.storedExpression;
            }

            public Expression GetBody()
            {
                return this.storedExpression.Body;
            }
        }

        /// <summary>
        /// Builder for generic ordered query
        /// </summary>
        /// <typeparam name="TRow">Grid row model type</typeparam>
        public class OrderedQueryBuilder<TRow>
        {
            protected List<BsColumnOrder> columnsOrder;
            protected Dictionary<string, Func<IQueryable<TEntity>, BsOrderType, IOrderedQueryable<TEntity>>> delegateSettings = new Dictionary<string, Func<IQueryable<TEntity>, BsOrderType, IOrderedQueryable<TEntity>>>();
            protected Dictionary<string, IQueryCriteria<TEntity>> expressionSettings = new Dictionary<string, IQueryCriteria<TEntity>>();

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

                if (!expressionSettings.ContainsKey(columnName))
                {
                    expressionSettings.Add(columnName, criteria);
                }
                else
                {
                    expressionSettings[columnName] = criteria;
                }
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
            public virtual IOrderedQueryable<TEntity> Order(IQueryable<TEntity> query, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> defaultOrderFunc, BsGridBaseRepositorySettings gridSettings = null)
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
                                if (gridSettings == null || !gridSettings.GoTo.HasValue || gridSettings.GoTo.Value != BsDirectionType.Prev)
                                {
                                    orderedQuery = criteria.OrderBy(orderedQuery);
                                }
                                else
                                {
                                    orderedQuery = criteria.OrderByDescending(orderedQuery);
                                }
                            }
                            else if (item.Type == BsOrderType.Descending)
                            {
                                if (gridSettings == null || !gridSettings.GoTo.HasValue || gridSettings.GoTo.Value != BsDirectionType.Prev)
                                {
                                    orderedQuery = criteria.OrderByDescending(orderedQuery);
                                }
                                else
                                {
                                    orderedQuery = criteria.OrderBy(orderedQuery);
                                }
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
                            if (gridSettings == null || !gridSettings.GoTo.HasValue || gridSettings.GoTo.Value != BsDirectionType.Prev)
                            {
                                orderedQuery = orderedQuery.OrderBy(name);
                            }
                            else
                            {
                                orderedQuery = orderedQuery.OrderByDescending(name);
                            }
                        }
                        else if (item.Type == BsOrderType.Descending)
                        {
                            if (gridSettings == null || !gridSettings.GoTo.HasValue || gridSettings.GoTo.Value != BsDirectionType.Prev)
                            {
                                orderedQuery = orderedQuery.OrderByDescending(name);
                            }
                            else
                            {
                                orderedQuery = orderedQuery.OrderBy(name);
                            }
                        }
                    }
                }

                return orderedQuery;
            }

            public IQueryCriteria<TEntity> GetCriteria(string columnName)
            {
                if (this.expressionSettings.ContainsKey(columnName))
                {
                    return this.expressionSettings[columnName];
                }

                return null;
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
        public abstract IOrderedQueryable<TEntity> OrderQuery(IQueryable<TEntity> query, BsGridBaseRepositorySettings gridSettings = null);

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
        /// Paginates the result using the direction the orderQueryable
        /// </summary>
        /// <param name="query"></param>
        /// <param name="settings"></param>
        public virtual IQueryable<TEntity> PaginateBy<TValue>(IQueryable<TEntity> queryable, Expression<Func<TEntity, TValue>> uniqueIdFunc, BsGridBaseRepositorySettings gridSettings)
        {

            var query = queryable.AsQueryable();

            if (gridSettings.OrderableColumns != null && gridSettings.OrderableColumns.Any())
            {
                var pe = Expression.Parameter(typeof(TEntity), "x");

                foreach (var orderColumn in gridSettings.OrderableColumns)
                {
                    if (gridSettings.GoTo.Value == BsDirectionType.Next || gridSettings.GoTo.Value == BsDirectionType.Prev)
                    {
                        Expression left;
                        Expression right;
                        object value;

                        var parameterSetVisitor = new ParameterReplaceVisitor();

                        uniqueIdFunc = Expression.Lambda<Func<TEntity, TValue>>(parameterSetVisitor.ReplaceParameter(uniqueIdFunc.Body, pe), pe);

                        var propertyInfo = typeof(TRow).GetProperty(orderColumn.Name);

                        var baseType = propertyInfo.PropertyType.BaseType;

                        if (baseType.Name == (typeof(BsGridColumnValue).Name))
                        {
                            var mockValue = new BsGridColumnValue<object, object>();

                            propertyInfo = propertyInfo.PropertyType.GetProperties().FirstOrDefault(p => p.Name == mockValue.GetPropertyName(x => x.ItemValue));
                        }

                        var propertyType = propertyInfo.PropertyType;

                        value = propertyType.IsEnum ? Enum.Parse(propertyType, orderColumn.Value.ToString()) : Convert.ChangeType(orderColumn.Value, propertyType);

                        var criteria = this.orderedQueryBuilder.GetCriteria(orderColumn.Name);

                        if (criteria != null)
                        {
                            var body = criteria.GetBody();

                            left = parameterSetVisitor.ReplaceParameter(body, pe);
                        }
                        else
                        {
                            left = Expression.Property(pe, orderColumn.Name);
                        }


                        right = Expression.Constant(value, propertyType);

                        Expression predicateBody = null;

                        Expression stringCompare = null;

                        if (propertyType == typeof(string))
                        {
                            stringCompare = Expression.Call(left, typeof(string).GetMethod("CompareTo", new[] { typeof(string) }), right);
                        }

                        var zeroConstant = Expression.Constant(0, typeof(int));

                        var uniqueIdConstant = Expression.Constant(Convert.ChangeType(gridSettings.UniqueID, typeof(TValue)), typeof(TValue));

                        if (propertyType.IsEnum)
                        {
                            left = Expression.Convert(left, typeof(long));
                            right = Expression.Convert(right, typeof(long));
                        }


                        switch (gridSettings.GoTo.Value)
                        {
                            case BsDirectionType.Prev:
                                switch (orderColumn.Type)
                                {
                                    case BsOrderType.Descending:

                                        if (propertyType == typeof(string))
                                        {
                                            var sortingKeyExpr = Expression.GreaterThan(stringCompare, zeroConstant);
                                            var sortingKeyEqExpr = Expression.Equal(stringCompare, zeroConstant);
                                            var uniqueIdExpr = Expression.GreaterThan(uniqueIdFunc.Body, uniqueIdConstant);

                                            predicateBody = Expression.Or(sortingKeyExpr, Expression.AndAlso(sortingKeyEqExpr, uniqueIdExpr));
                                        }
                                        else
                                        {
                                            var sortingKeyExpr = Expression.GreaterThan(left, right);
                                            var sortingKeyEqExpr = Expression.Equal(left, right);
                                            var uniqueIdExpr = Expression.GreaterThan(uniqueIdFunc.Body, uniqueIdConstant);

                                            predicateBody = Expression.Or(sortingKeyExpr, Expression.AndAlso(sortingKeyEqExpr, uniqueIdExpr));
                                        }

                                        break;
                                    case BsOrderType.Ascending:

                                        if (propertyType == typeof(string))
                                        {
                                            var sortingKeyExpr = Expression.LessThan(stringCompare, zeroConstant);
                                            var sortingKeyEqExpr = Expression.Equal(stringCompare, zeroConstant);
                                            var uniqueIdExpr = Expression.LessThan(uniqueIdFunc.Body, uniqueIdConstant);

                                            predicateBody = Expression.Or(sortingKeyExpr, Expression.AndAlso(sortingKeyEqExpr, uniqueIdExpr));
                                        }
                                        else
                                        {
                                            var sortingKeyExpr = Expression.LessThan(left, right);
                                            var sortingKeyEqExpr = Expression.Equal(left, right);
                                            var uniqueIdExpr = Expression.LessThan(uniqueIdFunc.Body, uniqueIdConstant);

                                            predicateBody = Expression.Or(sortingKeyExpr, Expression.AndAlso(sortingKeyEqExpr, uniqueIdExpr));
                                        }
                                        break;
                                }
                                break;
                            case BsDirectionType.Next:
                                switch (orderColumn.Type)
                                {
                                    case BsOrderType.Descending:
                                        if (propertyType == typeof(string))
                                        {
                                            var sortingKeyExpr = Expression.LessThan(stringCompare, zeroConstant);
                                            var sortingKeyEqExpr = Expression.Equal(stringCompare, zeroConstant);
                                            var uniqueIdExpr = Expression.LessThan(uniqueIdFunc.Body, uniqueIdConstant);

                                            predicateBody = Expression.Or(sortingKeyExpr, Expression.AndAlso(sortingKeyEqExpr, uniqueIdExpr));
                                        }
                                        else
                                        {
                                            var sortingKeyExpr = Expression.LessThan(left, right);
                                            var sortingKeyEqExpr = Expression.Equal(left, right);
                                            var uniqueIdExpr = Expression.LessThan(uniqueIdFunc.Body, uniqueIdConstant);


                                            predicateBody = Expression.Or(sortingKeyExpr, Expression.AndAlso(sortingKeyEqExpr, uniqueIdExpr));
                                        }

                                        break;
                                    case BsOrderType.Ascending:

                                        if (propertyType == typeof(string))
                                        {
                                            var sortingKeyExpr = Expression.GreaterThan(stringCompare, zeroConstant);
                                            var sortingKeyEqExpr = Expression.Equal(stringCompare, zeroConstant);
                                            var uniqueIdExpr = Expression.GreaterThan(uniqueIdFunc.Body, uniqueIdConstant);

                                            predicateBody = Expression.Or(sortingKeyExpr, Expression.AndAlso(sortingKeyEqExpr, uniqueIdExpr));
                                        }
                                        else
                                        {
                                            var sortingKeyExpr = Expression.GreaterThan(left, right);
                                            var sortingKeyEqExpr = Expression.Equal(left, right);
                                            var uniqueIdExpr = Expression.GreaterThan(uniqueIdFunc.Body, uniqueIdConstant);


                                            predicateBody = Expression.Or(sortingKeyExpr, Expression.AndAlso(sortingKeyEqExpr, uniqueIdExpr));
                                        }
                                        break;
                                }
                                break;

                        }

                        //var leftParams = new ParameterVisitor().GetParameters(left);

                        var whereCallExpression =
                                           Expression.Call(
                                                           typeof(Queryable),
                                                           "Where",
                                                           new[] { query.ElementType },
                                                           query.Expression,
                                                           Expression.Lambda<Func<TEntity, bool>>(predicateBody, new[] { pe })
                                                          );


                        query = query.Provider.CreateQuery<TEntity>(whereCallExpression);
                    }

                }
            }

            return query;
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

        public BsGridModel<TRow> ToBsGridViewModel<TValue>(BsGridBaseRepositorySettings settings, out int count, Expression<Func<TEntity, TValue>> uniqueIdSelector)
        {
            var grid = ToBsGridViewModel(settings, uniqueIdSelector);

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

        private void SetResult<TValue>(BsGridModel<TRow> result, IQueryable<TEntity> basicQuery, BsGridBaseRepositorySettings settings, Expression<Func<TEntity, TValue>> uniqueIdSelector)
        {
            var totalRecords = basicQuery.Select(x => false).Count();

            if (totalRecords > 0)
            {
                var pager = new BsPagerModel(totalRecords, this.settings.PageSize, this.settings.Page);

                IEnumerable<TRow> finalQuery = null;

                if (totalRecords > 1)
                {
                    var orderedQuery = this.OrderQuery(basicQuery);

                    if (settings.OrderableColumns.Any(c => c.Type == BsOrderType.Descending))
                    {
                        orderedQuery = orderedQuery.ThenByDescending(uniqueIdSelector);
                    }
                    else
                    {
                        orderedQuery = orderedQuery.ThenBy(uniqueIdSelector);
                    }

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
        }

        private void SetNoOffsetResult<TValue>(BsGridModel<TRow> result, IQueryable<TEntity> basicQuery, BsGridBaseRepositorySettings settings, Expression<Func<TEntity, TValue>> uniqueIdSelector)
        {
            var pager = new BsPagerModel
            {
                PageSize = settings.PageSize
            };

            IEnumerable<TRow> finalQuery = new List<TRow>();

            pager.GoTo = settings.GoTo.Value;

            if (settings.GoTo.Value == BsDirectionType.Last)
            {
                foreach (var orderItem in result.BaseSettings.OrderableColumns)
                {
                    if (orderItem.Type == BsOrderType.Descending)
                    {
                        orderItem.Type = BsOrderType.Ascending;
                    }
                    else if (orderItem.Type == BsOrderType.Ascending)
                    {
                        orderItem.Type = BsOrderType.Descending;
                    }
                }
            }


            var orderedQuery = this.OrderQuery(basicQuery, settings);

            var intermediateQuery = this.PaginateBy(orderedQuery, uniqueIdSelector, settings);

            var orderedPagerQuery = this.OrderQuery(intermediateQuery, settings);

            if (settings.OrderableColumns.Any(c => c.Type == BsOrderType.Descending))
            {
                if (this.settings.GoTo.HasValue && this.settings.GoTo.Value == BsDirectionType.Prev)
                {
                    orderedPagerQuery = orderedPagerQuery.ThenBy(uniqueIdSelector);
                }
                else
                {
                    orderedPagerQuery = orderedPagerQuery.ThenByDescending(uniqueIdSelector);
                }
            }
            else
            {
                if (this.settings.GoTo.HasValue && this.settings.GoTo.Value == BsDirectionType.Prev)
                {
                    orderedPagerQuery = orderedPagerQuery.ThenByDescending(uniqueIdSelector);
                }
                else
                {
                    orderedPagerQuery = orderedPagerQuery.ThenBy(uniqueIdSelector);
                }
            }


            if (settings.GoTo.Value == BsDirectionType.Last)
            {
                foreach (var orderItem in result.BaseSettings.OrderableColumns)
                {
                    if (orderItem.Type == BsOrderType.Descending)
                    {
                        orderItem.Type = BsOrderType.Ascending;
                    }
                    else if (orderItem.Type == BsOrderType.Ascending)
                    {
                        orderItem.Type = BsOrderType.Descending;
                    }
                }
            }

            var pagedQuery = this.OrderQuery(orderedPagerQuery.Take(pager.PageSize));

            if (settings.OrderableColumns.Any(c => c.Type == BsOrderType.Descending))
            {
                pagedQuery = pagedQuery.ThenByDescending(uniqueIdSelector);
            }
            else
            {
                pagedQuery = pagedQuery.ThenBy(uniqueIdSelector);
            }

            finalQuery = this.MapQuery(pagedQuery);

            result.Items = finalQuery.ToList();

            result.Pager = pager;

            result.Pager.CurrentPageRecords = result.Items.Count();
            result.Pager.TotalRecords = result.Pager.CurrentPageRecords;

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

        /// <summary>
        /// Creates GridModel based on Query, OrderQuery and MapQuery
        /// </summary>
        /// <param name="settings">Requested settings</param>
        /// <returns>Grid model</returns>
        public virtual BsGridModel<TRow> ToBsGridViewModel<TValue>(BsGridBaseRepositorySettings settings, Expression<Func<TEntity, TValue>> uniqueIdSelector)
        {
            this.settings = settings;

            var result = new BsGridModel<TRow>();

            //creates basic query
            var basicQuery = this.Query();

            //add column order
            result.BaseSettings.OrderColumns = settings.OrderColumns;

            //add orderable columns
            result.BaseSettings.OrderableColumns = settings.OrderableColumns;

            this.orderedQueryBuilder = new OrderedQueryBuilder<TRow>(this.settings.OrderableColumns);

            if (this.settings.GoTo.HasValue)
            {
                this.SetNoOffsetResult(result, basicQuery, settings, uniqueIdSelector);
            }
            else
            {
                this.SetResult(result, basicQuery, settings,uniqueIdSelector);
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