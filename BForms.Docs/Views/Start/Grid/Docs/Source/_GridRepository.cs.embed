using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BForms.Grid;
using MyGrid.Mock;
using MyGrid.Models;
using BForms.Models;
using Newtonsoft.Json;

namespace MyGrid.Repositories
{
    public class MoviesGridRepository : BsBaseGridRepository<Movie, MoviesRowModel>
    {
        #region Properties and Constructor
        private BFormsContext db;

        
        public BsGridRepositorySettings<MoviesSearchModel> Settings
        {
            get
            {
                return settings as BsGridRepositorySettings<MoviesSearchModel>;
            }
        }

        public MoviesGridRepository(BFormsContext _db)
        {
            db = _db;
        }
        #endregion

        #region Mappers
        public Func<Movie, MoviesRowModel> MapMovie_MovieRowModel = x =>
            new MoviesRowModel
            {
                Id = x.Id,
                Title = x.Title,
                WeekendRevenue = x.WeekendRevenue,
                GrossRevenue = x.GrossRevenue,
                ReleaseDate = x.ReleaseDate,
                Recommended = x.IsRecommended
            };

        public Func<Movie, MovieDetailsModel> MapMovie_MovieDetailsModel = x =>
            new MovieDetailsModel
            {
                Id = x.Id,
                Title = x.Title,
                GrossRevenue = x.GrossRevenue,
                WeekendRevenue = x.WeekendRevenue,
                Recommended = x.IsRecommended,
                Rating = x.Rating,
                Genres = x.Genres,
                Poster = x.Poster,
                ReleaseDate = new BsDateTime()
                {
                    DateValue = x.ReleaseDate
                }
            };
        #endregion

        #region Query/Order/Map
        public override IQueryable<Movie> Query()
        {
            var query = db.Movies.AsQueryable();

            return Filter(query);
        }

        public IQueryable<Movie> Filter(IQueryable<Movie> query)
        {

            if (this.Settings != null)
            {
                if (!string.IsNullOrEmpty(Settings.QuickSearch))
                {
                    var searched = Settings.QuickSearch.ToLower();

                    var queryQuick = query.Where(x => x.Title.ToLower().Contains(searched) ||
                                                        x.Genres.ToLower().Contains(searched));
                    query = queryQuick.Select(x => x);
                }
                else if (this.Settings.Search != null)
                {
                    #region Release Date

                    if (this.Settings.Search.ReleaseDate != null)
                    {
                        var fromDate = this.Settings.Search.ReleaseDate.From;
                        var toDate = this.Settings.Search.ReleaseDate.To;

                        if (fromDate.ItemValue.HasValue)
                        {
                            query = query.Where(x => x.ReleaseDate >= fromDate.ItemValue.Value);
                        }
                        if (toDate.ItemValue.HasValue)
                        {
                            query = query.Where(x => x.ReleaseDate <= toDate.ItemValue.Value);
                        }
                    }

                    #endregion

                    #region Title

                    if (!string.IsNullOrEmpty(this.Settings.Search.Title))
                    {
                        var title = this.Settings.Search.Title.ToLower();
                        query = query.Where(x => x.Title.ToLower().Contains(title));
                    }

                    #endregion

                    #region Recommended

                    if (this.Settings.Search.Recommended.SelectedValues.HasValue)
                    {
                        var isEnabled = this.Settings.Search.Recommended.SelectedValues.Value;

                        if (isEnabled == YesNoValueTypes.Yes)
                        {
                            query = query.Where(x => x.IsRecommended);
                        }
                        else if (isEnabled == YesNoValueTypes.No)
                        {
                            query = query.Where(x => !x.IsRecommended);
                        }
                    }

                    #endregion
                }
            }        

            return query;
        }

        public override IOrderedQueryable<Movie> OrderQuery(IQueryable<Movie> query)
        {
            this.orderedQueryBuilder.OrderFor(x => x.Recommended, y => y.IsRecommended);

            var ordered = this.orderedQueryBuilder.Order(query, x => x.OrderByDescending(y => y.WeekendRevenue));

            return ordered;
        }

        public override IEnumerable<MoviesRowModel> MapQuery(IQueryable<Movie> query)
        {
            return query.Select(MapMovie_MovieRowModel);
        }

        public override void FillDetails(MoviesRowModel row)
        {
            row.Details = db.Movies.Where(x => x.Id == row.Id).Select(MapMovie_MovieDetailsModel).FirstOrDefault();

            if (row.Details != null)
            {
                FillDetailsProperties(row.Details);
            }
        }

        #endregion

        #region CRUD
        public MoviesRowModel ReadRow(int objId)
        {
            return db.Movies.Where(x => x.Id == objId).Select(MapMovie_MovieRowModel).FirstOrDefault();
        }

