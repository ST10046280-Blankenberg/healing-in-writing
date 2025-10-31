using HealingInWriting.Data;
using HealingInWriting.Domain.Users;
using HealingInWriting.Interfaces.Repository;
using HealingInWriting.Interfaces.Services;
using HealingInWriting.Repositories.BankDetailsFolder;
using HealingInWriting.Repositories.Books;
using HealingInWriting.Repositories.Events;
using HealingInWriting.Repositories.Stories;
using HealingInWriting.Repositories.Volunteers;
using HealingInWriting.Services.Auth;
using HealingInWriting.Services.Books;
using HealingInWriting.Services.Common;
using HealingInWriting.Services.Events;
using HealingInWriting.Services.Stories;
using HealingInWriting.Services.Volunteers;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

// Add database context with SQLite
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add ASP.NET Core Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    // Password settings
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequiredLength = 8;

    // Lockout settings
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;

    // User settings
    options.User.RequireUniqueEmail = true;

    // Sign-in settings
    options.SignIn.RequireConfirmedEmail = true;
    options.SignIn.RequireConfirmedAccount = false;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders()
.AddClaimsPrincipalFactory<ApplicationUserClaimsPrincipalFactory>();

// Configure cookie settings for session security
// Protects against session fixation, session jacking, and CSRF attacks
builder.Services.ConfigureApplicationCookie(options =>
{
    // HttpOnly prevents JavaScript access to cookies (XSS protection)
    options.Cookie.HttpOnly = true;

    // Secure flag ensures cookies only sent over HTTPS (prevents MITM attacks)
    // Set to true in production once HTTPS is configured
    options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;

    // SameSite prevents cookies being sent with cross-site requests (CSRF protection)
    // Lax allows cookies on safe HTTP methods (GET) from external sites
    // Strict would block all cross-site cookie sending (more secure but may break workflows)
    options.Cookie.SameSite = SameSiteMode.Lax;

    // Custom cookie name for security through obscurity
    // Makes it less obvious which technology stack is being used
    options.Cookie.Name = "HIW.Auth";

    // Mark as essential for GDPR compliance (authentication cookies are necessary)
    options.Cookie.IsEssential = true;

    // Session timeout and renewal settings
    options.ExpireTimeSpan = TimeSpan.FromHours(24);
    options.SlidingExpiration = true; // Renews session on activity (prevents fixation)

    // Paths for authentication flows
    options.LoginPath = "/Auth/Login";
    options.LogoutPath = "/Auth/Logout";
    options.AccessDeniedPath = "/Auth/AccessDenied";
});

// Configure antiforgery token cookies for CSRF protection
builder.Services.AddAntiforgery(options =>
{
    // Secure CSRF token cookie with same protections as auth cookie
    options.Cookie.HttpOnly = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
    options.Cookie.SameSite = SameSiteMode.Strict; // Strict for CSRF tokens
    options.Cookie.Name = "HIW.CSRF";
});

// Add services to the container
builder.Services.AddControllersWithViews();
builder.Services.AddHttpClient();
builder.Services.AddScoped<IStoryService, StoryService>();
builder.Services.AddScoped<IStoryRepository, StoryRepository>();
builder.Services.AddScoped<IBookService, BookService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IBookRepository, BookRepository>();
builder.Services.AddScoped<IEventRepository, EventRepository>();
builder.Services.AddScoped<IEventService, EventService>();
builder.Services.AddScoped<IRegistrationRepository, RegistrationRepository>();
builder.Services.AddScoped<IRegistrationService, RegistrationService>();
builder.Services.AddScoped<IBackoffStateRepository, BackoffStateRepository>();
builder.Services.AddScoped<IBankDetailsRepository, BankDetailsRepository>();
builder.Services.AddScoped<IBankDetailsService, BankDetailsService>();
builder.Services.AddScoped<IVolunteerRepository, VolunteerRepository>();
builder.Services.AddScoped<IVolunteerService, VolunteerService>();

