# Controller Guidelines

This document provides practical guidelines for writing controllers in accordance with the project's layered architecture. Controllers should be **thin orchestration layers** that delegate all business logic to services.

## Core Principle

> **Controllers: Never own business logic or data access.**

Controllers are responsible for:
- Handling HTTP concerns (routing, request/response)
- Input validation
- Calling service methods
- Returning views or results
- Setting TempData for user feedback

Controllers should **NOT**:
- Contain business rules or calculations
- Directly manipulate domain entities
- Perform data transformations
- Build complex view models
- Contain helper methods with business logic

## Examples

### ❌ Bad: Business Logic in Controller

```csharp
[HttpPost]
public async Task<IActionResult> SetVisibility([FromBody] SetVisibilityRequest request)
{
    if (request == null)
        return BadRequest();

    // ❌ BAD: Fetching entity and modifying it directly in controller
    var book = await _bookService.GetBookByIdAsync(request.BookId);
    if (book == null)
        return NotFound();

    book.IsVisible = request.IsVisible;  // ❌ Business logic in controller
    await _bookService.UpdateBookAsync(book);

    return Ok();
}
```

### ✅ Good: Delegating to Service

```csharp
[HttpPost]
public async Task<IActionResult> SetVisibility([FromBody] SetVisibilityRequest request)
{
    if (request == null)
        return BadRequest();

    // ✅ GOOD: Delegate business logic to service
    var success = await _bookService.SetBookVisibilityAsync(request.BookId, request.IsVisible);

    if (!success)
        return NotFound();

    return Ok();
}
```

**Service Layer:**
```csharp
public async Task<bool> SetBookVisibilityAsync(int bookId, bool isVisible)
{
    var book = await _bookRepository.GetByIdAsync(bookId);
    if (book == null)
        return false;

    book.IsVisible = isVisible;  // Business logic in service
    await _bookRepository.UpdateAsync(book);
    return true;
}
```

---

### ❌ Bad: Complex Logic in Controller

```csharp
public async Task<IActionResult> Manage(string? status)
{
    var stories = await _storyService.GetAllStoriesForAdminAsync();

    // ❌ BAD: Complex name resolution logic in controller
    var storyViewModels = stories.Select(story => new AdminStoryListItemViewModel
    {
        StoryId = story.StoryId,
        Title = story.Title,
        AuthorName = story.IsAnonymous ? "Anonymous"
            : !string.IsNullOrWhiteSpace(story.Author?.User?.FirstName)
                ? $"{story.Author.User.FirstName} {story.Author.User.LastName}"
                : story.Author?.User?.Email ?? story.Author?.UserId ?? "Unknown"
    }).ToList();

    // ❌ BAD: Building dropdown options in controller
    var statusOptions = Enum.GetValues<StoryStatus>()
        .Select(s => new AdminSelectOption(
            s.ToString(),
            s.ToString(),
            s.ToString().Equals(status, StringComparison.OrdinalIgnoreCase)))
        .ToList();

    return View(new ManageViewModel { Stories = storyViewModels, StatusOptions = statusOptions });
}
```

### ✅ Good: Service Handles Logic

```csharp
public async Task<IActionResult> Manage(string? status)
{
    var stories = await _storyService.GetAllStoriesForAdminAsync();

    // ✅ GOOD: Delegate mapping and transformations to service
    var storyViewModels = stories.Select(story => new AdminStoryListItemViewModel
    {
        StoryId = story.StoryId,
        Title = story.Title,
        AuthorName = _storyService.ResolveAuthorName(story),  // Service method
        StatusBadgeClass = _storyService.GetStatusBadgeClass(story.Status)  // Service method
    }).ToList();

    var statusOptions = _storyService.BuildStatusOptions(status);  // Service method

    return View(new ManageViewModel { Stories = storyViewModels, StatusOptions = statusOptions });
}
```

---

### ❌ Bad: File Operations in Controller

```csharp
[HttpPost]
public async Task<IActionResult> AddGalleryItem(List<IFormFile> images, string altText)
{
    int successCount = 0;
    int failCount = 0;

    // ❌ BAD: Complex loop with business logic in controller
    foreach (var image in images)
    {
        if (image.Length == 0) continue;

        try
        {
            // ❌ File upload orchestration in controller
            var imageUrl = await _blobStorageService.UploadImageAsync(image, "gallery", isPublic: true);

            var entity = new GalleryItem
            {
                ImageUrl = imageUrl,
                AltText = altText,
                CreatedDate = DateTime.UtcNow
            };

            await _galleryService.AddAsync(entity, User.Identity?.Name ?? "System");
            successCount++;
        }
        catch (Exception ex)
        {
            failCount++;
        }
    }

    TempData["GallerySuccess"] = $"{successCount} photo(s) added successfully.";
    return RedirectToAction("Index");
}
```

### ✅ Good: Service Handles File Operations

