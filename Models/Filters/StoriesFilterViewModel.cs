using HealingInWriting.Domain.Stories;

namespace HealingInWriting.Models.Filters
{
    public class StoriesFilterViewModel : BaseFilterViewModel
    {
        public List<string> DateOptions { get; set; } = new();
        public List<string> SortOptions { get; set; } = new();
        public List<StoryCategory> CategoryOptions { get; set; } = new();

        // Selected values
        public string? SelectedDate { get; set; }
        public string? SelectedSort { get; set; }
        public StoryCategory? SelectedCategory { get; set; }
    }
}
