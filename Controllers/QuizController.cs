using Quizadilla.Models;
using Quizadilla.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Quizadilla.Controllers;

public class QuizController : Controller
{
    private readonly IQuizService _quizService;

    public QuizController(IQuizService quizService)
    {
        _quizService = quizService;
    }

    [HttpGet]
    public IActionResult Edit(int id)
    {
        var quiz = _quizService.GetQuizForEdit(id);
        if (quiz == null)
            return NotFound();

        return View(quiz);
    }

    [HttpPost]
    public IActionResult Delete(int id)
    {
        var ok = _quizService.DeleteQuiz(id);
        if (!ok)
            return NotFound();

        return RedirectToAction("Discover");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(Quiz updatedQuiz)
    {
        if (!ModelState.IsValid)
            return View(updatedQuiz);

        var result = _quizService.UpdateQuiz(updatedQuiz);
        if (result == null)
            return NotFound();

        return RedirectToAction("Discover");
    }

    public IActionResult Quiz(int id = 0)
    {
        var quiz = _quizService.GetQuizForPlay(id);
        if (quiz == null)
            return NotFound();

        return View(quiz);
    }

    public IActionResult Index()
    {
        return View();
    }

    [Authorize]
    public IActionResult Create()
    {
        return View();
    }

    [Authorize]
    [HttpPost]
    public IActionResult CreateQuiz(Quiz quiz)
    {
        if (!ModelState.IsValid)
            return View("Create", quiz);

        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (userId == null)
        {
            return BadRequest();
        }

        _quizService.CreateQuiz(quiz, userId);
        return RedirectToAction("MyQuizzes");
    }

    public IActionResult Discover()
    {
        var quizzes = _quizService.GetAllQuizzes();

        foreach (var quiz in quizzes)
        {
            Console.WriteLine(quiz.toString());
        }

        return View(quizzes);
    }

    [Authorize]
    public IActionResult MyQuizzes()
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (userId == null)
        {
            return BadRequest();
        }

        var quizzes = _quizService.GetQuizzesForUser(userId);
        return View(quizzes);
    }
}
