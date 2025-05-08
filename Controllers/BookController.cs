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
    public class BookController : Controller
    {
        private readonly IWebHostEnvironment _env;
        private readonly AppDbContext _context;

        public BookController(IWebHostEnvironment env, AppDbContext context)
        {
            _env = env;
            _context = context;
        }

        [HttpGet]
        public IActionResult Index(int page = 1)
        {
            int pageSize = 12;
            var books = _context.Books
                                .OrderByDescending(b => b.Id)
                                .Skip((page - 1) * pageSize)
                                .Take(pageSize)
                                .ToList();

            int totalBooks = _context.Books.Count();
            int totalPages = (int)Math.Ceiling((double)totalBooks / pageSize);

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            return View(books);
        }

        // Kitap Detay Sayfası
        public IActionResult Details(int id)
        {
            var book = _context.Books.FirstOrDefault(b => b.Id == id);

            if (book == null)
            {
                return NotFound();
            }

            return View(book);
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
        public async Task<IActionResult> Create(BookViewModel model)
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
                    var newBook = new Book
                    {
                        Title = model.Title,
                        Description = model.Description,
                        ImagePath = "/uploads/" + uniqueFileName
                    };

                    _context.Books.Add(newBook);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Kitap başarıyla eklendi!";
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

            var book = _context.Books.Find(id);
            if (book != null)
            {
                // Resmin fiziksel olarak silinmesi
                var filePath = Path.Combine(_env.WebRootPath, "uploads", Path.GetFileName(book.ImagePath));
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath); // Resmi sil
                }
                _context.Books.Remove(book);
                _context.SaveChanges();
                TempData["SuccessMessage"] = "Kitap başarıyla silindi!"; // Başarı mesajı
            }
            else
            {
                TempData["ErrorMessage"] = "Kitap bulunamadı!"; // Hata mesajı
            }
            return RedirectToAction("Index");  // Sayfayı yeniden aynı "Index" sayfasına yönlendiriyoruz
        }

    }
}
