using Microsoft.AspNetCore.Mvc;
using Quizadilla.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Quizadilla.Controllers;

[ApiController]
[Route("api/quiz")]
public class QuizApiController : ControllerBase
{
    private readonly IQuizRepository _repo;

    private static readonly string[] Themes = { "tomato", "guac", "cheese", "onion", "chicken", "salsa" };
    private static readonly Random Rng = new();

    public QuizApiController(IQuizRepository repo)
    {
        _repo = repo;
    }

    // GET api/quiz
    [HttpGet]
    public ActionResult<IEnumerable<Quiz>> GetAll()
    {
        var quizzes = _repo.GetQuizzes();
        return Ok(quizzes);
    }

    // GET api/quiz/{id}
    [HttpGet("{id:int}")]
    public ActionResult<Quiz> Get(int id)
    {
        var quiz = _repo.GetQuizWithDetails(id);
        if (quiz == null) return NotFound();

        // Bland options slik du gjør i MVC Quiz-viewet 
        var rng = new Random();
        foreach (var question in quiz.Questions)
            question.options = question.options.OrderBy(o => rng.Next()).ToList();

        return Ok(quiz);
    }

    // GET api/quiz/search?needle=abc
    [HttpGet("search")]
    public ActionResult<IEnumerable<Quiz>> Search(string needle)
    {
        if (string.IsNullOrWhiteSpace(needle))
            return Ok(Array.Empty<Quiz>());

        var quizzes = _repo.GetQuizzes();
        var result = new List<Quiz>();
        foreach (var quiz in quizzes)
        {
            if (quiz.Title.Contains(needle, StringComparison.OrdinalIgnoreCase))
                result.Add(quiz);
        }

        return Ok(result);
    }

    // GET api/quiz/my
    [HttpGet("my")]
    public ActionResult<IEnumerable<Quiz>> MyQuizzes()
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userId))
        {
            // Ingen innlogget bruker – returner tom liste i stedet for 400
            return Ok(Array.Empty<Quiz>());
        }

        var quizzes = _repo.GetUserQuizzes(userId);
        return Ok(quizzes);
    }

    // POST api/quiz
    [HttpPost]
    public ActionResult<Quiz> Create([FromBody] Quiz quiz)
    {
        // Samme logikk som i QuizController.CreateQuiz 
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
        if (userId == null)
        {
            // For enkelhets skyld: la React sende en dummy UserID hvis du ikke bruker auth.
            quiz.UserID = "react-user";
        }
        else
        {
            quiz.UserID = userId;
        }

        _repo.AddQuiz(quiz);
        _repo.Save();

        return CreatedAtAction(nameof(Get), new { id = quiz.QuizId }, quiz);
    }

    // PUT api/quiz/{id}
    [HttpPut("{id:int}")]
    public IActionResult Update(int id, [FromBody] Quiz updatedQuiz)
    {
        if (id != updatedQuiz.QuizId) return BadRequest();

        _repo.UpdateQuiz(updatedQuiz);
        _repo.Save();

        return NoContent();
    }

    // DELETE api/quiz/{id}
    [HttpDelete("{id:int}")]
    public IActionResult Delete(int id)
    {
        _repo.DeleteQuiz(id);
        _repo.Save();
        return NoContent();
    }
}

