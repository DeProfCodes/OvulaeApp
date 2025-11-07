using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OvulaeShared.Enums.App;

namespace OvulaeApp.Services.LocalDataService.Updates
{
    public interface IUpdatesService
    {
        public Task<bool> ReloadUpdateHistoryDataAsync();

        public bool PerformServerUpdateModule(ModuleType moduleType);

        public bool PerformServerUpdateFeature(OvulaeFeatureType featureType);
    }
}
