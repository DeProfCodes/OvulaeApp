using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OvulaeShared.Enums.App;

namespace OvulaeApp.ViewModels.Tips
{
    public class LikedTipsViewModel
    {
        public string UserId { get; set; }

        public ModuleType ModuleType { get; set; }

        public int Week { get; set; }
        
        public List<int> LikedTipIds { get; set; }

    }
}
