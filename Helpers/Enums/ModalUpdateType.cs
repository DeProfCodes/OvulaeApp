using System.ComponentModel.DataAnnotations;

namespace OvulaeApp.Helpers.Enums
{
    public enum ModalUpdateType
    {
        [Display(Name = "", ShortName = "", Description = "", GroupName = "")]
        None,

        [Display(Name = "", ShortName = "", Description = "", GroupName = "")]
        All,

        //Health
        [Display(Name = "WeightUpdate", ShortName = "Change Weight & Unit", Description = "Update your weight and measurement unit (in kg or lbs).", GroupName = "Health")]
        WeightUpdate,

        [Display(Name = "HeightUpdate", ShortName = "Change Height & Unit", Description = "Update your height and measurement unit (in cm or ft/in).", GroupName = "Health")]
        HeightUpdate,

        [Display(Name = "BloodTypeUpdate", ShortName = "Change Height & Unit", Description = "Update your height and measurement unit (in cm or ft/in).", GroupName = "Health")]
        BloodTypeUpdate,

        //Personal
        [Display(Name = "AgeUpdate", ShortName = "", Description = "", GroupName = "Personal")]
        AgeUpdate,

        [Display(Name = "FirstnameUpdate", ShortName = "Change Firstname", Description = "Update your given firstname.", GroupName = "Personal")]
        FirstnameUpdate,

        [Display(Name = "LastnameUpdate", ShortName = "Change Lastname", Description = "Update your given lastname.", GroupName = "Personal")]
        LastnameUpdate,

        //Cycle
        [Display(Name = "LMP", ShortName = "Change Last Period Date", Description = "Set a date when you had your last menstrual period started.", GroupName = "Pregnancy")]
        LMP,

        //Pregnancy
        [Display(Name = "CurrentWeekAlong", ShortName = "Change Pregnancy Week", Description = "Modify pregnancy week.", GroupName = "Pregnancy")]
        CurrentWeekAlong,

        [Display(Name = "PregnancyDueDate", ShortName = "Change Pregnancy Due Date", Description = "Change date to your new delivery date.", GroupName = "Pregnancy")]
        PregnancyDueDate,

        //Period

    }
}
