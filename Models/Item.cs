using System;
namespace Quizadilla.Models
{
    public class Item
{
    public int ItemID { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price  { get; set; }
    public string? Description { get; set; }
    public string? ImageURL { get; set; }
}    
}