using System.ComponentModel.DataAnnotations;

namespace ET_Vest.Models
{
    public class Inventory
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "Търговски обект")]
        [Required(ErrorMessage = "Обектът е задължителен")]
        public int TradeObjectId { get; set; }
        public TradeObject TradeObject { get; set; }

        [Display(Name = "Печатно издание")]
        [Required(ErrorMessage = "Изданието е задължително")]
        public int PrintedEditionId { get; set; }
        public PrintedEdition PrintedEdition { get; set; }

        [Display(Name = "Налично количество")]
        [Required(ErrorMessage = "Количеството е задължително")]
        [Range(1, int.MaxValue, ErrorMessage = 
            "Количеството трябва да бъде положително число и по-голямо от 0")]
        public int Quantity { get; set; }

        public bool IsDisposed { get; set; }
    }
}


