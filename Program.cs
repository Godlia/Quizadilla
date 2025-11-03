using Microsoft.EntityFrameworkCore;
using Quizadilla.Models;
using Microsoft.AspNetCore.Identity;
using Quizadilla.Data;
using Quizadilla.Areas.Identity.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<QuizDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("QuizDbContextConnection"))
);
builder.Services.AddDbContext<AuthDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("AuthDbContextConnection"))
);

builder.Services.AddDefaultIdentity<QuizadillaUser>(options => options.SignIn.RequireConfirmedAccount = true).AddEntityFrameworkStores<AuthDbContext>();

builder.Services.AddRazorPages();

var app = builder.Build();

// Apply migrations at startup (create a scope first)
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<QuizDbContext>();
    db.Database.Migrate();
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "Quiz",
    pattern: "{controller=Quiz}/{action=Index}/{id?}");

app.MapRazorPages();

app.Run();
