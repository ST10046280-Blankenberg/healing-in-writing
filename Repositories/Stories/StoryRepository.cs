using HealingInWriting.Data;
using HealingInWriting.Domain.Stories;
using HealingInWriting.Interfaces.Repository;
using Microsoft.EntityFrameworkCore;

namespace HealingInWriting.Repositories.Stories;

public class StoryRepository : IStoryRepository
{
    private readonly ApplicationDbContext _context;

    public StoryRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public Task AddAsync(Story story)
    {
        throw new NotImplementedException();
    }

    public Task DeleteAsync(int storyId)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<Story>> GetAllAsync()
    {
        throw new NotImplementedException();
    }

    public Task<Story?> GetByIdAsync(int storyId)
    {
        throw new NotImplementedException();
    }

    public Task UpdateAsync(Story story)
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<Story>> GetFilteredUserStoriesAsync(
        int profileId,
        string? searchText,
        string? selectedDate,
        string? selectedSort,
        StoryCategory? selectedCategory)
    {
        var query = _context.Stories
            .Include(s => s.Tags)
            .Include(s => s.Author)
            .Where(s => s.UserId == profileId);

        if (!string.IsNullOrWhiteSpace(searchText))
        {
            query = query.Where(s =>
                (!string.IsNullOrEmpty(s.Title) && s.Title.Contains(searchText)) ||
                (!string.IsNullOrEmpty(s.Summary) && s.Summary.Contains(searchText)));
        }

        if (selectedCategory.HasValue)
        {
            query = query.Where(s => s.Category == selectedCategory.Value);
        }

        if (!string.IsNullOrWhiteSpace(selectedDate))
        {
            var now = DateTime.UtcNow;
            if (selectedDate == "Last 7 Days")
                query = query.Where(s => s.CreatedAt >= now.AddDays(-7));
            else if (selectedDate == "Last 30 Days")
                query = query.Where(s => s.CreatedAt >= now.AddDays(-30));
            else if (selectedDate == "This Year")
                query = query.Where(s => s.CreatedAt.Year == now.Year);
        }

        if (selectedSort == "Oldest")
            query = query.OrderBy(s => s.CreatedAt);
        else
            query = query.OrderByDescending(s => s.CreatedAt);

        return await query.ToListAsync();
    }
}
