using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OvulaeApp.Models.Notifications
{
    public class PregnancyNotificationPreferences
    {
        public bool UseOwnTime { get; set; }
        public TimeSpan PreferredNotificationTime { get; set; } = TimeSpan.FromHours(12);

        // 📅 Core Pregnancy Tracking
        public bool NotifyWeeklyUpdates { get; set; } = true;
        public bool NotifyLogSymptoms { get; set; } = true;
        public bool NotifyEducationalTips { get; set; } = true;
        public bool NotifyAppointmentReminders { get; set; } = true;
        public bool NotifyJournalReminder { get; set; } = false;
        public bool NotifyHydration { get; set; } = false;
        public bool NotifySupplements { get; set; } = false;

        // 💗 Emotional & Motivational
        public bool NotifyAffirmations { get; set; } = false;
        public bool NotifyMilestoneCelebrations { get; set; } = true;

        // 👶 Baby Development
        public bool NotifyKickCount { get; set; } = true;
        public bool NotifyUltrasoundProgress { get; set; } = false;

        // 🌿 Lifestyle & Community
        public bool NotifyPartnerTips { get; set; } = false;
        public bool NotifyProductOffers { get; set; } = false;
        public bool NotifyCommunityEvents { get; set; } = false;
    }
}
