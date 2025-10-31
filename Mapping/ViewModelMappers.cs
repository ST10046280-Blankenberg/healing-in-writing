using HealingInWriting.Domain.Books;
using HealingInWriting.Domain.Volunteers;
using HealingInWriting.Models.Books;
using HealingInWriting.Models.Volunteer;
using Microsoft.AspNetCore.Http;
using System.Text.Json;

namespace HealingInWriting.Mapping;

// TODO: Provide helper methods for shaping view models outside controllers.
public static class ViewModelMappers
{
    #region Helper Methods
    private static string AuthorsToString(List<string>? authors) =>
        authors == null ? string.Empty : string.Join(", ", authors.Where(a => !string.IsNullOrWhiteSpace(a)));

    private static List<string> StringToAuthors(string? authors) =>
        string.IsNullOrWhiteSpace(authors)
            ? new List<string>()
            : authors.Split(',').Select(a => a.Trim()).Where(a => !string.IsNullOrWhiteSpace(a)).ToList();

    private static List<string> IndustryIdentifiersToStrings(List<IndustryIdentifier>? ids) =>
        ids?.Select(i => i.Identifier).Where(i => !string.IsNullOrWhiteSpace(i)).ToList() ?? new List<string>();

    private static List<IndustryIdentifier> StringsToIndustryIdentifiers(List<string>? ids) =>
        ids?.Select(isbn => new IndustryIdentifier
        {
            Type = isbn.Length == 13 ? "ISBN_13" : "ISBN_10",
            Identifier = isbn
        }).ToList() ?? new List<IndustryIdentifier>();

    private static string GetThumbnailUrl(ImageLinks? links) =>
        links?.Thumbnail ?? links?.SmallThumbnail ?? "/images/placeholder-book.svg";

    private static List<string> ParseAuthors(object? value)
    {
        if (value is string s)
            return s.Split(',').Select(a => a.Trim()).Where(a => !string.IsNullOrWhiteSpace(a)).ToList();
        if (value is JsonElement authorsElem && authorsElem.ValueKind == JsonValueKind.Array)
            return authorsElem.EnumerateArray().Select(a => a.GetString() ?? "").Where(a => !string.IsNullOrWhiteSpace(a)).ToList();
        return new List<string>();
    }

    private static List<string> ParseCategories(object? value)
    {
        if (value is string s)
            return s.Split(',').Select(c => c.Trim()).Where(c => !string.IsNullOrWhiteSpace(c)).ToList();
        if (value is JsonElement catsElem && catsElem.ValueKind == JsonValueKind.Array)
            return catsElem.EnumerateArray().Select(c => c.GetString() ?? "").Where(c => !string.IsNullOrWhiteSpace(c)).ToList();
        return new List<string>();
    }

    private static List<IndustryIdentifier> ParseIndustryIdentifiers(object? value)
    {
        if (value is IEnumerable<string> isbns)
            return isbns.Select(isbn => new IndustryIdentifier
            {
                Type = isbn.Length == 13 ? "ISBN_13" : "ISBN_10",
                Identifier = isbn.Trim()
            }).ToList();
        if (value is JsonElement idsElem && idsElem.ValueKind == JsonValueKind.Array)
            return idsElem.EnumerateArray().Select(id =>
                new IndustryIdentifier
                {
                    Type = id.TryGetProperty("type", out var type) ? type.GetString() ?? "" : "",
                    Identifier = id.TryGetProperty("identifier", out var ident) ? ident.GetString() ?? "" : ""
                }).ToList();
        return new List<IndustryIdentifier>();
    }
    #endregion

    #region Books
    public static Book ToBookFromDetailViewModel(this BookDetailViewModel model)
    {
        return new Book
        {
            BookId = model.BookId,
            Title = model.Title,
            Authors = StringToAuthors(model.Authors),
            Publisher = model.Publisher,
            PublishedDate = model.PublishedDate,
            Description = model.Description,
            PageCount = model.PageCount,
            Categories = model.Categories ?? new List<string>(),
            Language = model.Language,
            IndustryIdentifiers = StringsToIndustryIdentifiers(model.IndustryIdentifiers),
            ImageLinks = new ImageLinks
            {
                Thumbnail = model.ThumbnailUrl,
                SmallThumbnail = model.ThumbnailUrl
            },
            Condition = model.Condition, // Use enum directly
            Price = model.Price
        };
    }

    public static Book ToBookFromGoogleJson(JsonElement volumeInfo)
    {
        return new Book
        {
            Title = volumeInfo.GetProperty("title").GetString() ?? "",
            Authors = ParseAuthors(volumeInfo.TryGetProperty("authors", out var authors) ? authors : null),
            Publisher = volumeInfo.TryGetProperty("publisher", out var publisher) ? publisher.GetString() ?? "" : "",
            PublishedDate = volumeInfo.TryGetProperty("publishedDate", out var pubDate) ? pubDate.GetString() ?? "" : "",
            Description = volumeInfo.TryGetProperty("description", out var desc) ? desc.GetString() ?? "" : "",
            PageCount = volumeInfo.TryGetProperty("pageCount", out var pageCount) ? pageCount.GetInt32() : 0,
            Categories = ParseCategories(volumeInfo.TryGetProperty("categories", out var cats) ? cats : null),
            Language = volumeInfo.TryGetProperty("language", out var lang) ? lang.GetString() ?? "" : "",
            ImageLinks = volumeInfo.TryGetProperty("imageLinks", out var imgLinks)
                ? new ImageLinks
                {
                    Thumbnail = imgLinks.TryGetProperty("thumbnail", out var thumb) ? thumb.GetString() ?? "" : "",
                    SmallThumbnail = imgLinks.TryGetProperty("smallThumbnail", out var smallThumb) ? smallThumb.GetString() ?? "" : ""
                }
                : new ImageLinks(),
            IndustryIdentifiers = ParseIndustryIdentifiers(volumeInfo.TryGetProperty("industryIdentifiers", out var ids) ? ids : null)
        };
    }

