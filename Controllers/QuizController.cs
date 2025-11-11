using System;
using System.IO;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Quizadilla.Models;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace Quizadilla.Controllers;

public class QuizController : Controller
{
    private readonly QuizDbContext db;

    private static readonly string[] Themes = { "tomato", "guac", "cheese", "onion", "chicken", "salsa" };
    private static readonly Random Rng = new();

    public QuizController(QuizDbContext context)
    {
        db = context;
    }

    [HttpGet]
    public IActionResult Edit(int id)
    {
        var quiz = db.Quizzes
            .Include(q => q.Questions)
            .ThenInclude(q => q.options)
            .FirstOrDefault(q => q.QuizId == id);

        if (quiz == null)
            return NotFound();

        return View(quiz);
    }

    [HttpPost] 
    public IActionResult Delete(int id)
    {
        var quiz = db.Quizzes
            .Include(q => q.Questions)
            .ThenInclude(q => q.options)
            .FirstOrDefault(q => q.QuizId == id);

        if (quiz == null)
        {
            return NotFound();
        }

        // Remove related entities (Options -> Questions -> Quiz)
        foreach (var question in quiz.Questions.ToList())
        {
            // Remove all options for this question
            if (question.options != null && question.options.Any())
            {
                foreach (var option in question.options.ToList())
                {
                    db.Remove(option);
                }
            }

            db.Remove(question);
        }

        db.Remove(quiz);
        db.SaveChanges();

        return RedirectToAction("Discover");
    }




    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(Quiz updatedQuiz)
    {
        if (!ModelState.IsValid)
            return View(updatedQuiz);

        var existingQuiz = db.Quizzes
            .Include(q => q.Questions)
            .ThenInclude(q => q.options)
            .FirstOrDefault(q => q.QuizId == updatedQuiz.QuizId);

        if (existingQuiz == null)
            return NotFound();

        // --- Update quiz fields ---
        existingQuiz.Title = updatedQuiz.Title;
        existingQuiz.Description = updatedQuiz.Description;

        // --- Handle Questions ---
        var existingQuestions = existingQuiz.Questions.ToList();

        // Remove deleted questions
        foreach (var existingQuestion in existingQuestions)
        {
            if (!updatedQuiz.Questions.Any(q => q.Id == existingQuestion.Id))
            {
                db.Remove(existingQuestion);
            }
        }

        // Add or update questions
        foreach (var q in updatedQuiz.Questions)
        {
            var existingQuestion = existingQuestions.FirstOrDefault(eq => eq.Id == q.Id);

            if (existingQuestion == null)
            {
                // New question
                existingQuiz.Questions.Add(q);
            }
            else
            {
                // Update question
                existingQuestion.QuestionText = q.QuestionText;
                existingQuestion.correctString = q.correctString;

                // --- Handle Options ---
                var existingOptions = existingQuestion.options.ToList();

                // Remove deleted options
                foreach (var existingOption in existingOptions)
                {
                    if (!q.options.Any(o => o.OptionId == existingOption.OptionId))
                    {
                        db.Remove(existingOption);
                    }
                }

                // Add or update options
                foreach (var opt in q.options)
                {
                    var existingOpt = existingOptions.FirstOrDefault(o => o.OptionId == opt.OptionId);
                    if (existingOpt == null)
                    {
                        existingQuestion.options.Add(opt);
                    }
                    else
                    {
                        existingOpt.OptionText = opt.OptionText;
                    }
                }
            }
        }

        db.SaveChanges();
        return RedirectToAction("Discover");
    }




    public IActionResult Quiz(int id = 0)
    {
        Quiz quiz = db.Quizzes
            .Include(q => q.Questions)
            .ThenInclude(q => q.options)
            .FirstOrDefault(q => q.QuizId == id);


        if (quiz == null)
            return NotFound();

            var Rng = new Random();
            foreach (var question in quiz.Questions)

        { 
                question.options = question.options.OrderBy(o => Rng.Next()).ToList(); 
            }

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
        if (true)
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
                    {
                        q.options.Add(new Option { OptionText = q.correctString });
                    }
                }
            }

            if (string.IsNullOrWhiteSpace(quiz.Theme)) quiz.Theme = Themes[Rng.Next(Themes.Length)];

            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return BadRequest();
            }
            quiz.UserID = userId;

            db.Quizzes.Add(quiz);
            db.SaveChanges();
            return RedirectToAction("MyQuizzes");
        } else
        {
            return BadRequest("Error in modelstate");
        }
        
    }
    public IActionResult Discover()
    {
        var quizzes = db.Quizzes.Include(q => q.Questions).ToList();

        //print out all quizzes, questions, and options using it's toString method
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
        if(userId == null)
        {
        } else { 
            var quizzes = db.Quizzes.Where<Quiz>(q => q.UserID == userId).ToList();
            return View(quizzes);
        }
        return BadRequest();
    }
}