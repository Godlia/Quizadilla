using Microsoft.EntityFrameworkCore;
using Quizadilla.Models;
using Microsoft.AspNetCore.Identity;
using Quizadilla.Data;
using Quizadilla.Areas.Identity.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<QuizDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("QuizDbContextConnection"))
);
builder.Services.AddDbContext<AuthDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("AuthDbContextConnection"))
);

builder.Services.AddScoped<IQuizRepository, QuizRepository>();

builder.Services.AddDefaultIdentity<QuizadillaUser>(options => options.SignIn.RequireConfirmedAccount = false).AddEntityFrameworkStores<AuthDbContext>();

builder.Services.AddRazorPages();


var app = builder.Build();

// Apply migrations and seed sample data

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<QuizDbContext>();
    db.Database.Migrate();


    // Only seed if no quizzes exist
    if (!db.Quizzes.Any())
    {
        var json = System.IO.File.ReadAllText("Data/TemplateData/seed.json");
        var sampleQuizzes = System.Text.Json.JsonSerializer.Deserialize<List<Quiz>>(json);
        if (sampleQuizzes != null)
        {
            db.Quizzes.AddRange(sampleQuizzes);
            db.SaveChanges();
            Console.WriteLine(" Sample quizzes added to the database.");
        }
    }
}


// Configure the HTTP request pipeline
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

