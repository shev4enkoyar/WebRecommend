using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebRecommend.Models
{
    public class Comment
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Body { get; set; }

        public DateTime CreateDate { get; set; }

        [Required]
        public string UserId { get; set; }

        [ForeignKey("UserId")]
        public virtual AppUser User { get; set; }

        [Required]
        public int ArticleId { get; set; }

        [ForeignKey("ArticleId")]
        public virtual Article Article { get; set; }
    }
}
