using System.ComponentModel.DataAnnotations;

public enum RequestStatus
{
    [Display(Name = "Изчакваща")]
    Pending,

    [Display(Name = "Изпратена към собственик")]
    SentToOwner,

    [Display(Name = "Изпратена към доставчик")]
    SentToProvider,

    [Display(Name = "Отхвърлена")]
    Rejected,

    [Display(Name = "Изпълнена")]
    Completed
}
