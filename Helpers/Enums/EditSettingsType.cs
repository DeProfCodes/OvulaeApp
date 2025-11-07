using System.ComponentModel.DataAnnotations;

namespace OvulaeApp.Helpers.Enums
{
    public enum EditSettingsType
    {
        [Display(Name = "")]
        None,

        [Display(Name = "PersonalInfoUpdate")]
        PersonalInfoUpdate,

        [Display(Name = "HealthInfoUpdate")]
        HealthInfoUpdate,

        [Display(Name = "PregnancyInfoUpdate")]
        PregnancyInfoUpdate,
    }
}
