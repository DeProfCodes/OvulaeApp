using System.ComponentModel.DataAnnotations;

namespace OvulaeApp.Helpers.Enums
{
    public enum DashboardTab
    {
        [Display(Name = "", ShortName = "")]
        None,

        [Display(Name = "Home", ShortName = "home")]
        Home,

        [Display(Name = "Track", ShortName = "calendar")]
        Track,

        [Display(Name = "Learn", ShortName = "books")]
        Learn,

        [Display(Name = "Alerts", ShortName = "bell")]
        Alerts,

        [Display(Name = "Profile", ShortName = "profile")]
        Profile
    }
}
