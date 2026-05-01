using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using tclcnigeria.Models;

var builder = WebApplication.CreateBuilder(args);

// --- 1. Add Services to the Container ---

// Connection string pulled from appsettings.json locally
// or from Environment Variables on Render
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// PostgreSQL for Neon compatibility
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddDefaultIdentity<IdentityUser>(options => {
    options.Password.RequireDigit = false;
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;
    options.SignIn.RequireConfirmedAccount = false; // Allows login without email confirmation
})
.AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

var app = builder.Build();

// --- 2. Database Auto-Migration ---
// Try/catch removed so migration failures crash loudly and are visible in logs
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    db.Database.Migrate();
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