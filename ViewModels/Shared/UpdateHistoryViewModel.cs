using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OvulaeShared.Enums.App;

namespace OvulaeApp.ViewModels.Shared
{
    public class UpdateHistoryViewModel
    {
        public ModuleType ModuleType { get; set; }

        public OvulaeFeatureType FeatureType { get; set; }

        public bool PerformUpdate { get; set; }
    }
}
