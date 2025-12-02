using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Quizadilla.Models;
using System;
using System.Linq;
using System.Collections.Generic;

namespace Quizadilla.Controllers;

public class QuizController : Controller
{
    private readonly IQuizRepository _repo;

    private static readonly string[] Themes = { "tomato", "guac", "cheese", "onion", "chicken", "salsa" };
    private static readonly Random Rng = new();

    public QuizController(IQuizRepository repo)
    {
        _repo = repo;
    }

    [HttpGet]
    public IActionResult Edit(int id)
    {
        var quiz = _repo.GetQuizForEdit(id);
        if (quiz == null) return NotFound();

        return View(quiz);
    }

    [HttpPost]
    public IActionResult Delete(int id)
    {
        _repo.DeleteQuiz(id);
        _repo.Save();
        return RedirectToAction("Discover");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(Quiz updatedQuiz)
    {
        if (!ModelState.IsValid)
            return View(updatedQuiz);

        _repo.UpdateQuiz(updatedQuiz);
        _repo.Save();

        return RedirectToAction("Discover");
    }

    public IActionResult Quiz(int id = 0)
    {
        var quiz = _repo.GetQuizWithDetails(id);
        if (quiz == null) return NotFound();

        var rng = new Random();
        foreach (var question in quiz.Questions)
            question.options = question.options.OrderBy(o => rng.Next()).ToList();

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
        foreach (var q in quiz.Questions ?? new List<Question>())
        {
            q.options ??= new List<Option>();

            var correct = (q.correctString ?? "").Trim();
            if (!string.IsNullOrWhiteSpace(correct))
            {
                var hasCorrect = q.options.Any(o =>
                    string.Equals((o.OptionText ?? "").Trim(), correct, StringComparison.OrdinalIgnoreCase));

                if (!hasCorrect)
                    q.options.Add(new Option { OptionText = q.correctString ?? "" });
            }
        }

        if (string.IsNullOrWhiteSpace(quiz.Theme))
            quiz.Theme = Themes[Rng.Next(Themes.Length)];

        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (userId == null) return BadRequest();

        quiz.UserID = userId;

        _repo.AddQuiz(quiz);
        _repo.Save();

        return RedirectToAction("MyQuizzes");
    }

    public IActionResult Discover()
    {
        var quizzes = _repo.GetQuizzes();
        return View(quizzes);
    }

    [Authorize]
    public IActionResult MyQuizzes()
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (userId == null) return BadRequest();

        var quizzes = _repo.GetUserQuizzes(userId);
        return View(quizzes);
    }

    public IActionResult Search(string needle)
    {
        if (needle == null)
        {
            throw new ArgumentNullException("The search term was empty");
        }
        var quizzes = _repo.GetQuizzes();
        var result = new List<Quiz>();
        foreach (Quiz quiz in quizzes)
        {
            if (quiz.Title.Contains(needle))
            {
                result.Add(quiz);
            }
        }
        return View(result);


    }
}
