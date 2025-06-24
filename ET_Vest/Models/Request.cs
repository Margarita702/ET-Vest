using System;
using ET_Vest.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace ET_Vest.Models
{
    public class Request
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "Дата на заявка")]
        public DateTime RequestDate { get; set; }

        [Display(Name = "Заявено количество")]
        [Required(ErrorMessage =
            "Полето за заявено количество е задължително")]
        [Range(1, int.MaxValue, ErrorMessage =
            "Заявеното количество трябва да бъде положително число")]
        public int RequestedQuantity { get; set; }

        [Display(Name = "Статус")]
        public RequestStatus Status { get; set; }

        [Display(Name = "Категория")]
        public Category Category { get; set; }

        [Display(Name = "Търговски обект")]
        [Required(ErrorMessage =
            "Полето за търговски обект е задължително")]
        public int TradeObjectId { get; set; }
        public TradeObject TradeObject { get; set; }

        [Display(Name = "Печатно издание")]
        [Required(ErrorMessage = 
            "Полето за печатно издание е задължително")]
        public int PrintedEditionId { get; set; }
        public PrintedEdition PrintedEdition { get; set; }

        [Display(Name = "Доставчик")]
        [Required(ErrorMessage =
            "Полето за доставчик е задължително")]
        public int ProviderId { get; set; }
        public Provider Provider { get; set; }
    }
}


