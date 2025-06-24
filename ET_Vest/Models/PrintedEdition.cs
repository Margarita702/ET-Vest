using ET_Vest.Models.Enums;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ET_Vest.Models
{
    public class PrintedEdition
    {
        [Key]
        public int PrintedEditionId { get; set; }

        [Display(Name = "Заглавие")]
        [Required(ErrorMessage = "Заглавието е задължително поле.")]
        public string Title { get; set; }

        [Display(Name = "Категория")]
        [Required(ErrorMessage = "Категорията е задължително поле.")]
        [EnumDataType(typeof(Category))]
        public Category Category { get; set; }

        [Display(Name = "Периодичност")]
        [Required(ErrorMessage = "Периодичността е задължително поле.")]
        [EnumDataType(typeof(Periodicity))]
        public Periodicity Periodicity { get; set; }

        [Display(Name = "Доставна единична цена")]
        [Required(ErrorMessage = "Моля, въведете доставната единична цена.")]
        [RegularExpression(@"^[0-9]+(?:[\.,][0-9]{1,2})?$", 
            ErrorMessage = "Моля, въведете валидна цена.")]
        public decimal DeliveredUnitPrice { get; set; }

        [Display(Name = "Продажна цена")]
        [Required(ErrorMessage = "Моля, въведете продажната цена.")]
        [RegularExpression(@"^[0-9]+(?:[\.,][0-9]{1,2})?$",
            ErrorMessage = "Моля, въведете валидна цена.")]
        public decimal SalePrice { get; set; }

        public List<PrintedEditionProvider>? PrintEditionProviders { get; set; }

        public List<Request>? Requests { get; set; }

        public List<Sale>? Sale { get; set; }
    }
}