```csharp
[HttpPost]
public async Task<IActionResult> AddGalleryItem(List<IFormFile> images, string altText)
{
    // Input validation only
    if (images == null || images.Count == 0)
    {
        TempData["GalleryError"] = "Please select at least one image to upload.";
        return RedirectToAction("Index");
    }

    // ✅ GOOD: Delegate complex logic to service
    var (successCount, failCount, lastError) = await _galleryService.AddMultipleGalleryItemsAsync(
        images,
        altText,
        isAlbum: false,
        albumPhotoCount: null,
        collectionId: null,
        User.Identity?.Name ?? "System");

    // User feedback only
    if (successCount > 0)
    {
        TempData["GallerySuccess"] = $"{successCount} photo(s) added successfully.";
    }

    return RedirectToAction("Index");
}
```

---

## Common Patterns to Refactor

### 1. Helper Methods in Controllers

If you have private helper methods in a controller, they likely belong in a service:

```csharp
// ❌ BAD: In Controller
private static string ResolveAuthorName(Story story) { /* ... */ }
private static Dictionary<StoryStatus, int> CalculateStatusCounts(IEnumerable<Story> stories) { /* ... */ }
private static List<AdminSelectOption> BuildStatusOptions(string? selectedStatus) { /* ... */ }
```

**Solution:** Move to service interface and implementation:

```csharp
// ✅ GOOD: In IStoryService
string ResolveAuthorName(Story story);
Dictionary<StoryStatus, int> CalculateStatusCounts(IEnumerable<Story> stories);
List<AdminSelectOption> BuildStatusOptions(string? selectedStatus);
```

### 2. Direct Entity Manipulation

If you're fetching an entity and modifying it in the controller, create a service method:

```csharp
// ❌ BAD
var entity = await _service.GetByIdAsync(id);
entity.Property = newValue;
await _service.UpdateAsync(entity);
```

**Solution:**

```csharp
// ✅ GOOD: In Controller
await _service.UpdatePropertyAsync(id, newValue);

// ✅ GOOD: In Service
public async Task<bool> UpdatePropertyAsync(int id, string newValue)
{
    var entity = await _repository.GetByIdAsync(id);
    if (entity == null) return false;

    entity.Property = newValue;
    await _repository.UpdateAsync(entity);
    return true;
}
```

### 3. Complex Loops and Conditionals

If you have loops with error tracking, conditionals, or orchestration logic:

```csharp
// ❌ BAD: Complex loop in controller
foreach (var item in items)
{
    try
    {
        // Multiple operations
        // Error tracking
        // Conditionals
    }
    catch { /* ... */ }
}
```

**Solution:** Extract to service method that returns structured results:

```csharp
// ✅ GOOD: Service returns tuple with results
var (successCount, failCount, errors) = await _service.ProcessItemsAsync(items);
```

### 4. Building Dropdown/Select Options

Dropdown builders are presentation logic and belong in services:

```csharp
// ❌ BAD: In Controller
var options = Enum.GetValues<Status>()
    .Select(s => new SelectListItem { /* ... */ })
    .ToList();
```

**Solution:**

```csharp
// ✅ GOOD: In Controller
var options = _service.BuildStatusOptions(selectedStatus);

// ✅ GOOD: In Service
public List<AdminSelectOption> BuildStatusOptions(string? selectedStatus)
{
    return Enum.GetValues<Status>()
        .Select(s => new AdminSelectOption(
            s.ToString(),
            s.ToString(),
            s.ToString().Equals(selectedStatus, StringComparison.OrdinalIgnoreCase)))
        .ToList();
}
```

---

## Checklist for New Controller Actions

Before committing a controller action, verify:

- [ ] No private helper methods with business logic
- [ ] No direct domain entity manipulation
- [ ] No complex loops or conditionals
- [ ] No data transformations or calculations
- [ ] No dropdown/select list building
- [ ] No file operations beyond validation
- [ ] All business logic delegated to services
- [ ] Only HTTP concerns and user feedback remain

---

## What CAN Stay in Controllers?

Some logic is acceptable in controllers:

### ✅ Input Validation
```csharp
if (string.IsNullOrWhiteSpace(searchTerm))
{
    return BadRequest("Search term is required.");
}
```

### ✅ Simple Mapping to View Models (when no business logic)
```csharp
var viewModel = new SimpleViewModel
{
    Id = entity.Id,
    Name = entity.Name  // Direct property mapping
};
```

### ✅ Pagination Parameters
```csharp
const int pageSize = 10;
var skip = (page - 1) * pageSize;
```

### ✅ Setting TempData
```csharp
TempData["SuccessMessage"] = "Operation completed successfully.";
```

### ✅ Infrastructure Concerns
```csharp
// View rendering helpers (tightly coupled to ASP.NET MVC)
private async Task<string> RenderPartialViewToStringAsync(string viewName, object model)
{
    // This is infrastructure code, can stay in controller
}
```

---

## Benefits of This Approach

1. **Testability:** Services can be unit tested without HTTP concerns
2. **Reusability:** Business logic in services can be called from multiple controllers
3. **Maintainability:** Changes to business rules happen in one place
4. **Clarity:** Controllers show the flow, services show the logic
5. **Scalability:** Easy to add new features without bloating controllers

---

## Related Documentation

- [ARCHITECTURE.md](./ARCHITECTURE.md) - Overall architecture principles
- [TESTING.md](./TESTING.md) - Testing guidelines (if exists)

---

**Last Updated:** 2025-12-03