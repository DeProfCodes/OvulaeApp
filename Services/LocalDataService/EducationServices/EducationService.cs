using OvulaeApp.Helpers.Enums;
using OvulaeApp.Helpers.Functions;
using OvulaeApp.Services.LocalDataService.Updates;
using OvulaeShared.Enums;
using OvulaeShared.Enums.App;
using OvulaeShared.Helpers.API;
using OvulaeShared.Services.APIs.Interface;
using OvulaeShared.ViewModel.Education;

namespace OvulaeApp.Services.LocalDataService.EducationServices
{
    public class EducationService : IEducationService
    {
        private readonly IUpdatesService _updatesServ;
        private readonly IWebInterfaceApiService _webApi;

        private const string EducationFile = "EducationData.json";

        public EducationService(IUpdatesService updatesServ, IWebInterfaceApiService webApi)
        {
            _updatesServ = updatesServ;
            _webApi = webApi;
        }

        public async Task<bool> ReloadEducationDataAsync()
        {
            try
            {
                var data = await FileReaderHelper.GetAllFileData<List<EducationGroupItemsViewModel>>(EducationFile);

                if (_updatesServ.PerformServerUpdateFeature(OvulaeFeatureType.Education) || data == null)
                {
                    data = await _webApi.GetData<List<EducationGroupItemsViewModel>>($"{OvulaeApiEndPoints.EDUCATION.GET_EDUCATION_DATA}?moduleType=All");
                    await FileReaderHelper.SaveDataAsync(data, EducationFile);
                }

                LocalStorageService.PregnancyData.AllEducationBooks = data;
                return true;
            }
            catch
            {
                LocalStorageService.PregnancyData.AllEducationBooks = null;
                return false;
            }
        }

        private List<EducationGroupItemsViewModel> GetModuleData(ModuleType moduleType)
        {
            var data = LocalStorageService.PregnancyData.AllEducationBooks
                                                .Where(x => (moduleType != ModuleType.All) ? x.Book.Type == moduleType.GetDisplayName() : true)
                                                .ToList();

            return data;
        }

        private List<int> GetBookmarkedBookIds()
        {
            //this will be fetch from storage or cloud
            var bookmarkedBooks = new List<int>() { 1 };

            return bookmarkedBooks;
        }

        private List<EducationCover> GetEducationCoversFromEducationBook(List<EducationGroupItemsViewModel> educationBooks, List<int> BookedMarkedBookIds = null)
        {
            var result = new List<EducationCover>();

            foreach (var pregEd in educationBooks)
            {
                var cover = new EducationCover()
                {
                    BookId = pregEd.Book.Id,
                    ThumbnailSource = pregEd.Book.CoverImgSrc,
                    Categories = pregEd.Book.Categories,
                    Title = pregEd.Book.Title,
                    Description = pregEd.Book.Description,
                    Type = pregEd.Book.Type,
                };
                cover.Bookmarked = BookedMarkedBookIds != null && BookedMarkedBookIds.Contains(pregEd.Book.Id);

                result.Add(cover);
            }
            return result;
        }

