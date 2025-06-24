using System.ComponentModel.DataAnnotations;

namespace ET_Vest.Models
{
    public class TradeObject
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "Наименование на обекта")]
        [Required(ErrorMessage = "Наименованието на обекта е задължително")]
        public string Name { get; set; }

        [Display(Name = "Адрес")]
        [Required(ErrorMessage = "Адресът е задължителен")]
        public string Address { get; set; }

        [Display(Name = "Служител")]
        [Required(ErrorMessage = "Полето 'Служител' е задължително")]
        public string EmployeeId { get; set; } 

        // Reference to ApplicationUser as the Employee
        public ApplicationUser Employee { get; set; }

        public List<Request>? Requests { get; set; }

        public List<Sale>? Sales { get; set; }
    }
}


