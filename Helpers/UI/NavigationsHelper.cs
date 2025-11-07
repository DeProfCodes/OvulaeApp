using OvulaeApp.Helpers.Enums;
using OvulaeApp.Services.LocalDataService;
using OvulaeApp.Views.About;
using OvulaeApp.Views.Authentication;
using OvulaeApp.Views.Dashboard;
using OvulaeApp.Views.Education;
using OvulaeApp.Views.MenopauseTracker.Dashboard;
using OvulaeApp.Views.OvulationTracker.Dashboard;
using OvulaeApp.Views.PeriodTracker.Dashboard;
using OvulaeApp.Views.PregnancyTracker.Dashboard;
using OvulaeApp.Views.Settings;
using OvulaeShared.Enums;
using OvulaeShared.Enums.App;

namespace OvulaeApp.Helpers.UI
{
    public class NavigationsHelper
    {
        public static string GetCommonAppPagePage(PageNameTypes page)
        {
            var moduleType = LocalStorageService.AppPrimaryGoal;

            switch (page)
            {
                // Calendar Pages
                case PageNameTypes.PregnancyDashboardCalendarPage: return nameof(PregnancyDashboardCalendarPage);
                case PageNameTypes.OvulationDashboardCalendarPage: return nameof(OvulationDashboardCalendarPage);
                case PageNameTypes.PeriodDashboardCalendarPage: return nameof(PeriodDashboardCalendarPage);
                case PageNameTypes.MenopauseDashboardCalendarPage: return nameof(MenopauseDashboardCalendarPage);
                // Home Pages
                case PageNameTypes.PregnancyDashboardHomePage: return nameof(PregnancyDashboardHomePage);
                case PageNameTypes.OvulationDashboardHomePage: return nameof(OvulationDashboardHomePage);
                case PageNameTypes.PeriodDashboardHomePage: return nameof(PeriodDashboardHomePage);
                case PageNameTypes.MenopauseDashboardHomePage: return nameof(MenopauseDashboardHomePage);
                // Notification Pages
                case PageNameTypes.PregnancyNotificationsPage: return nameof(PregnancyNotificationsPage);
                case PageNameTypes.OvulationNotificationsPage: return nameof(OvulationNotificationsPage);
                case PageNameTypes.PeriodNotificationsPage: return nameof(PeriodNotificationsPage);
                case PageNameTypes.MenopauseNotificationsPage: return nameof(MenopauseNotificationsPage);
                // Education
                case PageNameTypes.EducationMainPage: return nameof(EducationMainPage);
                case PageNameTypes.EducationDetailsPage: return nameof(EducationDetailsPage);
                // Other Pages
                case PageNameTypes.AboutUsPage: return nameof(AboutUsPage);
                case PageNameTypes.HelpPage: return nameof(HelpPage);
                case PageNameTypes.SettingsPage: return nameof(SettingsPage);
                case PageNameTypes.DashboardProfilePage: return nameof(DashboardProfilePage);
                case PageNameTypes.PartnerSharingPage: return nameof(PartnerSharingPage);

                default: return nameof(LoginPage);
            }
        }

        public static string GetCommonAppPagePageString(string pageName)
        {
            var pageType = EnumHelper.GetEnumValueFromShortName<PageNameTypes>(pageName);

            return GetCommonAppPagePage(pageType);
        }

        public static PageNameTypes GetTabToPageNameType(DashboardTab tab)
        {
            var moduleType = LocalStorageService.AppPrimaryGoal;

            if (tab == DashboardTab.Home)
            {
                if (moduleType == ModuleType.Pregnancy) return PageNameTypes.PregnancyDashboardHomePage;
                if (moduleType == ModuleType.Ovulation) return PageNameTypes.OvulationDashboardHomePage;
                if (moduleType == ModuleType.PeriodTracker) return PageNameTypes.PeriodDashboardHomePage;
                if (moduleType == ModuleType.MenopauseTracker) return PageNameTypes.MenopauseDashboardHomePage;
            }
            if (tab == DashboardTab.Track)
            {
                if (moduleType == ModuleType.Pregnancy) return PageNameTypes.PregnancyDashboardCalendarPage;
                if (moduleType == ModuleType.Ovulation) return PageNameTypes.OvulationDashboardCalendarPage;
                if (moduleType == ModuleType.PeriodTracker) return PageNameTypes.PeriodDashboardCalendarPage;
                if (moduleType == ModuleType.MenopauseTracker) return PageNameTypes.MenopauseDashboardCalendarPage;
            }
            if (tab == DashboardTab.Alerts)
            {
                if (moduleType == ModuleType.Pregnancy) return PageNameTypes.PregnancyNotificationsPage;
                if (moduleType == ModuleType.Ovulation) return PageNameTypes.OvulationNotificationsPage;
                if (moduleType == ModuleType.PeriodTracker) return PageNameTypes.PeriodNotificationsPage;
                if (moduleType == ModuleType.MenopauseTracker) return PageNameTypes.MenopauseNotificationsPage;
            }
            if (tab == DashboardTab.Learn) return PageNameTypes.EducationMainPage;
            if (tab == DashboardTab.Profile) return PageNameTypes.DashboardProfilePage;

            return PageNameTypes.LoginPage;
        }

