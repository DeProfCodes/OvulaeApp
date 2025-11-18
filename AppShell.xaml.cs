using OvulaeApp.Views.About;
using OvulaeApp.Views.Authentication;
using OvulaeApp.Views.Dashboard;
using OvulaeApp.Views.Education;
using OvulaeApp.Views.GoalSetting;
using OvulaeApp.Views.MenopauseTracker.Dashboard;
using OvulaeApp.Views.MenopauseTracker.Onboarding;
using OvulaeApp.Views.OvulationTracker.Dashboard;
using OvulaeApp.Views.OvulationTracker.Onboarding;
using OvulaeApp.Views.PeriodTracker.Dashboard;
using OvulaeApp.Views.PeriodTracker.Onboarding;
using OvulaeApp.Views.PregnancyTracker.Dashboard;
using OvulaeApp.Views.PregnancyTracker.Onboarding;
using OvulaeApp.Views.PrivacyPolicy;
using OvulaeApp.Views.Settings;
using OvulaeApp.Views.Settings.EditSettingsPages.GeneralSettings;
using OvulaeApp.Views.Settings.EditSettingsPages.MenopauseSettings;
using OvulaeApp.Views.Settings.EditSettingsPages.OvulationSettings;
using OvulaeApp.Views.Settings.EditSettingsPages.PeriodSettings;
using OvulaeApp.Views.Settings.EditSettingsPages.PregnancySettings;
using OvulaeApp.Views.Settings.EditSettingsPages.SecuritySettings;
using OvulaeApp.Views.Splashscreen;
using OvulaeApp.Views.Subscription;

