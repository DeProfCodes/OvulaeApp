using Microsoft.Maui.Storage;
using Newtonsoft.Json;

using OvulaeShared.Enums.App;
using OvulaeShared.Helpers.API;
using OvulaeShared.Models.Shared;
using OvulaeShared.ViewModel.Diet;
using OvulaeShared.ViewModel.Education;
using OvulaeShared.ViewModel.Pregnancy;
using OvulaeShared.ViewModel.Symptoms;
using OvulaeShared.ViewModel.Tips;

namespace OvulaeApp.Helpers.Functions
{
    public class FileReaderHelper
    {
        public const string UPDATE_HISTORY_FILE = "UpdateHistory.json";

        private static string GetFolderForModuleType(ModuleType module)
        {
            if (module == ModuleType.Pregnancy) return "PregnancyJsons";
            return string.Empty;
        }

        private static string GetFileForTypeAndModule(ModuleType module, OvulaeFeatureType feature)
        {
            if (module == ModuleType.Pregnancy)
            {
                if (feature == OvulaeFeatureType.Dashboard) return $"PregnancyRawData.json";
                if (feature == OvulaeFeatureType.Education) return $"PregEdRawData.json";
                if (feature == OvulaeFeatureType.Diet) return $"PregDietRawData.json";
                if (feature == OvulaeFeatureType.Symptoms) return $"PregSymptomsRawData.json";
                if (feature == OvulaeFeatureType.Tips) return $"PregTipsRawData.json";
            }
            return string.Empty;
        }

        private static async Task<string> EnsureJsonDataFilePathAsync(string mobilePath, string projectPath)
        {
            if (!File.Exists(mobilePath))
            {
                using Stream stream = await FileSystem.OpenAppPackageFileAsync(projectPath);
                using FileStream destStream = File.Create(mobilePath);
                await stream.CopyToAsync(destStream);
            }

            return mobilePath;
        }

        private static async Task<string> EnsureJsonDataFileMobileOnlyAsync(string fileName)
        {
            var fullPath = Path.Combine(FileSystem.AppDataDirectory, fileName);

            if (!File.Exists(fullPath))
            {
                var initialContent = "[]"; 
                await File.WriteAllTextAsync(fullPath, initialContent);
            }
            return fullPath;
        }

        private static async Task<string> EnsureJsonDataFileAsync(string folderName = "", string fileName = "")
        {
            var destinationPath = Path.Combine(FileSystem.AppDataDirectory, fileName);
            var projectPath = $"Helpers/Files{(!string.IsNullOrEmpty(folderName) ?$"/{folderName}":"")}/{fileName}";

            return await EnsureJsonDataFilePathAsync(destinationPath, projectPath);
        }

        private static async Task<T> GetJsonDataFromPath<T>(string filePath)
        {
            if (!File.Exists(filePath))
            {
                Console.WriteLine($"File not found: {filePath}");
                return default(T);
            }

            try
            {
                var jsonData = await File.ReadAllTextAsync(filePath);
                var pregnancyData = APIResponseParserHelper.ParseJsonToObject<T>(jsonData);

                return pregnancyData;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading/parsing file: {ex.Message}");
                return default(T);
            }
        }

        private static async Task<T> GetJsonData<T>(ModuleType moduleType, OvulaeFeatureType featureType)
        {
            var folderName = GetFolderForModuleType(moduleType);
            var fileName = GetFileForTypeAndModule(moduleType, featureType);
            var filePath = await EnsureJsonDataFileAsync(folderName, fileName);

            return await GetJsonDataFromPath<T>(filePath);
        }

        public static async Task<bool> SaveDataFromPathAsync<T>(T data, string fullPath)
        {
            try
            {
                string json = JsonConvert.SerializeObject(data, Formatting.Indented);
                string directory = Path.GetDirectoryName(fullPath);

                if (!Directory.Exists(directory))
                    Directory.CreateDirectory(directory);

                await File.WriteAllTextAsync(fullPath, json);
                Console.WriteLine($"✅ data saved to: {fullPath}");

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Failed to save Data: {ex.Message}");
                return false;
            }
        }

        public static async Task<bool> SaveDataAsync<T>(T data, ModuleType moduleType, OvulaeFeatureType featureType)
        {
            var filename = GetFileForTypeAndModule(moduleType, featureType);

            string fullPath = Path.Combine(FileSystem.AppDataDirectory, filename);

            return await SaveDataFromPathAsync<T>(data, fullPath);
        }

        public static async Task<bool> SaveDataAsync<T>(T data, string filename)
        {
            string fullPath = Path.Combine(FileSystem.AppDataDirectory, filename);

            return await SaveDataFromPathAsync<T>(data, fullPath);
        }

        public static async Task<bool> SaveUpdateHistoryAsync(List<UpdateHistory> data)
        {
            var path = await EnsureJsonDataFileAsync("", UPDATE_HISTORY_FILE);
            return await SaveDataFromPathAsync<List<UpdateHistory>>(data, path);
        }

        public static async Task<List<UpdateHistory>> GetAllUpdateHistory()
        {
            var path = await EnsureJsonDataFileAsync("", UPDATE_HISTORY_FILE);
            var allUpdateHistory = await GetJsonDataFromPath<List<UpdateHistory>>(path);

            return allUpdateHistory;
        }

        public static async Task<T> GetAllFileData<T>(string filename)
        {
            string fullPath = Path.Combine(FileSystem.AppDataDirectory, filename);
            var allData = await GetJsonDataFromPath<T>(fullPath);

            return allData;
        }

        public static async Task<List<PregnancyDataGroupViewModel>> GetModulaDashboardData(ModuleType moduleType)
        {
            var data = await GetJsonData<List<PregnancyDataGroupViewModel>>(moduleType, OvulaeFeatureType.Dashboard);

            return data;
        }

        public static async Task<List<EducationGroupItemsViewModel>> GetEducationBooks(ModuleType moduleType)
        {
            var data = await GetJsonData<List<EducationGroupItemsViewModel>>(moduleType, OvulaeFeatureType.Education);

            return data;
        }

        public static async Task<List<DietGroupItemsViewModel>> GetDietDetails(ModuleType moduleType)
        {
            var data = await GetJsonData<List<DietGroupItemsViewModel>>(moduleType, OvulaeFeatureType.Diet);

            return data;
        }

        public static async Task<List<SymptomsGroupItemsViewModel>> GetSymptomsDetails(ModuleType moduleType)
        {
            var data = await GetJsonData<List<SymptomsGroupItemsViewModel>>(moduleType, OvulaeFeatureType.Symptoms);

            return data;
        }

        public static async Task<List<TipsGroupItemsViewModel>> GetTipsDetails(ModuleType moduleType)
        {
            var data = await GetJsonData<List<TipsGroupItemsViewModel>>(moduleType, OvulaeFeatureType.Tips);

            return data;
        }
    }
}
