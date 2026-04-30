using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using tclcnigeria.Models;

var builder = WebApplication.CreateBuilder(args);

// --- 1. Add Services to the Container ---

// Connection string will be pulled from appsettings.json locally 
// or from Environment Variables on Render
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// Switched from SQL Server to PostgreSQL for Neon Compatibility
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddDefaultIdentity<IdentityUser>(options => {
    options.Password.RequireDigit = false;
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;
})
.AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

var app = builder.Build();

// --- 2. Database Auto-Migration ---
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var db = services.GetRequiredService<ApplicationDbContext>();
        // This is CRITICAL for Render/Neon: It creates your tables automatically on the cloud
        db.Database.Migrate();
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred during startup migration.");
    }
}

// --- 3. Configure the HTTP Request Pipeline ---
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.MapStaticAssets();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.MapRazorPages();

app.Run();