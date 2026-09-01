using Microsoft.AspNetCore.Mvc;
using Menu.Contexts;
using Menu.Models;
using Microsoft.EntityFrameworkCore;

namespace Menu.Controllers;

public class MenuController : Controller
{
    private readonly MenuContext _context;
    
    public MenuController(MenuContext context)
    {
        _context = context;
    }
    public async Task<IActionResult> Index()
    {
        return View(await _context.Dishes.ToListAsync());
    }
    
    public async Task<IActionResult> Details (int? id)
    {
        var dish = await _context.Dishes
            .Include(di => di.DishIngredients)
            .ThenInclude(i => i.Ingredient)
            .FirstOrDefaultAsync(x => x.Id == id);
        
        if (dish == null)
        {
            return NotFound();
        }
        return View(dish);
    }
    
}