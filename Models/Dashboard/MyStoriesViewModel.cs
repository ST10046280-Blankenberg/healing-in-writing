using HealingInWriting.Domain.Stories;
using HealingInWriting.Models.Filters;

namespace HealingInWriting.Models.Dashboard
{
    public class MyStoriesViewModel
    {
        public IReadOnlyCollection<Story> Stories { get; set; }
        public StoriesFilterViewModel Filter { get; set; }
    }
}
