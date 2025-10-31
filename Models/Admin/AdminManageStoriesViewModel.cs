using HealingInWriting.Domain.Stories;

namespace HealingInWriting.Models.Admin;

public class AdminManageStoriesViewModel
{
    public IReadOnlyCollection<AdminStoryListItemViewModel> Stories { get; set; }
        = Array.Empty<AdminStoryListItemViewModel>();

    public AdminManageStoriesFilters Filters { get; set; } = new();

    public IReadOnlyCollection<AdminSelectOption> StatusOptions { get; set; }
        = Array.Empty<AdminSelectOption>();

    public IReadOnlyCollection<AdminSelectOption> DateOptions { get; set; }
        = Array.Empty<AdminSelectOption>();

    public IReadOnlyCollection<AdminSelectOption> TagOptions { get; set; }
        = Array.Empty<AdminSelectOption>();

    public IReadOnlyCollection<AdminSelectOption> SortOptions { get; set; }
        = Array.Empty<AdminSelectOption>();

    public int PendingCount { get; set; }

    public int PublishedCount { get; set; }

    public int DraftCount { get; set; }

    public int RejectedCount { get; set; }

    public int TotalStories { get; set; }

    public int CurrentPage { get; set; } = 1;

    public int TotalPages { get; set; } = 1;

    public int PageSize { get; set; } = 10;
}

public class AdminManageStoriesFilters
{
    public string? SearchTerm { get; set; }

    public string? Status { get; set; }

    public string? DateRange { get; set; }

    public string? Tag { get; set; }

    public string SortOrder { get; set; } = "newest";

    public int Page { get; set; } = 1;
}

public class AdminStoryListItemViewModel
{
    public int StoryId { get; init; }

    public string Title { get; init; } = string.Empty;

    public string AuthorName { get; init; } = string.Empty;

    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;

    public StoryStatus Status { get; init; } = StoryStatus.Submitted;

    public string StatusText { get; init; } = string.Empty;

    public string StatusBadgeClass { get; init; } = string.Empty;
}

public record AdminSelectOption(string Value, string Label, bool Selected);
