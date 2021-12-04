using System.ComponentModel.DataAnnotations.Schema;

namespace WebRecommend.Models
{
    public class Rating
    {
        public int ArticleId { get; set; }
        [ForeignKey("ArticleId")]
        public Article Article { get; set; }

        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public AppUser User { get; set; }

        public int Grade { get; set; }
    }
}
