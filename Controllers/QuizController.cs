using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Quizadilla.Models;

namespace Quizadilla.Controllers;
public class QuizController : Controller
{
    public IActionResult Discover()
    {
        return View();
    }
}