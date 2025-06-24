using System.ComponentModel.DataAnnotations;

namespace ET_Vest.Data
{
    public enum Roles
    {
        [Display(Name = "Собственик")]
        Owner,
        [Display(Name = "Служител")]
        Employee,
        [Display(Name = "Администратор")]
        Admin
    }
}