namespace OvulaeApp
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            RegisterRoutes();
        }

        private void RegisterRoutes()
        {
            //Splashscreen Pages
            Routing.RegisterRoute(nameof(SplashWelcomePage), typeof(SplashWelcomePage));
            Routing.RegisterRoute(nameof(SplashPrivacyPage), typeof(SplashPrivacyPage));
            Routing.RegisterRoute(nameof(SplashscreenRenderPage), typeof(SplashscreenRenderPage));

            //Privacy Pages
            Routing.RegisterRoute(nameof(PrivacyPolicyPage), typeof(PrivacyPolicyPage));
            Routing.RegisterRoute(nameof(TermsOfUsePage), typeof(TermsOfUsePage));

            //Authentication Pages
            Routing.RegisterRoute(nameof(LoginPage), typeof(LoginPage));
            Routing.RegisterRoute(nameof(PasswordResetMobilePage), typeof(PasswordResetMobilePage));
            Routing.RegisterRoute(nameof(PasswordResetEmailPage), typeof(PasswordResetEmailPage));
            Routing.RegisterRoute(nameof(PasswordResetCodePage), typeof(PasswordResetCodePage));
            Routing.RegisterRoute(nameof(SignUpEmailPage), typeof(SignUpEmailPage));
            Routing.RegisterRoute(nameof(SignUpInfoPage), typeof(SignUpInfoPage));

            //Goal Setting Pages
            Routing.RegisterRoute(nameof(BodyMetricsPage), typeof(BodyMetricsPage));
            Routing.RegisterRoute(nameof(GoalSettingPage), typeof(GoalSettingPage));

            //Subscription Pages
            Routing.RegisterRoute(nameof(PaymentWallPage), typeof(PaymentWallPage));
            Routing.RegisterRoute(nameof(PaymentFailedPage), typeof(PaymentFailedPage));
            Routing.RegisterRoute(nameof(PaymentWalliOSPage), typeof(PaymentWalliOSPage));
            Routing.RegisterRoute(nameof(PaymentFailedPageIOS), typeof(PaymentFailedPageIOS));

            //Pregnancy Tracker
            //Onboard
            Routing.RegisterRoute(nameof(PregnancyOnboardWelcomePage), typeof(PregnancyOnboardWelcomePage));
            Routing.RegisterRoute(nameof(PregnancyOnboardRenderPage), typeof(PregnancyOnboardRenderPage));
            Routing.RegisterRoute(nameof(PregnancyOnboardFinishPage), typeof(PregnancyOnboardFinishPage));
            //Dashboard
            Routing.RegisterRoute(nameof(PregnancyDashboardHomePage), typeof(PregnancyDashboardHomePage));
            Routing.RegisterRoute(nameof(PregnancyDashboardBabyInfoPage), typeof(PregnancyDashboardBabyInfoPage));
            Routing.RegisterRoute(nameof(PregnancyDashboardSymptomsPage), typeof(PregnancyDashboardSymptomsPage));
            Routing.RegisterRoute(nameof(PregnancyDashboardTipsPage), typeof(PregnancyDashboardTipsPage));
            Routing.RegisterRoute(nameof(PregnancyDashboardDayLoggerPage), typeof(PregnancyDashboardDayLoggerPage));
            Routing.RegisterRoute(nameof(PregnancyDashboardCalendarPage), typeof(PregnancyDashboardCalendarPage));
            Routing.RegisterRoute(nameof(PregnancyNotificationsPage), typeof(PregnancyNotificationsPage));

            //Ovulation Tracker
            //Onboard
            Routing.RegisterRoute(nameof(OvulationOnboardWelcomePage), typeof(OvulationOnboardWelcomePage));
            Routing.RegisterRoute(nameof(OvulationOnboardRenderPage), typeof(OvulationOnboardRenderPage));
            Routing.RegisterRoute(nameof(OvulationOnboardFinishPage), typeof(OvulationOnboardFinishPage));
            //Dashboard
            Routing.RegisterRoute(nameof(OvulationDashboardHomePage), typeof(OvulationDashboardHomePage));
            Routing.RegisterRoute(nameof(OvulationPhaseDetailsPage), typeof(OvulationPhaseDetailsPage));
            Routing.RegisterRoute(nameof(OvulationDashboardDayLoggerPage), typeof(OvulationDashboardDayLoggerPage));
            Routing.RegisterRoute(nameof(OvulationDashboardCalendarPage), typeof(OvulationDashboardCalendarPage));
            Routing.RegisterRoute(nameof(OvulationNotificationsPage), typeof(OvulationNotificationsPage));

            //Period Tracker
            //Onboard
            Routing.RegisterRoute(nameof(PeriodOnboardWelcomePage), typeof(PeriodOnboardWelcomePage));
            Routing.RegisterRoute(nameof(PeriodOnboardRenderPage), typeof(PeriodOnboardRenderPage));
            Routing.RegisterRoute(nameof(PeriodOnboardFinishPage), typeof(PeriodOnboardFinishPage));
            //Dashboard
            Routing.RegisterRoute(nameof(PeriodDashboardHomePage), typeof(PeriodDashboardHomePage));
            Routing.RegisterRoute(nameof(PeriodPhaseDetailsPage), typeof(PeriodPhaseDetailsPage));
            Routing.RegisterRoute(nameof(PeriodDashboardDayLoggerPage), typeof(PeriodDashboardDayLoggerPage));
            Routing.RegisterRoute(nameof(PeriodDashboardCalendarPage), typeof(PeriodDashboardCalendarPage));
            Routing.RegisterRoute(nameof(PeriodNotificationsPage), typeof(PeriodNotificationsPage));

            //Menopause Tracker
            //Onboard
            Routing.RegisterRoute(nameof(MenopauseOnboardWelcomePage), typeof(MenopauseOnboardWelcomePage));
            Routing.RegisterRoute(nameof(MenopauseOnboardRenderPage), typeof(MenopauseOnboardRenderPage));
            Routing.RegisterRoute(nameof(MenopauseOnboardFinishPage), typeof(MenopauseOnboardFinishPage));
            //Dashboard
            Routing.RegisterRoute(nameof(MenopauseDashboardHomePage), typeof(MenopauseDashboardHomePage));
            Routing.RegisterRoute(nameof(MenopauseDashboardDayLoggerPage), typeof(MenopauseDashboardDayLoggerPage));
            Routing.RegisterRoute(nameof(MenopauseDashboardCalendarPage), typeof(MenopauseDashboardCalendarPage));
            Routing.RegisterRoute(nameof(MenopauseStageDetailsPage), typeof(MenopauseStageDetailsPage));
            Routing.RegisterRoute(nameof(MenopauseNotificationsPage), typeof(MenopauseNotificationsPage));

            //Education
            Routing.RegisterRoute(nameof(EducationDetailsPage), typeof(EducationDetailsPage));
            Routing.RegisterRoute(nameof(EducationMainPage), typeof(EducationMainPage));
            Routing.RegisterRoute(nameof(EducationReferencesPage), typeof(EducationReferencesPage));

            //Dashboard
            Routing.RegisterRoute(nameof(CalendarTrackingPage), typeof(CalendarTrackingPage));
            Routing.RegisterRoute(nameof(NotificationsSettingsPage), typeof(NotificationsSettingsPage));
            Routing.RegisterRoute(nameof(DashboardProfilePage), typeof(DashboardProfilePage));
            Routing.RegisterRoute(nameof(PartnerSharingPage), typeof(PartnerSharingPage));
            Routing.RegisterRoute(nameof(NotificationAppLoad), typeof(NotificationAppLoad));

            //Settings
            Routing.RegisterRoute(nameof(SettingsPage), typeof(SettingsPage));
            Routing.RegisterRoute(nameof(EditSettingsPersonalInfoPage), typeof(EditSettingsPersonalInfoPage));
            Routing.RegisterRoute(nameof(EditSettingsHealthInfoPage), typeof(EditSettingsHealthInfoPage));
            Routing.RegisterRoute(nameof(EditSettingsMembershipInfoPage), typeof(EditSettingsMembershipInfoPage));
            Routing.RegisterRoute(nameof(EditSettingsAffiliateInfoPage), typeof(EditSettingsAffiliateInfoPage));
            Routing.RegisterRoute(nameof(EditSettingsPregnancyDatesPage), typeof(EditSettingsPregnancyDatesPage));
            Routing.RegisterRoute(nameof(EditSettingsPregnancyBabyInfoPage), typeof(EditSettingsPregnancyBabyInfoPage));
            Routing.RegisterRoute(nameof(EditSettingsPeriodCyclePage), typeof(EditSettingsPeriodCyclePage));
            Routing.RegisterRoute(nameof(EditSettingsOvulationPage), typeof(EditSettingsOvulationPage));
            Routing.RegisterRoute(nameof(EditMenopauseProfilePage), typeof(EditMenopauseProfilePage));
            Routing.RegisterRoute(nameof(EditSettingsSecurityPage), typeof(EditSettingsSecurityPage));

            //About
            Routing.RegisterRoute(nameof(AboutUsPage), typeof(AboutUsPage));
            Routing.RegisterRoute(nameof(HelpPage), typeof(HelpPage));


        }
    }
}
