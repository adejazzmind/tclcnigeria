using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using tclcnigeria.Models;
using tclcnigeria.Services;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddDataProtection()
    .PersistKeysToDbContext<ApplicationDbContext>()
    .SetApplicationName("tclcnigeria");

builder.Services.AddDefaultIdentity<IdentityUser>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;
    options.SignIn.RequireConfirmedAccount = false;
})
.AddRoles<IdentityRole>()
.AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddHttpClient();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddHttpClient<IAiService, AiService>();
builder.Services.AddTransient<IEmailSender, IdentityEmailSender>();

builder.Services.AddAntiforgery(options => {
    options.Cookie.Name = ".tclc.antiforgery";
    options.Cookie.SameSite = SameSiteMode.Lax;
    options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
    options.Cookie.HttpOnly = true;
});

builder.Services.ConfigureApplicationCookie(options => {
    options.Cookie.Name = ".tclc.auth";
    options.Cookie.SameSite = SameSiteMode.Lax;
    options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

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
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

    string[] roles = { "BibleSchoolStaff", "CTGStaff", "SuperAdmin", "MediaStaff" };
    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(new IdentityRole(role));
        }
    }

    async Task EnsureStaffAsync(string email, string tempPassword)
    {
        var user = await userManager.FindByEmailAsync(email);
        if (user == null)
        {
            user = new IdentityUser { UserName = email, Email = email, EmailConfirmed = true };
            await userManager.CreateAsync(user, tempPassword);
        }
        if (!await userManager.IsInRoleAsync(user, "BibleSchoolStaff"))
            await userManager.AddToRoleAsync(user, "BibleSchoolStaff");
        if (!await userManager.IsInRoleAsync(user, "CTGStaff"))
            await userManager.AddToRoleAsync(user, "CTGStaff");
    }

    async Task EnsureSuperAdminAsync(string email, string tempPassword)
    {
        var user = await userManager.FindByEmailAsync(email);
        if (user == null)
        {
            user = new IdentityUser { UserName = email, Email = email, EmailConfirmed = true };
            await userManager.CreateAsync(user, tempPassword);
        }
        if (!await userManager.IsInRoleAsync(user, "SuperAdmin"))
            await userManager.AddToRoleAsync(user, "SuperAdmin");
    }

    async Task EnsureMediaStaffAsync(string email, string tempPassword)
    {
        var user = await userManager.FindByEmailAsync(email);
        if (user == null)
        {
            user = new IdentityUser { UserName = email, Email = email, EmailConfirmed = true };
            await userManager.CreateAsync(user, tempPassword);
        }
        if (!await userManager.IsInRoleAsync(user, "MediaStaff"))
            await userManager.AddToRoleAsync(user, "MediaStaff");
    }

    await EnsureStaffAsync("adejazzmind@gmail.com", "Tclc@2026!");
    await EnsureStaffAsync("foluwalade@gmail.com", "TclcFunmi@2026!");
    await EnsureSuperAdminAsync("adejazzmind@gmail.com", "Tclc@2026!");
    await EnsureSuperAdminAsync("foluwalade@gmail.com", "TclcFunmi@2026!");
    await EnsureMediaStaffAsync("lanreayoaye@gmail.com", "TclcMedia@2026!");
}

app.MapGet("/.well-known/assetlinks.json", () => Results.Text("""
[{"relation":["delegate_permission/common.handle_all_urls"],"target":{"namespace":"android_app","package_name":"com.tclcnigeria.twa","sha256_cert_fingerprints":["39:90:5B:F9:1B:8B:8D:4B:B0:AA:E9:A8:53:36:28:41:3E:6F:4B:82:DD:EC:82:9F:5D:0A:96:54:96:F2:15:9C","C1:A2:C4:38:E3:70:0A:2E:4C:90:F4:53:D9:7B:97:C8:C1:77:D5:96:13:8A:33:A8:62:09:E9:8F:AC:74:C0:8A"]}}]
""", "application/json"));
app.Run();



