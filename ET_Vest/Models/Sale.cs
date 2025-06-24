using System;
using System.ComponentModel.DataAnnotations;

namespace ET_Vest.Models
{
    public class Sale
    {
        [Key]
        public int SalesId { get; set; }

        [Display(Name = "Дата на продажба")]
        public DateTime DateOfSale { get; set; }

        [Display(Name = "Печатно издание")]
        [Required(ErrorMessage = "Полето за печатно издание е задължително")]
        public int PrintedEditionId { get; set; }
        public PrintedEdition PrintedEdition { get; set; }

        [Display(Name = "Търговски обект")]
        [Required(ErrorMessage = "Полето за търговски обект е задължително")]
        public int TradeObjectId { get; set; }
        public TradeObject TradeObject { get; set; }

        [Display(Name = "Количество за продажба")]
        [Required(ErrorMessage = "Полето за количество за продажба е задължително")]
        [Range(1, int.MaxValue,
            ErrorMessage = "Количеството за продажба трябва да бъде положително число")]
        public int SoldQuantity { get; set; }

        public decimal Total => PrintedEdition?.SalePrice * SoldQuantity ?? 0;
    }
}



