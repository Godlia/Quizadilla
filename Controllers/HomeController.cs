using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Quizadilla.Models;

namespace Quizadilla.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly QuizDbContext db;

    public HomeController(ILogger<HomeController> logger, QuizDbContext context)
    {
        _logger = logger;
        db = context;
    }

    public IActionResult Index()
    {
        var quizzes = db.Quizzes.Include(q => q.Questions).ToList();
        return View(quizzes);
    }

    public IActionResult Privacy() => View();
    public IActionResult Test() => View();
    public IActionResult MyAccount() => View();
    public IActionResult Quizzes() => View();

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