        public static string GetLoaderMessage(PageNameTypes page)
        {
            switch (page)
            {
                // Calendar Pages
                case PageNameTypes.PregnancyDashboardCalendarPage: return "Preparing your pregnancy calendar page...";
                case PageNameTypes.OvulationDashboardCalendarPage: return "Preparing your fertility calendar page...";
                case PageNameTypes.PeriodDashboardCalendarPage: return "Preparing your period calendar page...";
                case PageNameTypes.MenopauseDashboardCalendarPage: return "Preparing your menopause calendar page...";
                // Home Pages
                case PageNameTypes.PregnancyDashboardHomePage: return "Loading pregnancy home...";
                case PageNameTypes.OvulationDashboardHomePage: return "Loading fertility home...";
                case PageNameTypes.PeriodDashboardHomePage: return "Loading period tracker home...";
                case PageNameTypes.MenopauseDashboardHomePage: return "Loading menopause tracker home...";
                // Notifications Pages
                case PageNameTypes.PregnancyNotificationsPage:
                case PageNameTypes.OvulationNotificationsPage:
                case PageNameTypes.MenopauseNotificationsPage:
                case PageNameTypes.PeriodNotificationsPage: return "Loading notifications page...";
                // Education
                case PageNameTypes.EducationMainPage: return "Loading education centre...";
                case PageNameTypes.EducationDetailsPage: return "Loading book details...";
                // Other Pages
                case PageNameTypes.AboutUsPage: return "Loading about page...";
                case PageNameTypes.HelpPage: return "Loading help page...";
                case PageNameTypes.SettingsPage: return "Loading settings page...";
                case PageNameTypes.DashboardProfilePage: return "Loading profile page...";
                case PageNameTypes.PartnerSharingPage: return "Loading partner sharing page...";
                //Authentication
                case PageNameTypes.LoginPage: return "Logging out...";
                
                default: return "Loading page...";
            }
        }

        public static int GetLoaderModalLines(PageNameTypes page)
        {
            switch (page)
            {
                case PageNameTypes.PregnancyDashboardCalendarPage:
                case PageNameTypes.OvulationDashboardCalendarPage:
                case PageNameTypes.MenopauseDashboardCalendarPage:
                case PageNameTypes.PeriodDashboardCalendarPage: return 2;

                default: return 1;
            }
        }

        public static int GetLoaderModalLinesFromName(string pageName)
        {
            PageNameTypes page = EnumHelper.GetEnumValueFromName<PageNameTypes>(pageName);

            return GetLoaderModalLines(page);
        }

        public static string GetLoaderMessageFromName(string pageName)
        {
            PageNameTypes page = EnumHelper.GetEnumValueFromName<PageNameTypes>(pageName);
            
            return GetLoaderMessage(page);
        }

        public static string GetDashboardPageNameFromModule(ModuleType moduleType)
        {
            LocalStorageService.AppPrimaryGoal = moduleType;
            switch (moduleType)
            {
                case ModuleType.Pregnancy: return nameof(PregnancyDashboardHomePage);
                case ModuleType.Ovulation: return nameof(OvulationDashboardHomePage);
                case ModuleType.PeriodTracker: return nameof(PeriodDashboardHomePage);
                case ModuleType.MenopauseTracker: return nameof(MenopauseDashboardHomePage);
                default: return nameof(LoginPage);
            }
        }

        public static string GetDashboardPageName(string appPrimaryGoal)
        {
            var moduleType = appPrimaryGoal != null ? EnumHelper.GetEnumValueFromName<ModuleType>(appPrimaryGoal) : ModuleType.PeriodTracker;
            return GetDashboardPageNameFromModule(moduleType);
        }
    }
}
