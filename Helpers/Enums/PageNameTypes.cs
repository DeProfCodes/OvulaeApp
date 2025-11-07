using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OvulaeApp.Helpers.Enums
{
    public enum PageNameTypes
    {
        [Display(Name = "", ShortName = "", Description = "")]
        None,

        #region Splashscreen

        [Display(Name = "SplashWelcomePage", ShortName = "SplashWelcomePage", GroupName = "Splash", Description = "")]
        SplashWelcomePage,

        [Display(Name = "SplashPrivacyPage", ShortName = "SplashPrivacyPage", GroupName = "Splash", Description = "")]
        SplashPrivacyPage,

        [Display(Name = "SplashscreenRenderPage", ShortName = "SplashscreenRenderPage", GroupName = "Splash", Description = "")]
        SplashscreenRenderPage,

        #endregion

        #region Privacy

        [Display(Name = "PrivacyPolicyPage", ShortName = "PrivacyPolicy", GroupName = "Privacy", Description = "")]
        PrivacyPolicyPage,

        [Display(Name = "TermsOfUsePage", ShortName = "Terms", GroupName = "Privacy", Description = "")]
        TermsOfUsePage,

        #endregion

        #region Authentication

        [Display(Name = "LoginPage", ShortName = "Login", GroupName = "Authentication", Description = "")]
        LoginPage,

        [Display(Name = "PasswordResetMobilePage", ShortName = "PasswordResetMobile", GroupName = "Authentication", Description = "")]
        PasswordResetMobilePage,

        [Display(Name = "PasswordResetEmailPage", ShortName = "PasswordResetEmail", GroupName = "Authentication", Description = "")]
        PasswordResetEmailPage,

        [Display(Name = "PasswordResetCodePage", ShortName = "PasswordResetCode", GroupName = "Authentication", Description = "")]
        PasswordResetCodePage,

        [Display(Name = "SignUpEmailPage", ShortName = "SignUpEmail", GroupName = "Authentication", Description = "")]
        SignUpEmailPage,

        [Display(Name = "SignUpInfoPage", ShortName = "SignUpInfo", GroupName = "Authentication", Description = "")]
        SignUpInfoPage,

        #endregion

        #region Goal Setting

        [Display(Name = "GoalSettingPage", ShortName = "GoalSetting", GroupName = "Goal", Description = "")]
        GoalSettingPage,

        #endregion

        #region Pregnancy

        [Display(Name = "PregnancyOnboardWelcomePage", ShortName = "PregnancyOnboardWelcome", GroupName = "Pregnancy", Description = "")]
        PregnancyOnboardWelcomePage,

        [Display(Name = "PregnancyOnboardRenderPage", ShortName = "PregnancyOnboardRender", GroupName = "Pregnancy", Description = "")]
        PregnancyOnboardRenderPage,

        [Display(Name = "PregnancyOnboardFinishPage", ShortName = "PregnancyOnboardFinish", GroupName = "Pregnancy", Description = "")]
        PregnancyOnboardFinishPage,

        [Display(Name = "PregnancyDashboardHomePage", ShortName = "PregnancyHome", GroupName = "Pregnancy", Description = "")]
        PregnancyDashboardHomePage,

        [Display(Name = "PregnancyDashboardBabyInfoPage", ShortName = "PregnancyBabyInfo", GroupName = "Pregnancy", Description = "")]
        PregnancyDashboardBabyInfoPage,

        [Display(Name = "PregnancyDashboardSymptomsPage", ShortName = "PregnancySymptoms", GroupName = "Pregnancy", Description = "")]
        PregnancyDashboardSymptomsPage,

        [Display(Name = "PregnancyDashboardTipsPage", ShortName = "PregnancyTips", GroupName = "Pregnancy", Description = "")]
        PregnancyDashboardTipsPage,

        [Display(Name = "PregnancyDashboardDayLoggerPage", ShortName = "PregnancyDayLogger", GroupName = "Pregnancy", Description = "")]
        PregnancyDashboardDayLoggerPage,

        [Display(Name = "PregnancyDashboardCalendarPage", ShortName = "PregnancyCalendar", GroupName = "Pregnancy", Description = "")]
        PregnancyDashboardCalendarPage,

        [Display(Name = "PregnancyNotificationsPage", ShortName = "PregnancyNotifications", GroupName = "Pregnancy", Description = "")]
        PregnancyNotificationsPage,

        #endregion

        #region Ovulation

        [Display(Name = "OvulationOnboardWelcomePage", ShortName = "OvulationOnboardWelcome", GroupName = "Ovulation", Description = "")]
        OvulationOnboardWelcomePage,

        [Display(Name = "OvulationOnboardRenderPage", ShortName = "OvulationOnboardRender", GroupName = "Ovulation", Description = "")]
        OvulationOnboardRenderPage,

        [Display(Name = "OvulationOnboardFinishPage", ShortName = "OvulationOnboardFinish", GroupName = "Ovulation", Description = "")]
        OvulationOnboardFinishPage,

        [Display(Name = "OvulationDashboardHomePage", ShortName = "OvulationHome", GroupName = "Ovulation", Description = "")]
        OvulationDashboardHomePage,

        [Display(Name = "OvulationPhaseDetailsPage", ShortName = "OvulationPhaseDetails", GroupName = "Ovulation", Description = "")]
        OvulationPhaseDetailsPage,

        [Display(Name = "OvulationDashboardDayLoggerPage", ShortName = "OvulationDayLogger", GroupName = "Ovulation", Description = "")]
        OvulationDashboardDayLoggerPage,

        [Display(Name = "OvulationDashboardCalendarPage", ShortName = "OvulationCalendar", GroupName = "Ovulation", Description = "")]
        OvulationDashboardCalendarPage,

        [Display(Name = "OvulationNotificationsPage", ShortName = "OvulationNotifications", GroupName = "Ovulation", Description = "")]
        OvulationNotificationsPage,

        #endregion

        #region Period Tracker

        [Display(Name = "PeriodOnboardWelcomePage", ShortName = "PeriodOnboardWelcome", GroupName = "PeriodTracker", Description = "")]
        PeriodOnboardWelcomePage,

        [Display(Name = "PeriodOnboardRenderPage", ShortName = "PeriodOnboardRender", GroupName = "PeriodTracker", Description = "")]
        PeriodOnboardRenderPage,

        [Display(Name = "PeriodOnboardFinishPage", ShortName = "PeriodOnboardFinish", GroupName = "PeriodTracker", Description = "")]
        PeriodOnboardFinishPage,

        [Display(Name = "PeriodDashboardHomePage", ShortName = "PeriodHome", GroupName = "PeriodTracker", Description = "")]
        PeriodDashboardHomePage,

        [Display(Name = "PeriodPhaseDetailsPage", ShortName = "PeriodPhaseDetails", GroupName = "PeriodTracker", Description = "")]
        PeriodPhaseDetailsPage,

        [Display(Name = "PeriodDashboardDayLoggerPage", ShortName = "PeriodDayLogger", GroupName = "PeriodTracker", Description = "")]
        PeriodDashboardDayLoggerPage,

        [Display(Name = "PeriodDashboardCalendarPage", ShortName = "PeriodCalendar", GroupName = "PeriodTracker", Description = "")]
        PeriodDashboardCalendarPage,

        [Display(Name = "PeriodNotificationsPage", ShortName = "PeriodNotifications", GroupName = "PeriodTracker", Description = "")]
        PeriodNotificationsPage,


        #endregion

        #region Menopause Tracker

        [Display(Name = "MenopauseOnboardWelcomePage", ShortName = "MenopauseOnboardWelcome", GroupName = "MenopauseTracker", Description = "")]
        MenopauseOnboardWelcomePage,

        [Display(Name = "MenopauseOnboardRenderPage", ShortName = "MenopauseOnboardRender", GroupName = "MenopauseTracker", Description = "")]
        MenopauseOnboardRenderPage,

        [Display(Name = "MenopauseOnboardFinishPage", ShortName = "MenopauseOnboardFinish", GroupName = "MenopauseTracker", Description = "")]
        MenopauseOnboardFinishPage,

        [Display(Name = "MenopauseDashboardHomePage", ShortName = "MenopauseHome", GroupName = "MenopauseTracker", Description = "")]
        MenopauseDashboardHomePage,

        [Display(Name = "MenopausePhaseDetailsPage", ShortName = "MenopausePhaseDetails", GroupName = "MenopauseTracker", Description = "")]
        MenopausePhaseDetailsPage,

        [Display(Name = "PeriodDashboardDayLoggerPage", ShortName = "MenopauseDayLogger", GroupName = "MenopauseTracker", Description = "")]
        MenopauseDashboardDayLoggerPage,

        [Display(Name = "MenopauseDashboardCalendarPage", ShortName = "MenopauseCalendar", GroupName = "MenopauseTracker", Description = "")]
        MenopauseDashboardCalendarPage,

        [Display(Name = "MenopauseNotificationsPage", ShortName = "MenopauseNotifications", GroupName = "MenopauseTracker", Description = "")]
        MenopauseNotificationsPage,

        #endregion

        #region Education

        [Display(Name = "EducationDetailsPage", ShortName = "EducationDetails", GroupName = "Education", Description = "")]
        EducationDetailsPage,

        [Display(Name = "EducationMainPage", ShortName = "EducationMain", GroupName = "Education", Description = "")]
        EducationMainPage,

        #endregion

        #region Other Dashboards

        [Display(Name = "CalendarTrackingPage", ShortName = "CalendarTracking", GroupName = "Dashboard", Description = "")]
        CalendarTrackingPage,

        [Display(Name = "NotificationsSettingsPage", ShortName = "EducationMain", GroupName = "Dashboard", Description = "")]
        NotificationsSettingsPage,

        [Display(Name = "SettingsPage", ShortName = "Settings", GroupName = "Dashboard", Description = "")]
        SettingsPage,

        [Display(Name = "DashboardProfilePage", ShortName = "Profile", GroupName = "Dashboard", Description = "")]
        DashboardProfilePage,

        [Display(Name = "PartnerSharingPage", ShortName = "PartnerSharing", GroupName = "Dashboard", Description = "")]
        PartnerSharingPage,

        #endregion

        #region About

        [Display(Name = "AboutUsPage", ShortName = "AboutUs", GroupName = "About", Description = "")]
        AboutUsPage,

        [Display(Name = "HelpPage", ShortName = "Help", GroupName = "About", Description = "")]
        HelpPage,

        #endregion

    }
}
