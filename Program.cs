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
builder.Services.AddScoped<IStoryService, StoryService>();
builder.Services.AddScoped<IBookService, BookService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IBookRepository, BookRepository>();

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

// Seed the database with test accounts
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        //TODO: Remove the following two lines in production
        // --- Drop and recreate the database (for dev/testing only) ---
        context.Database.EnsureDeleted();
        context.Database.EnsureCreated();
        // --- End drop/recreate ---

        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

        await DbInitialiser.InitialiseAsync(context, userManager, roleManager);

        // --- Seed books at startup ---
        var bookService = services.GetRequiredService<IBookService>() as BookService;
        if (bookService != null)
        {
            await bookService.SeedBooksAsync();
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
    // Inline styles are allowed (used throughout the site) but inline scripts are blocked
    var csp = string.Join("; ", new[]
    {
        "default-src 'self'",
        "script-src 'self'",
        "style-src 'self' 'unsafe-inline' https://fonts.googleapis.com",
        "font-src 'self' https://fonts.gstatic.com",
        "img-src 'self' data: https:",
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

app.MapControllerRoute(
    name: "admin",
    pattern: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
