using System.ComponentModel.DataAnnotations.Schema;

namespace WebRecommend.Models
{
    public class Reputation
    {
        public int ArticleId { get; set; }
        [ForeignKey("ArticleId")]
        public Article Article { get; set; }

        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public AppUser User { get; set; }

        public string AuthorId { get; set; }
        [ForeignKey("AuthorId")]
        public AppUser Author { get; set; }
    }
}
