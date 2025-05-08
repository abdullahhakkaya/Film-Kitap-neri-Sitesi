using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using System.Linq;
using Proje.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;

namespace Proje.Controllers
{
    public class MovieController : Controller
    {
        private readonly IWebHostEnvironment _env;
        private readonly AppDbContext _context;

        public MovieController(IWebHostEnvironment env, AppDbContext context)
        {
            _env = env;
            _context = context;
        }

        [HttpGet]
        public IActionResult Index(int page = 1)
        {
            int pageSize = 10;
            var movies = _context.Movies
                                .OrderByDescending(m => m.Id)
                                .Skip((page - 1) * pageSize)
                                .Take(pageSize)
                                .ToList();

            int totalMovies = _context.Movies.Count();
            int totalPages = (int)Math.Ceiling((double)totalMovies / pageSize);

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            return View(movies);
        }

        // Kitap Detay Sayfası
        public IActionResult Details(int id)
        {
            var movie = _context.Movies.FirstOrDefault(m => m.Id == id);

            if (movie == null)
            {
                return NotFound();
            }

            return View(movie);
        }

        [HttpGet]
        public IActionResult Create()
        {
            if (HttpContext.Session.GetString("UserEmail") == null)
            {
                return RedirectToAction("Login", "User");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(MovieViewModel model)
        {
            if (HttpContext.Session.GetString("UserEmail") == null)
            {
                return RedirectToAction("Login", "User");
            }

            if (ModelState.IsValid)
            {
                if (model.CoverPhoto != null && model.CoverPhoto.Length > 0)
                {
                    // Geçerli uzantıları kontrol et
                    var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
                    var fileExtension = Path.GetExtension(model.CoverPhoto.FileName).ToLower();

                    if (!allowedExtensions.Contains(fileExtension))
                    {
                        ModelState.AddModelError("CoverPhoto", "Sadece .jpg, .jpeg ve .png formatları desteklenmektedir.");
                        return View(model);
                    }

                    // Dosya yükleme klasörünü belirle
                    string uploadsFolder = Path.Combine(_env.WebRootPath, "uploads");
                    Directory.CreateDirectory(uploadsFolder);

                    // Dosya adını benzersiz yap
                    string uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(model.CoverPhoto.FileName);
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    // Dosyayı kaydet
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await model.CoverPhoto.CopyToAsync(fileStream);
                    }

                    // Veritabanına kayıt ekle
                    var newMovie = new Movie
                    {
                        Title = model.Title,
                        Description = model.Description,
                        ImagePath = "/uploads/" + uniqueFileName
                    };

                    _context.Movies.Add(newMovie);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Film başarıyla eklendi!";
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError("CoverPhoto", "Lütfen bir resim dosyası seçin.");
                }
            }

            return View(model);
        }
        [HttpPost]
        public IActionResult Delete(int id)
        {
            if (HttpContext.Session.GetString("UserRole") != "Admin")
            {
                return Unauthorized();
            }

            var movie = _context.Movies.Find(id);
            if (movie != null)
            {
                var filePath = Path.Combine(_env.WebRootPath, "uploads", Path.GetFileName(movie.ImagePath));
                if(System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }
                _context.Movies.Remove(movie);
                _context.SaveChanges();
                TempData["SuccessMessage"] = "Film başarıyla silindi!"; // Başarı mesajı
            }
            else
            {
                TempData["ErrorMessage"] = "Film bulunamadı!"; // Hata mesajı
            }
            return RedirectToAction("Index");
        }
    }
}
