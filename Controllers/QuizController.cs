using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Quizadilla.Models;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;

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
    public IActionResult EditQuiz(Quiz updatedQuiz)
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        var oldQuiz = _repo.GetQuizWithDetails(updatedQuiz.QuizId);
        if (userId == null) return Unauthorized();
        if (oldQuiz == null || oldQuiz.UserID != userId) return Unauthorized();


        if (!ModelState.IsValid)
        {
            Console.WriteLine("ModelState is invalid");
            // Ensure collections the view iterates aren't null to avoid rendering errors
            updatedQuiz.Questions ??= new List<Question>();
            foreach (var q in updatedQuiz.Questions)
                q.Options ??= new List<Option>();
            try
            {
                _repo.UpdateQuiz(updatedQuiz);
                _repo.Save();
                return RedirectToAction("MyQuizzes");
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Exception during quiz update: " + ex.Message);
                // Return the same Edit view so validation messages and the user's input are shown
                return View("Edit", updatedQuiz);
            }


        }

        _repo.UpdateQuiz(updatedQuiz);
        _repo.Save();
        return RedirectToAction("MyQuizzes");

    }

    public IActionResult Quiz(int id = 0)
    {
        var quiz = _repo.GetQuizWithDetails(id);
        if (quiz == null) return NotFound();

        var rng = new Random();
        foreach (var question in quiz.Questions)
            question.Options = question.Options.OrderBy(o => rng.Next()).ToList();

        return View(quiz);
    }

    public IActionResult Index()
    {
        return View();
    }
    public IActionResult Categories()
    {
        return View();
    }
    public IActionResult MostPopular()
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
        // Ensure questions/options exist and that at least one Option per question is marked correct.
        // Allow multiple options to be marked IsCorrect (do not force a single correct option).
        /*foreach (var q in quiz.Questions ??= new List<Question>())
        {
            q.options ??= new List<Option>();

            // Preserve any IsCorrect flags the UI posted. If none are set, pick the first so data stays consistent.
            if (!q.options.Any(o => o.IsCorrect))
            {
                if (q.options.Any())
                {
                    q.options.First().IsCorrect = true;
                }
            }
        }*/

        foreach(var question in quiz.Questions)
        {
            if(question.Options.Count < 2) { 
                ModelState.AddModelError("", "Each question must have at least two options.");
                return RedirectToAction("Create");
            }
        }

        if (string.IsNullOrWhiteSpace(quiz.Theme))
            quiz.Theme = Themes[Rng.Next(Themes.Length)];

        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (userId == null) return Unauthorized();

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
            if (quiz.Title.Contains(needle, StringComparison.OrdinalIgnoreCase) ||
                (quiz.Description != null && quiz.Description.Contains(needle, StringComparison.OrdinalIgnoreCase)))
            {
                result.Add(quiz);
            }
        }
        return View(result);
    }
}

