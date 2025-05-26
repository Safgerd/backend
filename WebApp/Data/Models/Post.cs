using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApp.Data.Models
{
    public class Post
    {
        [Key]
        public int Id { get; set; }
        [Display(Name = "Название поста")]
        [Required(ErrorMessage = "Пост должен иметь название")]
        public string Title { get; set; }
        [Display(Name = "Содержание")]
        public string Content { get; set; }
        [Required]
        public string UserId { get; set; }

        [ForeignKey("UserId")]
        public virtual IdentityUser User { get; set; }
        
        public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();

    }
}
