using System;
using System.ComponentModel.DataAnnotations;

namespace HealingInWriting.Domain.Books;

public class BackoffState
{
    [Key]
    public int Id { get; set; } = 1; // Always 1 for singleton/global backoff
    public DateTimeOffset? LastImportAttemptUtc { get; set; }
    public double CurrentBackoffSeconds { get; set; }
}