using System;
using System.IO;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Quizadilla.Models;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace Quizadilla.Controllers;
public class QuizController : Controller
{
    private readonly QuizDbContext db;

    public QuizController(QuizDbContext context)
    {
        db = context;
    }

    public IActionResult Quiz(int id = 0)
    {
        Quiz quiz = db.Quizzes.Include(q => q.Questions).FirstOrDefault(q => q.QuizId == id);
        return View(quiz);
    }
    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Discover()
    {
        
        var peepee = new Quiz();
        peepee.Title = "Test Quiz";
        peepee.Description = "Just a test quiz.";
        peepee.Questions = new List<Question>
        {
            new Question { QuestionText = "What is the capital of France?",
                           options = new List<Option>
                           {
                               new Option { OptionText = "Berlin" },
                               new Option { OptionText = "Madrid" },
                               new Option { OptionText = "Paris" },
                               new Option { OptionText = "Rome" }
                           },
                           correctString = "Paris"
            },
          };


        db.Quizzes.Add(peepee);
        db.SaveChanges();

        Console.WriteLine("Quizzes added to the database.");

        var quizzes = db.Quizzes.Include(q => q.Questions).ToList();

        //print out all quizzes, questions, and options using it's toString method
        foreach (var quiz in quizzes)
        {
            Console.WriteLine(quiz.toString());
        }

        return View();
    }
}