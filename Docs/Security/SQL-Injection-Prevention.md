# SQL Injection Prevention Guidelines

## Current Protection Status

✅ **Protected** - The application currently has no SQL injection vulnerabilities.

### What's in Place
- ASP.NET Core Identity handles all user authentication queries using parameterised queries internally
- No raw SQL queries are used anywhere in the codebase
- Entity Framework Core automatically parameterises all LINQ queries
- No controllers directly access the database context
- Repository pattern is defined but not yet implemented

## Guidelines for Future Development

When implementing repositories and database access, follow these rules strictly:

### ✅ Safe Practices

#### 1. Use LINQ Queries (Preferred)
```csharp
// Safe: EF Core automatically parameterises LINQ queries
var stories = await _context.Stories
    .Where(s => s.UserId == userId)
    .ToListAsync();
```

#### 2. Use Parameterised Raw SQL (When Raw SQL is Necessary)
```csharp
// Safe: Parameters are passed separately
var stories = await _context.Stories
    .FromSqlRaw("SELECT * FROM Stories WHERE UserId = {0}", userId)
    .ToListAsync();
```

#### 3. Use SQL Parameters Explicitly
```csharp
// Safe: Using SqlParameter objects
var userIdParam = new SqliteParameter("@userId", userId);
var stories = await _context.Stories
    .FromSqlRaw("SELECT * FROM Stories WHERE UserId = @userId", userIdParam)
    .ToListAsync();
```

### ❌ Unsafe Practices (NEVER DO THIS)

#### 1. String Concatenation in SQL
```csharp
// DANGEROUS: Allows SQL injection
var sql = "SELECT * FROM Stories WHERE UserId = " + userId;
var stories = await _context.Stories.FromSqlRaw(sql).ToListAsync();
```

#### 2. String Interpolation in FromSqlRaw
```csharp
// DANGEROUS: String interpolation happens before parameterisation
var stories = await _context.Stories
    .FromSqlRaw($"SELECT * FROM Stories WHERE UserId = {userId}")
    .ToListAsync();
```

#### 3. Building SQL from User Input
```csharp
// DANGEROUS: User input directly in SQL string
var sql = $"SELECT * FROM Stories WHERE Title LIKE '%{searchTerm}%'";
var stories = await _context.Stories.FromSqlRaw(sql).ToListAsync();
```

## Special Considerations

### Dynamic Queries
If you need to build dynamic queries based on search criteria, use LINQ's conditional Where clauses:

```csharp
// Safe approach for dynamic queries
var query = _context.Stories.AsQueryable();

if (!string.IsNullOrWhiteSpace(searchTerm))
    query = query.Where(s => s.Title.Contains(searchTerm));

if (userId.HasValue)
    query = query.Where(s => s.UserId == userId.Value);

var stories = await query.ToListAsync();
```

### Stored Procedures
If using stored procedures, always use parameterised calls:

```csharp
// Safe: Parameters passed separately
await _context.Database
    .ExecuteSqlRawAsync("EXEC GetStoriesByUser @userId",
        new SqliteParameter("@userId", userId));
```

### Dynamic Column/Table Names
If you absolutely must use dynamic table or column names (rare cases), use an allowlist approach:

```csharp
// Safe: Validate against allowlist before using
var allowedSortColumns = new[] { "Title", "CreatedAt", "UpdatedAt" };
if (!allowedSortColumns.Contains(sortColumn))
    throw new ArgumentException("Invalid sort column");

var sql = $"SELECT * FROM Stories ORDER BY {sortColumn}";
var stories = await _context.Stories.FromSqlRaw(sql).ToListAsync();
```

## Testing for SQL Injection

When implementing database access, test with these malicious inputs:

- `' OR '1'='1`
- `'; DROP TABLE Users; --`
- `1'; DELETE FROM Stories WHERE '1'='1`
- `admin'--`
- `' UNION SELECT * FROM Users --`

All should be safely handled as data values, not SQL commands.

## Code Review Checklist

Before merging any database access code, verify:

- [ ] No string concatenation used in SQL queries
- [ ] No string interpolation used with `FromSqlRaw` or `ExecuteSqlRaw`
- [ ] All user inputs are passed as parameters
- [ ] LINQ queries are preferred over raw SQL where possible
- [ ] Any raw SQL uses parameterised queries
- [ ] Dynamic column/table names use allowlist validation
- [ ] Code has been tested with SQL injection payloads

## Additional Resources

- [OWASP SQL Injection Prevention Cheat Sheet](https://cheatsheetseries.owasp.org/cheatsheets/SQL_Injection_Prevention_Cheat_Sheet.html)
- [EF Core Raw SQL Queries](https://learn.microsoft.com/en-us/ef/core/querying/sql-queries)
- [Parameterised Queries in .NET](https://learn.microsoft.com/en-us/dotnet/api/system.data.sqlclient.sqlcommand.parameters)
