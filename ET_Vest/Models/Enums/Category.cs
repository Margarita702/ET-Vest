using System.ComponentModel.DataAnnotations;

namespace ET_Vest.Models.Enums
{
    public enum Category
    {
        [Display(Name = "Вестник")]
        Newspaper,
        [Display(Name = "Списание")]
        Magazine,
        [Display(Name = "Книга")]
        Book,
        [Display(Name = "Енциклопедия")]
        Encyclopedia
    }
}
