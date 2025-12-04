using OvulaeApp.Helpers.Enums;
using OvulaeShared.Enums.App;
using OvulaeShared.Models.Education;
using OvulaeShared.ViewModel.Education;

namespace OvulaeApp.Services.LocalDataService.EducationServices
{
    public interface IEducationService
    {
        public Task<bool> ReloadEducationDataAsync();

        public Task<List<EducationGroupItemsViewModel>> GetEducationDetailsServer(ModuleType moduleType);

        public List<EducationCoverGroup> GetModuleEducationGroupsCoversByType(ModuleType moduleType, EducationCategoryType educationType = EducationCategoryType.AllCategories);

        public List<EducationCover> GetCurrentWeekEducationCovers(ModuleType moduleType, int week);

        public List<EducationCoverGroup> GetEducationGroupsCovers(ModuleType moduleType);

        public EducationGroupItemsViewModel GetEducationBookDetails(int bookId);
    }
}