    public static Book ToBookFromForm(IFormCollection form)
    {
        return new Book
        {
            Title = form["Title"],
            Authors = ParseAuthors(form["Author"].ToString()),
            Publisher = form["Publisher"],
            PublishedDate = form["PublishDate"],
            Description = form["Description"],
            PageCount = int.TryParse(form["PageCount"], out var pc) ? pc : 0,
            Categories = ParseCategories(form["Categories"].ToString()),
            Language = form["Language"],
            IndustryIdentifiers = ParseIndustryIdentifiers(
                new[] { form["IsbnPrimary"].ToString(), form["IsbnSecondary"].ToString() }
                    .Where(isbn => !string.IsNullOrWhiteSpace(isbn))
            ),
            ImageLinks = new ImageLinks
            {
                Thumbnail = string.IsNullOrWhiteSpace(form["ThumbnailUrl"].ToString()) ? string.Empty : form["ThumbnailUrl"].ToString(),
                SmallThumbnail = string.IsNullOrWhiteSpace(form["SmallThumbnailUrl"].ToString()) ? string.Empty : form["SmallThumbnailUrl"].ToString()
            },
            // Parse Condition from form, default to Good if not valid or missing
            Condition = Enum.TryParse<BookCondition>(form["Condition"], out var cond) ? cond : BookCondition.Good,
            // Parse Price from form, default to 0 if not present or invalid
            Price = decimal.TryParse(form["Price"], out var price) ? price : 0m
        };
    }

    public static BookDetailViewModel ToBookDetailViewModel(this Book book)
    {
        if (book == null) return null!;

        return new BookDetailViewModel
        {
            BookId = book.BookId,
            Title = book.Title,
            Authors = AuthorsToString(book.Authors),
            PublishedDate = book.PublishedDate,
            Description = book.Description,
            Categories = book.Categories ?? new List<string>(),
            ThumbnailUrl = GetThumbnailUrl(book.ImageLinks),
            PageCount = book.PageCount,
            Language = book.Language,
            Publisher = book.Publisher,
            IndustryIdentifiers = IndustryIdentifiersToStrings(book.IndustryIdentifiers),
            Condition = book.Condition, // Use enum directly
            Price = book.Price
        };
    }

    public static BookSummaryViewModel ToBookSummaryViewModel(this Book book)
    {
        if (book == null) return null!;

        return new BookSummaryViewModel
        {
            BookId = book.BookId,
            Title = book.Title,
            Authors = AuthorsToString(book.Authors),
            PublishedDate = book.PublishedDate ?? string.Empty,
            Publisher = book.Publisher ?? string.Empty,
            PageCount = book.PageCount,
            Description = book.Description ?? string.Empty,
            Categories = book.Categories?.ToList() ?? new List<string>(),
            ThumbnailUrl = GetThumbnailUrl(book.ImageLinks)
        };
    }

    public static BookInventoryRowViewModel ToBookInventoryRowViewModel(Book book)
    {
        if (book == null) return null!;

        return new BookInventoryRowViewModel
        {
            BookId = book.BookId,
            Title = book.Title,
            Categories = book.Categories?.ToList() ?? new List<string>(),
            ThumbnailUrl = book.ImageLinks?.Thumbnail ?? string.Empty,
            Condition = book.Condition.ToString(),
            IsVisible = book.IsVisible,
            Authors = AuthorsToString(book.Authors)
        };
    }
    #endregion

    #region Volunteers
    public static VolunteerHour ToVolunteerHour(LogHoursViewModel model, int volunteerId, string? attachmentUrl)
    {
        return new VolunteerHour
        {
            VolunteerId = volunteerId,
            Date = model.Date,
            Activity = model.Activity,
            Hours = model.Hours,
            AttachmentUrl = attachmentUrl,
            Status = VolunteerHourStatus.Pending,
            SubmittedAt = DateTime.UtcNow,
            Comments = model.Notes
        };
    }
    public static VolunteerHourApprovalViewModel ToVolunteerHourApprovalViewModel(VolunteerHour hour)
    {
        return new VolunteerHourApprovalViewModel
        {
            Id = hour.Id,
            VolunteerName = hour.Volunteer?.User != null
                ? $"{hour.Volunteer.User.FirstName} {hour.Volunteer.User.LastName}"
                : "Unknown",
            VolunteerAvatarUrl = !string.IsNullOrEmpty(hour.Volunteer?.User?.FirstName)
                ? $"/images/volunteers/{hour.Volunteer.User.FirstName.ToLower()}.jpg"
                : "/images/volunteers/default.jpg",
            Date = hour.Date,
            Activity = hour.Activity,
            Hours = hour.Hours,
            AttachmentUrl = hour.AttachmentUrl,
            Status = hour.Status.ToString()
        };
    }

    public static VolunteerHourSummaryViewModel ToVolunteerHourSummaryViewModel(IEnumerable<VolunteerHour> hours)
    {
        return new VolunteerHourSummaryViewModel
        {
            TotalHours = hours.Sum(h => h.Hours),
            ValidatedHours = hours.Where(h => h.Status == VolunteerHourStatus.Approved).Sum(h => h.Hours),
            PendingHours = hours.Where(h => h.Status == VolunteerHourStatus.Pending).Sum(h => h.Hours),
            NeedsInfoHours = hours.Where(h => h.Status == VolunteerHourStatus.NeedsInfo).Sum(h => h.Hours)
        };
    }
    #endregion
}
