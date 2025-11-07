using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OvulaeApp.Helpers.Enums
{
    public enum ModalCloseType
    {
        [Display(Name = "")]
        None,

        [Display(Name = "CloseButton")]
        CloseButton,

        [Display(Name = "Accept")]
        Accept,

        [Display(Name = "Reject")]
        Reject,

        [Display(Name = "Cancel")]
        Cancel,

        [Display(Name = "Error")]
        Error,
    }
}
