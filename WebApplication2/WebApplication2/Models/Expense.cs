using System.ComponentModel.DataAnnotations;

namespace WebApplication2.Models;

public class Expense
{
    public int Id {get; set;}
    [Required]
    public string Description {get; set;}

    [Required]
    [Range(0.01, double.MaxValue), Err]
    public string Amount { get; set; }

    public string Category {get; set;}
    public DateTime Date { get; set; } = DateTime.Now;
}