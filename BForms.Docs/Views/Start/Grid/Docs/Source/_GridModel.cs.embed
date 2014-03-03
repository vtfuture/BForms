using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using BForms.Models;
using BForms.Mvc;
using MyGrid.Mock;

namespace MyGrid.Models
{
    public class MoviesRowModel : BsGridRowModel<MovieDetailsModel>
    {
        public int Id { get; set; }

        [BsGridColumn(Width = 4, IsEditable = true, IsSortable = true)]
        public string Title { get; set; }

        [BsGridColumn(Width = 2, IsEditable = false, IsSortable = true)]
        public decimal WeekendRevenue { get; set; }

        [BsGridColumn(Width = 2, IsEditable = false, IsSortable = false)]
        public decimal GrossRevenue { get; set; }

        [BsGridColumn(Width = 2, IsEditable = false, IsSortable = true)]
        public DateTime ReleaseDate { get; set; }

        [BsGridColumn(Width = 2, IsEditable = false, IsSortable = true)]
        public bool Recommended { get; set; }

        public override object GetUniqueID()
        {
            return Id;
        }
    }

    public class MoviesViewModel
    {
        [BsGrid(HasDetails = true, Theme = BsTheme.Blue)]
        [Display(Name = "Top Movies")]
        public BsGridModel<MoviesRowModel> Grid { get; set; }

        [BsToolbar(Theme = BsTheme.Black)]
        [Display(Name = "Top Movies")]
        public BsToolbarModel<MoviesSearchModel, MoviesNewModel> Toolbar { get; set; }
    }

    public class MoviesSearchModel
    {
        public MoviesSearchModel()
        {
            Recommended = new BsSelectList<YesNoValueTypes?>();
            Recommended.ItemsFromEnum(typeof(YesNoValueTypes));
            Recommended.SelectedValues = YesNoValueTypes.Both;
        }

        [Display(Name = "Title")]
        [BsControl(BsControlType.TextBox)]
        public string Title { get; set; }

        [Display(Name = "Release Date Interval")]
        [BsControl(BsControlType.DatePickerRange)]
        public BsRange<DateTime?> ReleaseDate { get; set; }

        [BsControl(BsControlType.RadioButtonList)]
        [Display(Name = "Recommended")]
        public BsSelectList<YesNoValueTypes?> Recommended { get; set; }
    }

    public class MoviesNewModel
    {
        public MoviesNewModel()
        {
            Recommended = new BsSelectList<YesNoValueTypes?>();
            Recommended.ItemsFromEnum(typeof(YesNoValueTypes), YesNoValueTypes.Both);
            Recommended.SelectedValues = YesNoValueTypes.Yes; 
        }

        [Required]
        [Display(Name = "Movie Title")]
        [BsControl(BsControlType.TextBox)]
        public string Title { get; set; }

        [Display(Name = "Weekend Revenue")]
        [BsControl(BsControlType.TextBox)]
        public decimal WeekendRevenue { get; set; }

        [Display(Name = "Gross Revenue")]
        [BsControl(BsControlType.TextBox)]
        public decimal GrossRevenue { get; set; }

        [Required]
        [Display(Name = "Release Date")]
        [BsControl(BsControlType.DateTimePicker)]
        public BsDateTime ReleaseDate { get; set; }

        [Display(Name = "Genres", Prompt = "Select movie genres")]
        [BsControl(BsControlType.ListBox)]
        public BsSelectList<List<int>> GenresList { get; set; }

        [Display(Name = "Rating")]
        [BsControl(BsControlType.NumberInline)]
        public BsRangeItem<int?> Rating { get; set; }

        [Required]
        [Display(Name = "Recommended")]
        [BsControl(BsControlType.RadioButtonList)]
        public BsSelectList<YesNoValueTypes?> Recommended { get; set; }

        [Display(Name = "Poster Url", Prompt = "http://site.com/image.png")]
        [BsControl(BsControlType.Url)]
        public string Poster { get; set; }
    }

    public class MovieDetailsModel
    {
        public MovieDetailsModel()
        {
            IsRecommendedRadioButton = new BsSelectList<YesNoValueTypes>();
            IsRecommendedRadioButton.ItemsFromEnum(typeof(YesNoValueTypes), YesNoValueTypes.Both);
            IsRecommendedRadioButton.SelectedValues = YesNoValueTypes.Both;
        }

        public int Id { get; set; }

        [Required]
        [Display(Name = "Title")]
        [BsControl(BsControlType.TextBox)]
        public string Title { get; set; }

        [Required]
        [Display(Name = "Weekend Revenue")]
        [BsControl(BsControlType.Number)]
        public decimal WeekendRevenue { get; set; }

        [Display(Name = "Gross Revenue")]
        [BsControl(BsControlType.Number)]
        public decimal GrossRevenue { get; set; }

        [Required]
        [Display(Name = "Release Date")]
        [BsControl(BsControlType.DatePicker)]
        public BsDateTime ReleaseDate { get; set; }

        [Display(Name = "Genres")]
        [BsControl(BsControlType.ListBox)]
        public BsSelectList<List<int>> GenresList { get; set; }
        public string Genres { get; set; }

        [Display(Name = "Rating")]
        [BsControl(BsControlType.Number)]
        public double Rating { get; set; }

        [Required]
        [Display(Name = "Recomended")]
        [BsControl(BsControlType.RadioButtonList)]
        public BsSelectList<YesNoValueTypes> IsRecommendedRadioButton { get; set; }
        public bool Recommended { get; set; }

        [Display(Name = "Poster Image")]
        [BsControl(BsControlType.Url)]
        public string Poster { get; set; }
    }

    public enum EditComponents
    {
        Info = 1,
        Revenue = 2
    }
}