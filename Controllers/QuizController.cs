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
        var NewQuiz = new Quiz
        {
            Title = "peepee",
            Description = "This is a sample quiz description.",
            Questions = new List<Question>
            {
                new Question
                {
                    QuestionText = "JAAAAA?"
                },
                new Question
                {
                    QuestionText = "What is 2 + 2?"
                }
            }
        };

        var peepee = new Quiz();
        peepee.Title = "Test Quiz";
        peepee.Description = "Just a test quiz.";


        db.Quizzes.Add(NewQuiz);
        db.Quizzes.Add(peepee);
        db.SaveChanges();

        Console.WriteLine("Quizzes added to the database.");

        var quizzes = db.Quizzes.Include(q => q.Questions).ToList();
        foreach (var quiz in quizzes)
        {
            Console.WriteLine($"Quiz ID: {quiz.QuizId}, Title: {quiz.Title}, Description: {quiz.Description}");
            foreach (var question in quiz.Questions)
            {
                Console.WriteLine($"\tQuestion ID: {question.Id}, Text: {question.QuestionText}");
            }
        }
        
        /*
        var conn = db.Database.GetDbConnection();
        string dbPath;
        try
        {
            var builder = new SqliteConnectionStringBuilder(conn.ConnectionString);
            dbPath = Path.GetFullPath(builder.DataSource ?? "QuizDatabase.db");
        }
        catch
        {
            dbPath = Path.GetFullPath("QuizDatabase.db");
        }

        System.Diagnostics.Debug.WriteLine($"SQLite file path: {dbPath}");
        Console.WriteLine($"SQLite file path: {dbPath}");
        */
        return View();
    }
}