using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Proje.Models;

namespace Proje.Controllers
{
    public class UserController : Controller
    {
        private readonly AppDbContext _context;

        public UserController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(User user)
        {
            var existingUser = _context.Users.FirstOrDefault(u => u.Email == user.Email && u.Password == user.Password);
            if (existingUser != null)
            {
                // Oturum açıldığında session'a kullanıcı bilgilerini kaydedin
                HttpContext.Session.SetString("UserEmail", existingUser.Email); // Kullanıcıyı session'a ekleyin
                HttpContext.Session.SetString("UserRole", existingUser.Role); // Kullanıcıyı session'a ekleyin
                ViewBag.UserEmail = existingUser.Email;
                ViewBag.UserRole = existingUser.Role;
                return RedirectToAction("Index", "Home");
            }

            ViewBag.Error = "Geçersiz giriş bilgileri!";
            return View();
        }

        public IActionResult Signup()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Signup(User user)
        {
            if (_context.Users.Any(u => u.Email == user.Email))
            {
                ViewBag.Error = "Bu e-mail zaten kayıtlı!";
                return View();
            }

            // Şifrenin null olmaması gerektiğini kontrol etme
            if (string.IsNullOrEmpty(user.Password))
            {
                ViewBag.Error = "Şifre boş olamaz!";
                return View();
            }
            // Kullanıcının rolünü belirliyoruz
            user.Role = "User";  // Varsayılan olarak User rolü

            _context.Users.Add(user);
            _context.SaveChanges();
            
            // Kayıt olduktan sonra, kullanıcıyı otomatik giriş yapıyoruz
            var existingUser = _context.Users.FirstOrDefault(u => u.Email == user.Email && u.Password == user.Password);
            
            if (existingUser != null)
            {
                // Giriş başarılıysa, kullanıcı bilgilerini session'a kaydediyoruz
                HttpContext.Session.SetString("UserEmail", user.Email);
                // Giriş başarılıysa, ana sayfaya yönlendiriyoruz
                return RedirectToAction("Index", "Home");
            }

            // Eğer giriş başarısızsa hata mesajı gösteriyoruz
            ViewBag.Error = "Giriş yaparken bir hata oluştu!";
            return View();
        }
        public IActionResult Logout()
        {
            HttpContext.Session.Remove("UserEmail"); // Oturumu sonlandır
            HttpContext.Session.Remove("UserRole"); // Oturumu sonlandır
            return RedirectToAction("Index", "Home"); // Ana sayfaya yönlendir
        }
    }
}