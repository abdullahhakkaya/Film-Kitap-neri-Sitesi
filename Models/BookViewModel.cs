using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Proje.Models
{
    public class BookViewModel
    {
        [Required(ErrorMessage = "Başlık zorunludur.")]
        [StringLength(100, ErrorMessage = "Başlık en fazla 100 karakter olabilir.")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Açıklama zorunludur.")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Bir resim dosyası seçmelisiniz.")]
        public IFormFile CoverPhoto { get; set; }
    }
}
