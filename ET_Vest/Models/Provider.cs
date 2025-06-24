using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ET_Vest.Models
{
    public class Provider
    {
        [Key]
        public int ProviderId { get; set; }

        [Display(Name = "Име")]
        [Required(ErrorMessage = "Името на доставчика е задължително")]
        public string Name { get; set; }

        [Display(Name = "Телефонен номер")]
        [RegularExpression(@"^\+?\d{0,2}\s?\(?\d{3}\)?[-.\s]?\d{3}[-.\s]?\d{4}$",
            ErrorMessage = "Невалиден телефонен номер")]
        public string PhoneNumber { get; set; }

        [Display(Name = "Имейл")]
        [EmailAddress(ErrorMessage = "Невалиден имейл адрес")]
        public string Email { get; set; }

        [Display(Name = "Град")]
        [Required(ErrorMessage = "Градът на доставчика е задължителен")]
        public string City { get; set; }

        public List<PrintedEditionProvider>? PrintEditionProviders { get; set; }
    }
}






