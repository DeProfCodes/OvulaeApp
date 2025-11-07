using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OvulaeApp.Helpers.Enums
{
    public enum EducationCategoryType
    {
        [Display(Name = "")]
        None,

        [Display(Name = "Top Recommended for you")]
        Recommended,

        [Display(Name = "Bookmarks")]
        Bookmarks,

        [Display(Name = "AllCategories")]
        AllCategories,
    }
}
