using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Quizadilla.Areas.Identity.Data;
using Quizadilla.Dtos;
using Quizadilla.Models;

namespace Quizadilla.Controllers;

[Route("api/quiz")]
[ApiController]
public class QuizApiController : ControllerBase
{
    private readonly IQuizRepository _repo;
    private readonly UserManager<QuizadillaUser> _userManager;
    private readonly QuizDbContext _db;

    public QuizApiController(
        IQuizRepository repo,
        UserManager<QuizadillaUser> userManager,
        QuizDbContext db)
    {
        _repo = repo;
        _userManager = userManager;
        _db = db;
    }

    // -----------------------------------------------------
    // CREATE QUIZ
    // -----------------------------------------------------
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> CreateQuiz([FromBody] CreateQuizDto dto)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
            return Unauthorized();

        var quiz = new Quiz
        {
            Title = dto.Title,
            Description = dto.Description ?? "",
            UserID = user.Id,
            Questions = dto.Questions.Select(q => new Question
            {
                QuestionText = q.QuestionText,
                correctString = q.CorrectString ?? "",
                options = q.Options.Select(o => new Option
                {
                    OptionText = o.OptionText
                }).ToList()
            }).ToList()
        };

        _repo.AddQuiz(quiz);
        _repo.Save();

        return Ok(quiz);
    }

    // -----------------------------------------------------
    // GET ALL
    // -----------------------------------------------------
    [HttpGet]
    public IActionResult GetAll()
    {
        return Ok(_repo.GetQuizzes());
    }

    // -----------------------------------------------------
    // GET MY QUIZZES
    // -----------------------------------------------------
    [HttpGet("my")]
    [Authorize]
    public async Task<IActionResult> My()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
            return Unauthorized();

        return Ok(_repo.GetUserQuizzes(user.Id));
    }

    // -----------------------------------------------------
    // GET QUIZ (FULL DETAILS)
    // -----------------------------------------------------
    [HttpGet("{id}")]
    public IActionResult GetQuiz(int id)
    {
        var quiz = _repo.GetQuizWithDetails(id);
        if (quiz == null) return NotFound();

        return Ok(quiz);
    }

    // -----------------------------------------------------
    // UPDATE QUIZ
    // -----------------------------------------------------
    [HttpPut("{id}")]
    [Authorize]
    public async Task<IActionResult> UpdateQuiz(int id, [FromBody] UpdateQuizDto dto)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
            return Unauthorized();

        var existing = _repo.GetQuizForEdit(id);
        if (existing == null)
            return NotFound("Quiz not found.");

        if (existing.UserID != user.Id)
            return Forbid();

        // Update title + description
        existing.Title = dto.Title;
        existing.Description = dto.Description ?? "";

        // --- Handle Questions ---
        var incomingQuestions = dto.Questions;

        // Remove deleted questions
        foreach (var oldQ in existing.Questions.ToList())
        {
            if (!incomingQuestions.Any(q => q.Id == oldQ.Id))
            {
                _db.Remove(oldQ);
            }
        }

        // Add/update questions
        foreach (var q in incomingQuestions)
        {
            var existingQ = existing.Questions.FirstOrDefault(x => x.Id == q.Id);

            if (existingQ == null)
            {
                // NEW QUESTION
                existing.Questions.Add(new Question
                {
                    QuestionText = q.QuestionText,
                    correctString = q.CorrectString ?? "",
                    options = q.Options.Select(o => new Option
                    {
                        OptionText = o.OptionText
                    }).ToList()
                });
            }
            else
            {
                // UPDATE EXISTING QUESTION
                existingQ.QuestionText = q.QuestionText;
                existingQ.correctString = q.CorrectString ?? "";

                // Remove deleted options
                foreach (var oldOpt in existingQ.options.ToList())
                {
                    if (!q.Options.Any(o => o.OptionId == oldOpt.OptionId))
                        _db.Remove(oldOpt);
                }

                // Add or update options
                foreach (var o in q.Options)
                {
                    var existingOpt = existingQ.options.FirstOrDefault(x => x.OptionId == o.OptionId);

                    if (existingOpt == null)
                    {
                        existingQ.options.Add(new Option
                        {
                            OptionText = o.OptionText
                        });
                    }
                    else
                    {
                        existingOpt.OptionText = o.OptionText;
                    }
                }
            }
        }

        _repo.Save();
        return Ok(existing);
    }
}
