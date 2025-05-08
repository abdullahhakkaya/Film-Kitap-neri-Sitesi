using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Proje.Models;

namespace Proje.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly AppDbContext _context;

    public HomeController(ILogger<HomeController> logger, AppDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    public IActionResult Index()
    {
        var userEmail = HttpContext.Session.GetString("UserEmail");
        ViewBag.UserEmail = userEmail;

        var recentBooks = _context.Books.OrderByDescending(b => b.Id).Take(2).ToList();
        var recentMovies = _context.Movies.OrderByDescending(m => m.Id).Take(2).ToList();
        var viewModel = new HomeViewModel
        {
            RecentBooks = recentBooks,
            RecentMovies = recentMovies
        };
        return View(viewModel);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
