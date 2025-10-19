using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Quizadilla.Models;

namespace Quizadilla.Controllers;
public class ItemController : Controller
{
    public IActionResult Table()
    {
        var items = new List<Item>();
        var item1 = new Item();
        item1.ItemID = 1;
        item1.Name = "Pizza";
        item1.Price = 60;

        var item2 = new Item { ItemID = 2, Name = "Leg", Price = 12 };

        items.Add(item1);
        items.Add(item2);

        ViewBag.CurrentViewName = "List of Quizzadilllas";
        return View(items);
    }
}