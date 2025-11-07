using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OvulaeApp.Helpers.Enums
{
    public enum OvulationPhase
    {
        [Display(Name = "")]
        None = 0,

        [Display(Name = "MenstrualPhase")]
        Menstrual = 1,

        [Display(Name = "Follicular")]
        Follicular = 2,

        [Display(Name = "Ovulation")]
        Ovulation = 3,

        [Display(Name = "Luteal")]
        Luteal = 4,
    }
}
