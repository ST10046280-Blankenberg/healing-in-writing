using HealingInWriting.Domain.Books;

public interface IBackoffStateRepository
{
    Task<BackoffState?> GetAsync();
    Task SaveAsync(BackoffState state);
}