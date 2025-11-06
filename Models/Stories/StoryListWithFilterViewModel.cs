using HealingInWriting.Models.Filters;

namespace HealingInWriting.Models.Stories
{
    public class StoryListWithFilterViewModel
    {
        public StoryListViewModel StoryList { get; set; }
        public StoriesFilterViewModel Filter { get; set; }
    }
}