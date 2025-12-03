using Microsoft.AspNetCore.Mvc;
using Quizadilla.Models;          
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/quiz")]
public class QuizApiController : ControllerBase
{
    private readonly QuizDbContext _db;

    public QuizApiController(QuizDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var quizzes = await _db.Quizzes.ToListAsync();
        return Ok(quizzes);
    }
}