// Configure rate limiting to prevent brute force, credential stuffing, and DDoS attacks
// Uses IP address as the partition key to track requests per client
builder.Services.AddRateLimiter(options =>
{
    // Reject requests that exceed the limit with 429 Too Many Requests
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

    // Strict policy for authentication endpoints to prevent brute force attacks
    // Allows 5 login/register attempts per 15 minutes per IP address
    options.AddFixedWindowLimiter("authentication", limiterOptions =>
    {
        limiterOptions.PermitLimit = 5;
        limiterOptions.Window = TimeSpan.FromMinutes(15);
        limiterOptions.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        limiterOptions.QueueLimit = 0; // No queueing, reject immediately when limit reached
    });

    // Standard policy for form submissions and API requests
    // Allows 20 requests per minute per IP address
    options.AddFixedWindowLimiter("standard", limiterOptions =>
    {
        limiterOptions.PermitLimit = 20;
        limiterOptions.Window = TimeSpan.FromMinutes(1);
        limiterOptions.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        limiterOptions.QueueLimit = 0;
    });

    // Lenient policy for general page views and read operations
    // Allows 100 requests per minute per IP address
    options.AddFixedWindowLimiter("lenient", limiterOptions =>
    {
        limiterOptions.PermitLimit = 100;
        limiterOptions.Window = TimeSpan.FromMinutes(1);
        limiterOptions.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        limiterOptions.QueueLimit = 0;
    });

    // Global fallback limiter for endpoints without specific policies
    // Prevents general abuse across the entire application
    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
    {
        // Use IP address as partition key
        var ipAddress = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";

        return RateLimitPartition.GetFixedWindowLimiter(ipAddress, _ =>
            new FixedWindowRateLimiterOptions
            {
                PermitLimit = 100,
                Window = TimeSpan.FromMinutes(1),
                QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                QueueLimit = 0
            });
    });
});

var app = builder.Build();

// Set default culture to en-US for all requests
var supportedCultures = new[] { new CultureInfo("en-US") };
app.UseRequestLocalization(new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture("en-US"),
    SupportedCultures = supportedCultures,
    SupportedUICultures = supportedCultures
});

// Seed the database with test accounts
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        var context = services.GetRequiredService<ApplicationDbContext>();

        // Automatically handle database migrations on startup
        // This ensures database schema stays in sync with code changes
        // Prevents "table does not exist" or "column does not exist" errors
        try
        {
            var pendingMigrations = context.Database.GetPendingMigrations().ToList();
            var appliedMigrations = context.Database.GetAppliedMigrations().ToList();

            logger.LogInformation($"Applied migrations: {appliedMigrations.Count}, Pending migrations: {pendingMigrations.Count}");

            if (pendingMigrations.Any())
            {
                logger.LogInformation("Applying pending migrations: {Migrations}", string.Join(", ", pendingMigrations));
                context.Database.Migrate();
                logger.LogInformation("Migrations applied successfully.");
            }
            else
            {
                logger.LogInformation("Database is up to date. No pending migrations.");

                // Validate that database can be accessed (catches schema mismatch issues)
                try
                {
                    _ = context.Books.Any();
                    logger.LogInformation("Database schema validation successful.");
                }
                catch (Exception validationEx)
                {
                    logger.LogWarning(validationEx, "Database schema validation failed. Attempting to recreate database...");

                    // Only in development: recreate database on schema mismatch
                    if (app.Environment.IsDevelopment())
                    {
                        logger.LogWarning("Development mode: Dropping and recreating database...");
                        context.Database.EnsureDeleted();
                        context.Database.Migrate();
                        logger.LogInformation("Database recreated successfully.");
                    }
                    else
                    {
                        logger.LogError("Production mode: Cannot auto-fix schema mismatch. Manual intervention required.");
                        throw;
                    }
                }
            }
        }
        catch (Exception migrationEx)
        {
            logger.LogError(migrationEx, "Failed to apply migrations.");

            // In development, offer nuclear option: drop and recreate
            if (app.Environment.IsDevelopment())
            {
                logger.LogWarning("Development mode: Attempting to recreate database from scratch...");
                try
                {
                    context.Database.EnsureDeleted();
                    context.Database.Migrate();
                    logger.LogInformation("Database recreated successfully after migration failure.");
                }
                catch (Exception recreateEx)
                {
                    logger.LogError(recreateEx, "Failed to recreate database. Manual intervention required.");
                    throw;
                }
            }
            else
            {
                throw;
            }
        }

        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

        await DbInitialiser.InitialiseAsync(context, userManager, roleManager);

        // --- Seed books at startup (only if database is empty) ---
        var bookService = services.GetRequiredService<IBookService>() as BookService;
        if (bookService != null)
        {
            var existingBooks = await bookService.GetPagedForAdminAsync(
                searchTerm: null,
                selectedAuthor: null,
                selectedCategory: null,
                selectedTag: null,
                skip: 0,
                take: 20);
            if (!existingBooks.Any())
            {
                logger.LogInformation("Database is empty. Seeding books...");
                await bookService.SeedBooksAsync();
                logger.LogInformation("Book seeding completed.");
            }
            else
            {
                logger.LogInformation("Books already exist in database. Skipping seeding.");
            }
        }
        // --- End book seeding ---
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while seeding the database.");
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

