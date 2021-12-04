using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebRecommend.Models
{
    public class Article
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Title")]
        public string Title { get; set; }

        [Required]
        [Display(Name = "Desсription")]
        public string Desсription { get; set; }

        [Required]
        [Display(Name = "Body")]
        public string Body { get; set; }

        public DateTime CreateDate { get; set; }

        [Display(Name = "AuthorMark")]
        public int AuthorMark { get; set; }

        [Required]
        public double RatingAverage { get; set; }

        [Display(Name = "UserId")]
        public string UserId { get; set; }

        [ForeignKey("UserId")]
        public virtual AppUser User { get; set; }

        [Required]
        [Display(Name = "CategoryId")]
        public int CategoryId { get; set; }

        [ForeignKey("CategoryId")]
        public virtual Category Category { get; set; }
    }
}
