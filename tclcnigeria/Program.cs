using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using tclcnigeria.Models;
using tclcnigeria.Services;

var builder = WebApplication.CreateBuilder(args);

// Connection string
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

// PostgreSQL / Neon
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddDefaultIdentity<IdentityUser>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;
    options.SignIn.RequireConfirmedAccount = false;
})
.AddEntityFrameworkStores<ApplicationDbContext>();

// ✅ Email Service
builder.Services.AddScoped<IEmailService, EmailService>();

// ✅ Identity Email Sender (for password reset + confirmation)
builder.Services.AddScoped<IEmailSender<IdentityUser>, IdentityEmailSender>();
builder.Services.AddScoped<IEmailSender, IdentityEmailSender>();

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

var app = builder.Build();

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