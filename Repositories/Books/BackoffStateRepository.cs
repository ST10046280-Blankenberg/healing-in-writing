using HealingInWriting.Domain.Books;
using HealingInWriting.Data;
using Microsoft.EntityFrameworkCore;

namespace HealingInWriting.Repositories.Books
{
    public class BackoffStateRepository : IBackoffStateRepository
    {
        private readonly ApplicationDbContext _context;

        public BackoffStateRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<BackoffState?> GetAsync()
        {
            // Always use Id = 1 for singleton/global backoff
            return await _context.BackoffStates.FirstOrDefaultAsync(b => b.Id == 1);
        }

        public async Task SaveAsync(BackoffState state)
        {
            var existing = await _context.BackoffStates.FirstOrDefaultAsync(b => b.Id == 1);
            if (existing == null)
            {
                state.Id = 1;
                _context.BackoffStates.Add(state);
            }
            else
            {
                existing.LastImportAttemptUtc = state.LastImportAttemptUtc;
                existing.CurrentBackoffSeconds = state.CurrentBackoffSeconds;
                _context.BackoffStates.Update(existing);
            }
            await _context.SaveChangesAsync();
        }
    }
}
