using AutoMarket.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// ===============================
// Database
// ===============================
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? "Server=(localdb)\\mssqllocaldb;Database=AutoMarketDb;Trusted_Connection=True;MultipleActiveResultSets=true";

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// ===============================
// Identity + Roles
// ===============================
builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
{
    // За обучение/тест: да можеш да логваш веднага без email confirmation
    options.SignIn.RequireConfirmedAccount = false;

    // (по желание) по-лесни пароли докато тестваш:
    // options.Password.RequiredLength = 6;
    // options.Password.RequireNonAlphanumeric = false;
    // options.Password.RequireUppercase = false;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

// ===============================
// MVC + Razor Pages (Identity UI)
// ===============================
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

var app = builder.Build();

// ===============================
// Apply migrations (create/update DB)
// ===============================
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    db.Database.Migrate();
}

// ===============================
// Middleware pipeline
// ===============================
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// 🔥 ЗАДЪЛЖИТЕЛНО за Identity
app.UseAuthentication();
app.UseAuthorization();

// ===============================
// Routes
// ===============================
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// 🔥 ЗАДЪЛЖИТЕЛНО за /Identity/Account/Login и Register
app.MapRazorPages();

// ===============================
// Seed roles + users (по новия модел)
// ===============================
await IdentitySeed.SeedAsync(app.Services);

app.Run();

