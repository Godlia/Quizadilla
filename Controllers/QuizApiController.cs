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
            Theme = dto.Theme ?? "tomato", 
            UserID = user.Id,
            Questions = dto.Questions.Select(q => new Question
            {
                QuestionText = q.QuestionText,
                Options = q.Options.Select(o => new Option
                {
                    OptionText = o.OptionText,
                    IsCorrect = o.IsCorrect
                }).ToList()
            }).ToList()
        };

        _repo.AddQuiz(quiz);
        _repo.Save();

        return Ok(quiz);
    }

    
    [HttpGet]
    public IActionResult GetAll()
    {
        return Ok(_repo.GetQuizzes());
    }

  
    [HttpGet("my")]
    [Authorize]
    public async Task<IActionResult> My()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
            return Unauthorized();

        return Ok(_repo.GetUserQuizzes(user.Id));
    }

   
    [HttpGet("{id}")]
    public IActionResult GetQuiz(int id)
    {
        var quiz = _repo.GetQuizWithDetails(id);
        if (quiz == null) return NotFound();

        return Ok(quiz);
    }

  
    [HttpGet("search")]
    public IActionResult Search([FromQuery] string needle)
    {
        if (string.IsNullOrWhiteSpace(needle))
            return BadRequest("Search term was empty");

        var quizzes = _repo.GetQuizzes();
        var result = quizzes
            .Where(q =>
                q.Title.Contains(needle, StringComparison.OrdinalIgnoreCase) ||
                (!string.IsNullOrEmpty(q.Description) &&
                 q.Description.Contains(needle, StringComparison.OrdinalIgnoreCase))
            ).ToList();

        return Ok(result);
    }

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

       
        quiz.Title = dto.Title;
        quiz.Description = dto.Description ?? "";
        quiz.Theme = dto.Theme ?? quiz.Theme;      

       
        var existingQuestions = quiz.Questions.ToList();
        quiz.Questions.Clear();

        foreach (var qDto in dto.Questions)
        {
            Question qEntity;

            if (qDto.Id == 0)
            {
                qEntity = new Question();
            }
            else
            {
                qEntity = existingQuestions.FirstOrDefault(q => q.Id == qDto.Id);
                if (qEntity == null)
                    return BadRequest($"Question with ID {qDto.Id} not found.");

                _db.Entry(qEntity).Collection(q => q.Options).Load();
            }

            qEntity.QuestionText = qDto.QuestionText;

            
            var existingOptions = qEntity.Options.ToList();
            qEntity.Options.Clear();

            foreach (var oDto in qDto.Options)
            {
                Option oEntity;

                if (oDto.OptionId == 0)
                {
                    oEntity = new Option();
                }
                else
                {
                    oEntity = existingOptions.FirstOrDefault(o => o.OptionId == oDto.OptionId);
                    if (oEntity == null)
                        return BadRequest($"Option with ID {oDto.OptionId} not found.");
                }

                oEntity.OptionText = oDto.OptionText;
                oEntity.IsCorrect = oDto.IsCorrect;
                qEntity.Options.Add(oEntity);
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
            return Forbid();

        _repo.DeleteQuiz(id);
        _repo.Save();

        return Ok(new { message = "Quiz deleted" });
    }
}
