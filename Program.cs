using Microsoft.EntityFrameworkCore;
using Quizadilla.Models;
using Quizadilla.Areas.Identity.Data; 
using Quizadilla.Data;                
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllersWithViews();


builder.Services.AddDbContext<QuizDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("QuizDbContextConnection"))
);


builder.Services.AddDbContext<AuthDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("AuthDbContextConnection"))
);


builder.Services.AddIdentity<QuizadillaUser, IdentityRole>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
})
.AddEntityFrameworkStores<AuthDbContext>()
.AddDefaultTokenProviders();


builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.HttpOnly = false;
    options.Cookie.SameSite = SameSiteMode.None;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
});


builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReact", policy =>
    {
        policy
            .WithOrigins("http://localhost:5173")  
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();                   
    });
});


builder.Services.AddScoped<IQuizRepository, QuizRepository>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var quizDb = scope.ServiceProvider.GetRequiredService<QuizDbContext>();
    //quizDb.Database.Migrate();
    

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


if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();


app.UseCors("AllowReact");
app.UseAuthentication();
app.UseAuthorization();


app.MapControllers();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}"
);

app.Run();
