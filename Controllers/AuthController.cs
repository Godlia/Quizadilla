using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Quizadilla.Areas.Identity.Data;
using Quizadilla.Dtos;

namespace Quizadilla.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<QuizadillaUser> _userManager;
        private readonly SignInManager<QuizadillaUser> _signInManager;

        public AuthController(
            UserManager<QuizadillaUser> userManager,
            SignInManager<QuizadillaUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null)
                return Unauthorized("Invalid email or password");

            var result = await _signInManager.PasswordSignInAsync(
                user,
                dto.Password,
                isPersistent: true,
                lockoutOnFailure: false
            );

            if (!result.Succeeded)
                return Unauthorized("Invalid email or password");

            return Ok(new { message = "Logged in" });
        }

        
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto){
        if (!ModelState.IsValid)
            return BadRequest(new { 
            errors = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToList()
            });


        var user = new QuizadillaUser{
            UserName = dto.Email,
            Email = dto.Email
        };

        var result = await _userManager.CreateAsync(user, dto.Password);

        if (!result.Succeeded){
            return BadRequest(new
            {
            errors = result.Errors.Select(e => e.Description).ToList()
            });
        }

        return Ok(new { message = "Registration successful" });
    }

       
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return Ok(new { message = "Logged out" });
        }

      
        [HttpGet("me")]
        public IActionResult Me()
        {
            if (!User.Identity!.IsAuthenticated)
                return Unauthorized();

            return Ok(new
            {
                email = User.Identity.Name
            });
        }

    
    [HttpPost("change-username")]
    public async Task<IActionResult> ChangeUsername([FromBody] ChangeUsernameDto dto){
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
            return Unauthorized();

        user.UserName = dto.NewUsername;

        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
            return BadRequest(result.Errors);

        return Ok(new { username = user.UserName });
        }

    [HttpPost("change-email")]
    public async Task<IActionResult> ChangeEmail([FromBody] ChangeEmailDto dto){
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
            return Unauthorized();

        var token = await _userManager.GenerateChangeEmailTokenAsync(user, dto.NewEmail);
        var result = await _userManager.ChangeEmailAsync(user, dto.NewEmail, token);

        if (!result.Succeeded)
            return BadRequest(result.Errors);

       
        user.UserName = dto.NewEmail;
        await _userManager.UpdateAsync(user);

        return Ok(new { email = user.Email });
    }

    [HttpPost("change-password")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto dto){
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
            return Unauthorized();

        var result = await _userManager.ChangePasswordAsync(
            user,
            dto.CurrentPassword,
            dto.NewPassword
        );

        if (!result.Succeeded)
            return BadRequest(result.Errors);

        return Ok(new { message = "Password changed" });
    }
   
    [HttpDelete("delete-account")]
    public async Task<IActionResult> DeleteAccount(){
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
            return Unauthorized();

        var result = await _userManager.DeleteAsync(user);

        if (!result.Succeeded)
            return BadRequest(result.Errors);

        await _signInManager.SignOutAsync();

        return Ok(new { message = "Account deleted" });
    }

    }
}