        public MoviesRowModel Create(MoviesNewModel model)
        {
            var entity = new Movie();

            if(model != null)
            {
                entity.Id = db.Movies.Count() + 1;
                if(model.Recommended.SelectedValues.HasValue)
                    entity.IsRecommended = model.Recommended.SelectedValues.Value == YesNoValueTypes.Yes ? true : false;
                entity.Title = model.Title;
                if (model.ReleaseDate.DateValue.HasValue)
                    entity.ReleaseDate = model.ReleaseDate.DateValue.Value;
                if (!string.IsNullOrEmpty(model.Poster))
                    entity.Poster = model.Poster;
                entity.GrossRevenue = model.GrossRevenue;
                entity.WeekendRevenue = model.WeekendRevenue;
                if (model.Rating.ItemValue.HasValue)
                    entity.Rating = (double)model.Rating.ItemValue.Value;
                entity.Genres = string.Join(",", model.GenresList.SelectedValues);
            };

            db.Movies.Add(entity);
            db.SaveChanges();

            return MapMovie_MovieRowModel(entity);
        }

        public MovieDetailsModel Update(MovieDetailsModel model, int objId, EditComponents componentId)
        {
            var entity = db.Movies.FirstOrDefault(x => x.Id == objId);

            if (entity != null)
            {
                switch (componentId)
                {
                    case EditComponents.Info:
                        entity.Title = model.Title;
                        entity.IsRecommended = model.IsRecommendedRadioButton.SelectedValues == YesNoValueTypes.Yes ? true : false;
                        if(!string.IsNullOrEmpty(model.Poster))
                            entity.Poster = model.Poster;
                        entity.Rating = model.Rating;
                        entity.Genres = string.Join(",",model.GenresList.SelectedValues);
                        break;
                    case EditComponents.Revenue:
                        entity.GrossRevenue = model.GrossRevenue;
                        entity.WeekendRevenue = model.WeekendRevenue;
                        if (model.ReleaseDate.DateValue.HasValue)
                            entity.ReleaseDate = model.ReleaseDate.DateValue.Value;
                        break;
                }
                db.SaveChanges();
            }

            return FillDetailsProperties(MapMovie_MovieDetailsModel(entity));
        }

        public List<MoviesRowModel> ReadRows(List<int> objIds)
        {
            return db.Movies.Where(x => objIds.Contains(x.Id)).Select(MapMovie_MovieRowModel).ToList();
        }

        public void RecommendUnrecommend(int objId, bool? recommended)
        {
            var entity = db.Movies.FirstOrDefault(x => x.Id == objId);

            if (entity != null)
            {
                entity.IsRecommended = recommended.HasValue ? recommended.Value : !entity.IsRecommended;
                db.SaveChanges();
            }
        }

        public void Delete(int objId)
        {
            var entity = db.Movies.FirstOrDefault(x => x.Id == objId);

            if (entity != null)
            {
                db.Movies.Remove(entity);
                db.SaveChanges();
            }
        }
        #endregion

        #region Helpers
        public MovieDetailsModel FillDetailsProperties(MovieDetailsModel detailsModel)
        {
            detailsModel.IsRecommendedRadioButton.SelectedValues = detailsModel.Recommended ? YesNoValueTypes.Yes : YesNoValueTypes.No;
            detailsModel.GenresList = new BsSelectList<List<int>>();
            detailsModel.GenresList.ItemsFromEnum(typeof(MovieGenre));
            detailsModel.GenresList.SelectedValues = new List<int>();
            var options = detailsModel.Genres.Split(',').ToList();
            foreach (string option in options)
            {
                MovieGenre genre;
                if (Enum.TryParse(option, true, out genre))
                    detailsModel.GenresList.SelectedValues.Add((int)genre);
            }

            return detailsModel;
        }

        public MoviesSearchModel GetSearchForm()
        {
            return new MoviesSearchModel
            {
                ReleaseDate = new BsRange<DateTime?>()
                {
                    From = new BsRangeItem<DateTime?>
                    {
                        ItemValue = new DateTime(2013, 1, 1)
                    },
                    To = new BsRangeItem<DateTime?>
                    {
                        ItemValue = DateTime.Now
                    }
                }
            };
        }

        public MoviesNewModel GetNewForm()
        {
            var genres = new BsSelectList<List<int>>();
            genres.ItemsFromEnum(typeof (MovieGenre));
            genres.SelectedValues = new List<int>();

            return new MoviesNewModel
            {
                ReleaseDate = new BsDateTime{ DateValue = DateTime.Now },
                GenresList = genres,
                Rating = new BsRangeItem<int?>
                {
                    ItemValue = 9,
                    MinValue = 0,
                    MaxValue = 10,
                    TextValue = "0-10",
                    Display = "Movie Rating"
                }
            };
        }

        #endregion
    }
}