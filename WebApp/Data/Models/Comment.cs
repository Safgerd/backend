using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApp.Data.Models
{
    public class Comment
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public string UserId { get; set; }
        public int PostId { get; set; }

        [ForeignKey("UserId")]
        public virtual IdentityUser User { get; set; }
        [ForeignKey("PostId")]
        public virtual Post Post { get; set; }

    }
}
