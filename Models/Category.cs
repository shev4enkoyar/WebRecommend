using System.ComponentModel.DataAnnotations;

namespace WebRecommend.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "CategoryName")]
        public string Name { get; set; }
    }
}
