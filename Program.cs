using HealingInWriting.Data;
using HealingInWriting.Domain.Users;
using HealingInWriting.Interfaces.Services;
using HealingInWriting.Services.Auth;
using HealingInWriting.Services.Books;
using HealingInWriting.Services.Stories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.RateLimiting;
using HealingInWriting.Interfaces.Repository;
using HealingInWriting.Repositories.Books;
using HealingInWriting.Services.Common;
using HealingInWriting.Repositories.BankDetailsFolder;
using System.Globalization;
using HealingInWriting.Repositories.Events;
using HealingInWriting.Services.Events;
using Microsoft.AspNetCore.Localization;

var builder = WebApplication.CreateBuilder(args);

// Add database context with environment-based provider switching
// Development: SQLite for local work
// Production: Azure SQL Server for cloud deployment
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    if (builder.Environment.IsDevelopment())
    {
        options.UseSqlite(connectionString);
    }
    else
    {
        options.UseSqlServer(connectionString);
    }
});

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

// Azure Blob Storage for image uploads (optional, configured via StorageConnection)
// Register clients only if connection string is present
var storageConnectionString = builder.Configuration.GetConnectionString("StorageConnection");
if (!string.IsNullOrEmpty(storageConnectionString))
{
    builder.Services.AddSingleton(sp =>
    {
        return new Azure.Storage.Blobs.BlobServiceClient(storageConnectionString);
    });
    builder.Services.AddScoped(sp =>
    {
        var blobServiceClient = sp.GetRequiredService<Azure.Storage.Blobs.BlobServiceClient>();
        var containerName = builder.Configuration["Blob:Container"] ?? "uploads";
        return blobServiceClient.GetBlobContainerClient(containerName);
    });
}

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

// Azure deployment toggles from environment variables
// RUN_MIGRATIONS: Set to "true" to apply pending migrations on startup (use cautiously in production)
// MAINTENANCE: Set to "on" to show maintenance page whilst deploying or troubleshooting
var runMigrations = builder.Configuration["RUN_MIGRATIONS"] == "true";
var maintenanceMode = builder.Configuration["MAINTENANCE"] == "on";

// Set default culture to en-US for all requests
var supportedCultures = new[] { new CultureInfo("en-US") };
app.UseRequestLocalization(new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture("en-US"),
    SupportedCultures = supportedCultures,
    SupportedUICultures = supportedCultures
});

// Safe startup: handle migrations and seeding based on environment
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();
    var context = services.GetRequiredService<ApplicationDbContext>();

    try
    {
        // Development: Automatic database creation and migration
        if (app.Environment.IsDevelopment())
        {
            logger.LogInformation("Development mode: Ensuring database is created...");
            context.Database.EnsureCreated();

            // Apply any pending migrations
            var pendingMigrations = context.Database.GetPendingMigrations().ToList();
            if (pendingMigrations.Any())
            {
                logger.LogInformation("Applying {Count} pending migrations: {Migrations}",
                    pendingMigrations.Count, string.Join(", ", pendingMigrations));
                context.Database.Migrate();
                logger.LogInformation("Migrations applied successfully.");
            }
        }
        // Production/Azure: Only migrate if explicitly enabled via RUN_MIGRATIONS toggle
        else if (runMigrations)
        {
            logger.LogInformation("RUN_MIGRATIONS=true: Applying pending migrations...");
            context.Database.Migrate();
            logger.LogInformation("Production migrations applied successfully.");
        }
        else
        {
            logger.LogInformation("Skipping migrations (RUN_MIGRATIONS not set). Database assumed ready.");
        }

        // Seed test accounts, roles, and demo data (only in development)
        if (app.Environment.IsDevelopment())
        {
            var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
            await DbInitialiser.InitialiseAsync(context, userManager, roleManager);

            // Seed books from Google Books API (development only, avoid rate limits in production)
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
        }
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "An error occurred whilst seeding the database.");
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

// Health check endpoint - does not access database, always responds
// Used by Azure App Service health monitoring and load balancers
app.MapGet("/healthz", () => Results.Ok("OK"));

// Maintenance mode - show holding page when deploying or troubleshooting
// Controlled via MAINTENANCE=on environment variable in Azure
if (maintenanceMode)
{
    app.MapGet("/", () => Results.Content(
        "<h1>Healing in Writing</h1><p>We are preparing the next update.</p>",
        "text/html"));
    app.MapFallback(() => Results.Redirect("/"));
}
else
{
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
}

app.Run();
