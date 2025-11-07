using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime;
using System.Text;
using System.Threading.Tasks;
using OvulaeApp.Helpers.Functions;
using OvulaeApp.Models.Notifications;
using OvulaeApp.Services.LocalDataService.CycleServices;
using OvulaeApp.Services.LocalDataService.UsersServices;
using OvulaeApp.Services.Notifications;
using OvulaeShared.Enums.App;
using Shiny.Jobs;

namespace OvulaeApp.Services.Jobs.Daily
{
    public class DailyCycleJob : IJob
    {
        private readonly INotificationSchedulerService _scheduler;
        private readonly INotificationsPreferenceService _notificationsPrefService;
        private readonly ICycleService _cycleServ;

        public DailyCycleJob(INotificationSchedulerService scheduler, INotificationsPreferenceService notificationsPrefService, ICycleService cycleServ)
        {
            _scheduler = scheduler;
            _notificationsPrefService = notificationsPrefService;
            _cycleServ = cycleServ;
        }

        public async Task Run(JobInfo jobInfo, CancellationToken cancelToken)
        {
            await _cycleServ.LoadCycleDataAsync();

            var moduleType = _cycleServ.GetModuleType();

            List<ScheduledNotification> dayPlan;

            if (moduleType == ModuleType.Ovulation || moduleType == ModuleType.PeriodTracker)
            {
                var prefs = await _notificationsPrefService.GetPeriodOvulationNotifications();
                dayPlan = _cycleServ.BuildDayPlan(DateTime.Today, prefs);
            }
            else if (moduleType == ModuleType.Pregnancy)
            {
                dayPlan = await _notificationsPrefService.BuildFullWeekPlanForPregnancyNotifications();
            }
            else
            {
                dayPlan = new List<ScheduledNotification>();
            }

            await _scheduler.SchedulePlanAsync(dayPlan);
        }
    }
}
