using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OvulaeShared.Models.Tips;

namespace OvulaeApp.ViewModels.Tips
{
    public class TipViewModel
    {
        public TipItem TipItem { get; set; }

        public bool IsChecked { get; set; }
    }
}
