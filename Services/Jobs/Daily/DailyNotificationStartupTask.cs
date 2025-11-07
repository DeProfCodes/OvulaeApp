using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shiny.Jobs;
using Shiny;

namespace OvulaeApp.Services.Jobs.Daily
{
    public class DailyNotificationStartupTask : IShinyStartupTask
    {
        private readonly DailyCycleJob _job;

        public DailyNotificationStartupTask(DailyCycleJob job)
        {
            _job = job;
        }

        public async void Start()
        {
            var jobInfo = new JobInfo(
                                Identifier: "manual.cycle.job",
                                JobType: typeof(DailyCycleJob),
                                Parameters: null,
                                RequiredInternetAccess: InternetAccess.None,
                                BatteryNotLow: false,
                                DeviceCharging: false
                            );

            await _job.Run(jobInfo, CancellationToken.None);
        }
    }
}
