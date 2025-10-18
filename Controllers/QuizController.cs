using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using project.Models;
using Quizadilla.Models;

namespace Quizadilla.Controllers;
public class QuizController : Controller
{
    private readonly QuizDbContext db;

    public QuizController(QuizDbContext context)
    {
        db = context;
    }

    public IActionResult Discover()
    {
        var NewQuiz = new Quiz
        {
            Title = "Sample Quiz",
            Description = "This is a sample quiz description.",
            Questions = new Question[]
            {
                new Question
                {
                    QuestionText = "What is the capital of France?",
                    Options = new string[] { "Berlin", "Madrid", "Paris", "Rome" },
                    CorrectOptionIndex = 2
                },
                new Question
                {
                    QuestionText = "What is 2 + 2?",
                    Options = new string[] { "3", "4", "5", "6" },
                    CorrectOptionIndex = 1
                }
            }
        };
        db.SaveChanges();

        return View();
    }
}