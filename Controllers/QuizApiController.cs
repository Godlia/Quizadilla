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

    public QuizApiController(IQuizRepository repo, UserManager<QuizadillaUser> userManager, QuizDbContext db)
    {
        _repo = repo;
        _userManager = userManager;
        _db = db;
    }

    // -----------------------------
    // CREATE QUIZ
    // -----------------------------
   [HttpPost]
[Authorize]
public async Task<IActionResult> CreateQuiz([FromBody] CreateQuizDto dto)
{
    var user = await _userManager.GetUserAsync(User);
    if (user == null)
        return Unauthorized();

    if (string.IsNullOrWhiteSpace(dto.Title))
        return BadRequest("Quiz title is required");

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


    // -----------------------------
    // GET ALL
    // -----------------------------
    [HttpGet]
    public IActionResult GetAll()
    {
        return Ok(_repo.GetQuizzes());
    }

    // -----------------------------
    // GET MY QUIZZES
    // -----------------------------
    [HttpGet("my")]
    [Authorize]
    public async Task<IActionResult> My()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
            return Unauthorized();

        return Ok(_repo.GetUserQuizzes(user.Id));
    }

    // -----------------------------
    // GET ONE QUIZ
    // -----------------------------
    [HttpGet("{id}")]
    public IActionResult GetQuiz(int id)
    {
        var quiz = _repo.GetQuizWithDetails(id);
        if (quiz == null) return NotFound();

        return Ok(quiz);
    }
    // -----------------------------
    // SEARCH FUNCTION
    // -----------------------------    
    [HttpGet("search")]
public IActionResult Search([FromQuery] string needle)
{
    if (string.IsNullOrWhiteSpace(needle))
    {
        return BadRequest("Search term was empty");
    }

    var quizzes = _repo.GetQuizzes();
    var result = new List<Quiz>();

    foreach (var quiz in quizzes)
    {
        if (quiz.Title.Contains(needle, StringComparison.OrdinalIgnoreCase) ||
            (!string.IsNullOrEmpty(quiz.Description) &&
             quiz.Description.Contains(needle, StringComparison.OrdinalIgnoreCase)))
        {
            result.Add(quiz);
        }
    }

    return Ok(result);
}

    // -----------------------------
    // UPDATE QUIZ
    // -----------------------------
    [HttpPut("{id}")]
[Authorize]
public async Task<IActionResult> UpdateQuiz(int id, [FromBody] UpdateQuizDto dto)
{
    var user = await _userManager.GetUserAsync(User);
    if (user == null)
        return Unauthorized();

    var quiz = _repo.GetQuizForEdit(id);
    if (quiz == null)
        return NotFound("Quiz not found.");

    if (quiz.UserID != user.Id)
        return Forbid();

    // Oppdater top-level fields
    quiz.Title = dto.Title;
    quiz.Description = dto.Description ?? "";

    //
    // ------------------------------
    //    HANDLE QUESTIONS SAFELY
    // ------------------------------
    //

    // 1. Map eksisterende spørsmål
    var existingQuestions = quiz.Questions.ToList();

    // 2. Tøm spørsmål — vi legger tilbake kontrollert
    quiz.Questions.Clear();

    foreach (var qDto in dto.Questions)
    {
        Question qEntity;

        if (qDto.Id == 0)
        {
            // NYTT SPØRSMÅL
            qEntity = new Question();
        }
        else
        {
            // Finn eksisterende
            qEntity = existingQuestions.FirstOrDefault(x => x.Id == qDto.Id);
            if (qEntity == null)
            {
                return BadRequest($"Question with ID {qDto.Id} not found.");
            }

            // Rens opp eksisterende options for konflikt
            _db.Entry(qEntity).Collection(x => x.options).Load();
        }

        // Oppdater values
        qEntity.QuestionText = qDto.QuestionText;
        qEntity.correctString = qDto.CorrectString ?? "";

        //
        // ------------------------------
        //    HANDLE OPTIONS SAFELY
        // ------------------------------
        //

        var existingOptions = qEntity.options.ToList();
        qEntity.options.Clear();

        foreach (var oDto in qDto.Options)
        {
            Option oEntity;

            if (oDto.OptionId == 0)
            {
                // Ny option
                oEntity = new Option();
            }
            else
            {
                oEntity = existingOptions.FirstOrDefault(o => o.OptionId == oDto.OptionId);
                if (oEntity == null)
                    return BadRequest($"Option with ID {oDto.OptionId} not found.");
            }

            oEntity.OptionText = oDto.OptionText;
            qEntity.options.Add(oEntity);
        }

        quiz.Questions.Add(qEntity);
    }

    _repo.Save();

    return Ok(quiz);
    }
    [HttpDelete("{id}")]
[Authorize]
public async Task<IActionResult> DeleteQuiz(int id)
{
    var user = await _userManager.GetUserAsync(User);
    if (user == null)
        return Unauthorized();

    var quiz = _repo.GetQuizForEdit(id);
    if (quiz == null)
        return NotFound("Quiz not found");

    if (quiz.UserID != user.Id)
        return Forbid(); // ikke eier

    _repo.DeleteQuiz(id);
    _repo.Save();

    return Ok(new { message = "Quiz deleted" });
}
}
