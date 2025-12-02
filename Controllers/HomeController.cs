using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Quizadilla.Models;

namespace Quizadilla.Controllers;

public class HomeController : Controller
{
    private readonly IQuizRepository _repo;
    private readonly ILogger<HomeController> _logger;
    private readonly QuizDbContext db;

    public HomeController(ILogger<HomeController> logger, QuizDbContext context, IQuizRepository repo)
    {
        _logger = logger;
        db = context;
        _repo = repo;
    }

    public IActionResult Index()
    {
        var quizzes = _repo.GetQuizzes();
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
