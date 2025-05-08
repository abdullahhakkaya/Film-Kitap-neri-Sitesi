using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Proje.Models;


namespace Proje.Controllers
{
    public class AdminController : Controller
    {
        private readonly AppDbContext _context;

        // AdminController constructor
        public AdminController(AppDbContext context)
        {
            _context = context;
        }

        // Admin kullanıcıları yönetebilecek
        [HttpGet]
        public IActionResult ManageUsers()
        {
            var userRole = HttpContext.Session.GetString("UserRole");
            if (userRole != "Admin")
            {
                return RedirectToAction("Index", "Home");
            }
            var users = _context.Users.ToList(); // Veritabanından tüm kullanıcıları al
            return View(users);
        }

        // Kullanıcıyı silme
        [HttpPost]
        public IActionResult DeleteUser(int id)
        {
            var user = _context.Users.FirstOrDefault(u => u.Id == id);
            if (user != null)
            {
                _context.Users.Remove(user);
                _context.SaveChanges();
            }
            return RedirectToAction("ManageUsers");
        }

        public IActionResult SomeAction()
        {
            var userEmail = HttpContext.Session.GetString("UserEmail");
            var user = _context.Users.FirstOrDefault(u => u.Email == userEmail);
            
            // Kullanıcı bilgilerini view'a gönder
            ViewBag.UserRole = user?.Role;
            return View();
        }

    }
}