// Security headers to prevent XSS, clickjacking, and other attacks
app.Use(async (context, next) =>
{
    // Prevent clickjacking by blocking iframe embedding
    context.Response.Headers.Append("X-Frame-Options", "DENY");

    // Content Security Policy to prevent XSS attacks
    // Restricts what resources can be loaded and from where
    // 'self' allows resources from same origin only
    // Google Fonts domains are explicitly allowed for typography
    // jsDelivr CDN is allowed for Quill rich text editor
    // Inline styles are allowed (used throughout the site) but inline scripts are blocked
    var csp = string.Join("; ", new[]
    {
        "default-src 'self'",
        "script-src 'self' https://cdn.jsdelivr.net",
        "style-src 'self' 'unsafe-inline' https://fonts.googleapis.com https://cdn.jsdelivr.net",
        
        // 1. ADDED https://cdn.jsdelivr.net to allow Font Awesome fonts
        "font-src 'self' https://fonts.gstatic.com https://cdn.jsdelivr.net", 
        
        // 2. ADDED http://books.google.com to allow book cover images
        "img-src 'self' data: https: http://books.google.com",

        "connect-src 'self'",
        "frame-ancestors 'none'",
        "base-uri 'self'",
        "form-action 'self'"
    });
    context.Response.Headers.Append("Content-Security-Policy", csp);

    // Prevent MIME type sniffing which could lead to XSS
    // Forces browser to respect declared content types
    context.Response.Headers.Append("X-Content-Type-Options", "nosniff");

    // Control how much referrer information is shared
    // Protects user privacy whilst maintaining analytics capability
    context.Response.Headers.Append("Referrer-Policy", "strict-origin-when-cross-origin");

    // Restrict access to browser features and APIs
    // Disables potentially dangerous features the site doesn't need
    context.Response.Headers.Append("Permissions-Policy", "geolocation=(), microphone=(), camera=()");

    await next();
});

app.UseStaticFiles();

app.UseRouting();

// Enable rate limiting middleware
// Must be placed after UseRouting() and before UseAuthentication()
app.UseRateLimiter();

app.UseAuthentication();
app.UseAuthorization();

// Admin Area routes - explicit pattern for proper form tag helper resolution
// Using explicit "Admin" instead of {area:exists} fixes POST form generation issues
app.MapControllerRoute(
    name: "admin",
    pattern: "Admin/{controller=Dashboard}/{action=Index}/{id?}",
    defaults: new { area = "Admin" });

// Default route for non-area controllers
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
