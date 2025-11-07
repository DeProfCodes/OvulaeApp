using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OvulaeShared.Enums.App;
using OvulaeShared.ViewModel.Diet;

namespace OvulaeApp.Services.LocalDataService.DietServices
{
    public class DietService : IDietService
    {
        public DietService()
        {
            
        }

        private List<DietGroupItemsViewModel> GetModuleData(ModuleType moduleType)
        {
            var data = new List<DietGroupItemsViewModel>();

            if (moduleType == ModuleType.Pregnancy)
                data = LocalStorageService.PregnancyData.AllPregDietGroups;

            return data;
        }

        public List<DietGroupItemsViewModel> GetWeekDiet(ModuleType moduleType, int week)
        {
            var data = GetModuleData(moduleType).Where(d => d.Group.Weeks.Contains(week)).ToList();

            data.ForEach(d =>
            {
                d.Group.CoverImageSrc = !string.IsNullOrEmpty(d.Group.CoverImageSrc) ? $"Diet/Pregnancy/covers/{d.Group.CoverImageSrc}" : "";
                d.Items.ForEach(d =>
                {
                    d.ImageSrc = !string.IsNullOrEmpty(d.ImageSrc) ? $"Diet/Pregnancy/all/{d.ImageSrc}" : "";
                });
            });

            return data;
        }
    }
}
