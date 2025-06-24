using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ET_Vest.Models.Enums
{
    public enum Periodicity
    {
        [Display(Name = "Ежедневник")]
        Daily,
        [Display(Name = "Ежеседмичник")]
        Weekly,
        [Display(Name = "Ежемесечник")]
        Monthly,
        [Display(Name = "Няма периодичност")]
        NoPeriodicity
    }
}
