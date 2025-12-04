using Microsoft.EntityFrameworkCore;
using Quizadilla.Models;
using Quizadilla.Areas.Identity.Data; // for QuizadillaUser
using Quizadilla.Data;                // for AuthDbContext
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

// -----------------------------
// 1. MVC Controllers + Views
// -----------------------------
builder.Services.AddControllersWithViews();

// -----------------------------
// 2. Quiz database
// -----------------------------
builder.Services.AddDbContext<QuizDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("QuizDbContextConnection"))
);

// -----------------------------
// 3. Identity database for users
// -----------------------------
builder.Services.AddDbContext<AuthDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("AuthDbContextConnection"))
);

// -----------------------------
// 4. ASP.NET Identity
// -----------------------------
builder.Services.AddIdentity<QuizadillaUser, IdentityRole>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
})
.AddEntityFrameworkStores<AuthDbContext>()
.AddDefaultTokenProviders();

// -----------------------------
// 5. Cookie settings for React
// -----------------------------
builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.HttpOnly = false;
    options.Cookie.SameSite = SameSiteMode.None;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
});

// -----------------------------
// 6. CORS for React + cookies
// -----------------------------
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReact", policy =>
    {
        policy
            .WithOrigins("http://localhost:5173")  // Vite
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();                   // IMPORTANT
    });
});

// -----------------------------
// 7. Repos
// -----------------------------
builder.Services.AddScoped<IQuizRepository, QuizRepository>();

var app = builder.Build();

// -----------------------------
// 8. Database migrations
// -----------------------------
using (var scope = app.Services.CreateScope())
{
    var quizDb = scope.ServiceProvider.GetRequiredService<QuizDbContext>();
    quizDb.Database.Migrate();

    if (!quizDb.Quizzes.Any())
    {
        var json = File.ReadAllText("Data/TemplateData/seed.json");
        var sampleQuizzes = System.Text.Json.JsonSerializer.Deserialize<List<Quiz>>(json);
        if (sampleQuizzes != null)
        {
            quizDb.Quizzes.AddRange(sampleQuizzes);
            quizDb.SaveChanges();
        }
    }
}

// -----------------------------
// 9. Pipeline
// -----------------------------
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// ORDER MATTERS!
app.UseCors("AllowReact");
app.UseAuthentication();
app.UseAuthorization();

// -----------------------------
// 10. Map routes n controllers
// -----------------------------
app.MapControllers();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}"
);

app.Run();
