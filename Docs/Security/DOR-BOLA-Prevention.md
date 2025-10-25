# Direct Object Reference (DOR) / Broken Object Level Authorisation (BOLA) Prevention

## Overview

DOR/BOLA vulnerabilities occur when an application exposes a reference to an internal object (like a database ID) and fails to verify that the requesting user is authorised to access it.

**Example Attack:**
- User A submits a story and gets ID 123
- User B changes the URL from `/Stories/Edit/456` to `/Stories/Edit/123`
- Without proper checks, User B can now edit User A's story

## Current Protection Status

### ✅ Protected Endpoints

1. **AdminController** - All actions restricted to Admin role
   ```csharp
   [Authorize(Roles = "Admin")]
   public class AdminController : Controller
   ```

2. **BooksController.ImportBookByIsbn** - Restricted to Admin role
   ```csharp
   [Authorize(Roles = "Admin")]
   [HttpGet]
   public async Task<IActionResult> ImportBookByIsbn(string isbn)
   ```

### ⚠️ Requires Implementation

1. **StoriesController.Details** - Needs authorisation checks for non-published stories
2. **Future story edit/delete endpoints** - Must verify ownership

## Prevention Guidelines

### Rule 1: Never Trust User-Provided IDs

Always validate that the current user is authorised to access the resource.

❌ **Bad Example:**
```csharp
public async Task<IActionResult> Edit(int id)
{
    var story = await _storyService.GetByIdAsync(id);
    // Anyone can edit any story by changing the ID!
    return View(story);
}
```

✅ **Good Example:**
```csharp
[Authorize]
public async Task<IActionResult> Edit(int id)
{
    var story = await _storyService.GetByIdAsync(id);

    if (story == null)
        return NotFound();  // Return 404, not 403, to prevent enumeration

    // Check if user owns the story or is an admin
    var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    var isAdmin = User.IsInRole("Admin");

    if (story.UserId != userId && !isAdmin)
        return NotFound();  // Return 404 to prevent resource enumeration

    return View(story);
}
```

### Rule 2: Set Ownership Server-Side

Never trust client-provided ownership information.

❌ **Bad Example:**
```csharp
[HttpPost]
public async Task<IActionResult> Create(Story story)
{
    // Attacker could set UserId to someone else's ID!
    await _storyService.CreateAsync(story);
    return RedirectToAction("Index");
}
```

✅ **Good Example:**
```csharp
[HttpPost]
[Authorize]
public async Task<IActionResult> Create(CreateStoryViewModel model)
{
    var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    if (string.IsNullOrEmpty(userId))
        return Unauthorized();

    var story = new Story
    {
        Title = model.Title,
        Content = model.Content,
        UserId = userId,  // CRITICAL: Set server-side from authenticated user
        CreatedAt = DateTime.UtcNow,  // CRITICAL: Set server-side
        Status = StoryStatus.Pending
    };

    await _storyService.CreateAsync(story);
    return RedirectToAction("Index");
}
```

### Rule 3: Implement Resource-Based Authorisation

For complex scenarios, use ASP.NET Core's resource-based authorisation.

```csharp
public class StoryAuthorizationHandler : AuthorizationHandler<OperationAuthorizationRequirement, Story>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        OperationAuthorizationRequirement requirement,
        Story resource)
    {
        var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (requirement.Name == "Edit" || requirement.Name == "Delete")
        {
            // Allow if user owns the story or is an admin
            if (resource.UserId == userId || context.User.IsInRole("Admin"))
            {
                context.Succeed(requirement);
            }
        }

        return Task.CompletedTask;
    }
}
```

Usage:
```csharp
[Authorize]
public async Task<IActionResult> Edit(int id)
{
    var story = await _storyService.GetByIdAsync(id);
    if (story == null)
        return NotFound();

    var authResult = await _authorizationService.AuthorizeAsync(
        User, story, "Edit");

    if (!authResult.Succeeded)
        return NotFound();  // Return 404, not 403

    return View(story);
}
```

### Rule 4: Return 404, Not 403

When authorisation fails, return `NotFound()` (404) instead of `Forbid()` (403) to prevent resource enumeration.

**Why?**
- 403 confirms the resource exists but user can't access it
- 404 makes it ambiguous whether the resource exists at all
- Prevents attackers from enumerating valid IDs

❌ **Bad:**
```csharp
if (story.UserId != userId)
    return Forbid();  // Tells attacker the story exists!
```

✅ **Good:**
```csharp
if (story.UserId != userId && !isAdmin)
    return NotFound();  // Ambiguous - could be missing or forbidden
```

### Rule 5: Use Role-Based Access Control (RBAC)

For administrative functions, always use `[Authorize(Roles = "...")]`:

```csharp
// Entire controller restricted to Admin
[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    // All actions automatically protected
}

// Specific action restricted
[Authorize(Roles = "Admin,Moderator")]
public async Task<IActionResult> Approve(int id)
{
    // Only admins and moderators can access
}
```

### Rule 6: Check Permissions in Service Layer Too

Don't rely solely on controller checks. Add defensive checks in services:

```csharp
public class StoryService : IStoryService
{
    public async Task<bool> UpdateAsync(Story story, string requestingUserId, bool isAdmin)
    {
        var existing = await _repository.GetByIdAsync(story.StoryId);

        if (existing == null)
            return false;

        // Double-check authorisation in service layer
        if (existing.UserId != requestingUserId && !isAdmin)
            throw new UnauthorizedAccessException("User not authorised to update this story");

        existing.Title = story.Title;
        existing.Content = story.Content;
        existing.UpdatedAt = DateTime.UtcNow;

        await _repository.UpdateAsync(existing);
        return true;
    }
}
```

## Implementation Checklist

When implementing new features with user-owned resources:

- [ ] Add `[Authorize]` attribute to controller or actions
- [ ] Retrieve current user ID from `User.FindFirst(ClaimTypes.NameIdentifier)?.Value`
- [ ] Check if resource exists (`null` check)
- [ ] Check if user owns resource or has admin role
- [ ] Return `NotFound()` if unauthorised (not `Forbid()`)
- [ ] Set ownership server-side (UserId, CreatedAt, etc.)
- [ ] Never trust client-provided IDs for ownership
- [ ] Add service-layer authorisation checks
- [ ] Log authorisation failures for security monitoring
- [ ] Test with different user accounts

## Common Vulnerable Endpoints

Watch for these patterns:

1. **Edit/Update endpoints** - `/Stories/Edit/{id}`
2. **Delete endpoints** - `/Stories/Delete/{id}`
3. **View private data** - `/Users/Profile/{id}` (viewing someone else's profile)
4. **Download attachments** - `/Documents/Download/{id}`
5. **API endpoints** - `/api/stories/{id}`

## Testing for BOLA Vulnerabilities

1. Create two test accounts (User A and User B)
2. User A creates a resource (story, profile, etc.)
3. Note the resource ID
4. Log in as User B
5. Try to access User A's resource by changing the ID in the URL
6. Verify User B gets a 404 (not the resource)

## Additional Resources

- [OWASP API Security Top 10 - Broken Object Level Authorization](https://owasp.org/API-Security/editions/2023/en/0xa1-broken-object-level-authorization/)
- [ASP.NET Core Authorization Documentation](https://learn.microsoft.com/en-us/aspnet/core/security/authorization/)
- [Resource-Based Authorization](https://learn.microsoft.com/en-us/aspnet/core/security/authorization/resourcebased)