        public async Task<List<EducationGroupItemsViewModel>> GetEducationDetailsServer(ModuleType moduleType)
        {
            try
            {
                var data = await _webApi.GetData<List<EducationGroupItemsViewModel>>($"{OvulaeApiEndPoints.EDUCATION.GET_EDUCATION_DATA}?moduleType={moduleType.GetDisplayName()}");
                return data;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public List<EducationCover> GetCurrentWeekEducationCovers(ModuleType moduleType, int week)
        {
            var weekPregEdData = GetModuleData(moduleType).Where(d => week > 0 ? d.Book.Weeks.Contains(week) : true).ToList();

            var bookmarkedBooks = GetBookmarkedBookIds();
            var result = GetEducationCoversFromEducationBook(weekPregEdData, bookmarkedBooks);

            return result;
        }

        public List<EducationCoverGroup> GetModuleEducationGroupsCoversByType(ModuleType moduleType, EducationCategoryType educationType)
        {
            var result = new List<EducationCoverGroup>();

            try
            {
                var data = GetModuleData(moduleType);
                var bookmarkedBooksIds = GetBookmarkedBookIds();

                if (educationType == EducationCategoryType.Recommended)
                {
                    var recommended = new List<EducationCover>();

                    data = data.Where(x => x.Book.Type == moduleType.GetDisplayName()).ToList();

                    if (moduleType == ModuleType.Pregnancy)
                    {
                        recommended = GetCurrentWeekEducationCovers(moduleType, LocalStorageService.PregnancyData.PregnancyDataCurrentWeek);
                    }
                    else
                    {
                        recommended = GetEducationCoversFromEducationBook(data, bookmarkedBooksIds);
                    }

                    if (recommended != null && recommended.Count > 0)
                    {
                        result.Add(new EducationCoverGroup()
                        {
                            CoversHeading = "Top Recommended for you",
                            EducationCovers = recommended
                        });
                    }
                }
                else if (educationType == EducationCategoryType.Bookmarks)
                {
                    var bookmarkedEduBooks = data.Where(x => bookmarkedBooksIds.Contains(x.Book.Id)).ToList();
                    var bookmarked = GetEducationCoversFromEducationBook(bookmarkedEduBooks, bookmarkedBooksIds);

                    if (bookmarked != null && bookmarked.Count > 0)
                    {
                        result.Add(new EducationCoverGroup()
                        {
                            CoversHeading = "Bookmarks",
                            EducationCovers = bookmarked
                        });
                    }
                }
                else if (educationType == EducationCategoryType.AllCategories)
                {
                    var bookTypes = data.Select(x => x.Book.Type).Distinct().ToList();

                    foreach (var type in bookTypes)
                    {
                        var moduleTypeBooks = data.Where(x => x.Book.Categories.Contains(type)).ToList();
                        var moduleCovers = GetEducationCoversFromEducationBook(moduleTypeBooks, bookmarkedBooksIds);

                        var coverGroup = new EducationCoverGroup()
                        {
                            CoversHeading = type,
                            EducationCovers = moduleCovers
                        };
                        result.Add(coverGroup);
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return result;
        }

        private string GetEducationGroupTitle(string type)
        {
            if (type.ToLower() == "other" || type.ToLower() == "ovulaeeducation") return "Other";
            
            var moduleType = EnumHelper.GetEnumValueFromName<ModuleType>(type);

            if (moduleType == ModuleType.Pregnancy) return "Pregnancy Education";
            if (moduleType == ModuleType.Ovulation) return "Fertility & Ovulation Education";
            if (moduleType == ModuleType.PeriodTracker) return "Menstrual Cycle Education";
            if (moduleType == ModuleType.MenopauseTracker) return "Menopause Education";

            return "Other";
        }

        public List<EducationCoverGroup> GetAllModulesEducationGroupsCoversByType(List<int> booksIdsExclude, ModuleType moduleType)
        {
            var result = new List<EducationCoverGroup>();

            var data = GetModuleData(ModuleType.All).Where(x => !booksIdsExclude.Contains(x.Book.Id));
            var bookTypes = data.Select(x => x.Book.Type).Distinct().OrderByDescending(type => type == moduleType.GetDisplayName()).ToList();

            foreach (var type in bookTypes)
            {
                var moduleTypeBooks = data.Where(x => x.Book.Type == type).ToList();
                var moduleCovers = GetEducationCoversFromEducationBook(moduleTypeBooks);

                var coverGroup = new EducationCoverGroup()
                {
                    CoversHeading = GetEducationGroupTitle(type),
                    EducationCovers = moduleCovers
                };
                result.Add(coverGroup);
            }
            return result;
        }

        public List<EducationCoverGroup> GetEducationGroupsCovers(ModuleType moduleType) 
        {
            var result = new List<EducationCoverGroup>();

            try
            {
                var moduleRecommendedEd = GetModuleEducationGroupsCoversByType(moduleType, EducationCategoryType.Recommended);
                
                var recommendedBookIds = moduleRecommendedEd.SelectMany(x => x.EducationCovers.Select(x => x.BookId)).ToList();
                
                var allModulesEd = GetAllModulesEducationGroupsCoversByType(recommendedBookIds, moduleType);
                
                result.AddRange(moduleRecommendedEd);
                result.AddRange(allModulesEd);
            }
            catch (Exception ex) 
            {

            }
            return result;
        }

        public EducationGroupItemsViewModel GetEducationBookDetails(int bookId)
        {
            var data = GetModuleData(ModuleType.All);

            var result = data.FirstOrDefault(x => x.Book.Id == bookId);

            return result;
        }
    }
}
