using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Proje.Models
{
    public class Book
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Başlık zorunludur.")]
        [StringLength(100, ErrorMessage = "Başlık en fazla 100 karakter olabilir.")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Açıklama zorunludur.")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Görsel zorunludur.")]
        public string ImagePath { get; set; } // Kaydedilen dosyanın yolu, URL üzerinden indirilen resim buraya kaydedilecek

        // IFormFile kaldırıldı çünkü artık dosya yüklemiyoruz.
        [NotMapped]
        public IFormFile CoverPhoto { get; set; }

    }
}
