using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OvulaeApp.ViewModels.Chat;
using OvulaeShared.Enums.App;

namespace OvulaeApp.Services.LocalDataService.ChatBot
{
    public class ChatBotService : IChatBotService
    {
        private static List<ChatFAQItem> AllFAQData => CreateFAQsData();

        public ChatBotService()
        {
            
        }

        public List<ChatFAQItem> GetAllModulesFAQs()
        {
            try
            {
                return AllFAQData;
            }
            catch (Exception ex)
            {
                return new List<ChatFAQItem>();
            }
        }

        public List<ChatFAQItem> GetModuleFAQs(ModuleType moduleType)
        {
            try
            {
                var data = AllFAQData.Where(x => x.ModuleType == moduleType).ToList();

                return data;
            }
            catch (Exception ex)
            {
                return new List<ChatFAQItem>();
            }
        }

        private static List<ChatFAQItem> CreateFAQsData()
        {
            var result = new List<ChatFAQItem>();

            result.AddRange(CreatePeriodTrackerFAQs());
            result.AddRange(CreateOvulationFAQs());
            result.AddRange(CreatePregnancyFAQs());

            return result;
        }

        private static List<ChatFAQItem> CreatePeriodTrackerFAQs()
        {
            return new List<ChatFAQItem>
            {
                new ChatFAQItem
                {
                    ModuleType = ModuleType.PeriodTracker,
                    Question = "How do I log my period?",
                    Answer = "Go to the Period Tracker module and tap the '+' button to log your period start and end dates.",
                    Keywords = new List<string> { "log period", "add period", "record period", "track period", "period start", "period end", "enter period", "input period", "add menstruation", "add cycle" }
                },
                new ChatFAQItem
                {
                    ModuleType = ModuleType.PeriodTracker,
                    Question = "Can I edit a period entry?",
                    Answer = "Yes, tap on the period entry in your calendar and select edit to update the dates or details.",
                    Keywords = new List<string> { "edit period", "change period", "update period", "modify period", "correct period", "fix period" }
                },
                new ChatFAQItem
                {
                    ModuleType = ModuleType.PeriodTracker,
                    Question = "How do I delete a period entry?",
                    Answer = "Tap on the period entry you wish to remove, and select the delete option to remove it from your records.",
                    Keywords = new List<string> { "delete period", "remove period", "erase period", "clear period", "cancel period", "remove entry" }
                },
                new ChatFAQItem
                {
                    ModuleType = ModuleType.PeriodTracker,
                    Question = "Can Ovulae predict my next period?",
                    Answer = "Yes, based on your previous cycle data, Ovulae can predict your next period and notify you in advance.",
                    Keywords = new List<string> { "predict period", "next period", "period prediction", "when is my period", "forecast period", "period notify", "cycle predict" }
                },
                new ChatFAQItem
                {
                    ModuleType = ModuleType.PeriodTracker,
                    Question = "How do I view my cycle history?",
                    Answer = "You can view your full cycle history under the History or Calendar section in the Period Tracker module.",
                    Keywords = new List<string> { "cycle history", "view history", "period history", "cycle log", "past periods", "period records", "period calendar" }
                },
                new ChatFAQItem
                {
                    ModuleType = ModuleType.PeriodTracker,
                    Question = "Does Ovulae handle irregular cycles?",
                    Answer = "Yes, Ovulae adjusts its predictions based on your cycle variability to provide accurate tracking for irregular cycles.",
                    Keywords = new List<string> { "irregular cycles", "uneven cycles", "different cycle lengths", "cycle variability", "irregular period", "long cycle", "short cycle" }
                },
                new ChatFAQItem
                {
                    ModuleType = ModuleType.PeriodTracker,
                    Question = "Can I set reminders for my period?",
                    Answer = "Yes, you can enable period reminders under Settings > Notifications to receive alerts before your period starts.",
                    Keywords = new List<string> { "period reminder", "period notification", "notify period", "remind period", "alert period", "cycle reminder" }
                },
                new ChatFAQItem
                {
                    ModuleType = ModuleType.PeriodTracker,
                    Question = "How accurate is the period tracker?",
                    Answer = "Ovulae uses your past period data and averages to predict your next period, improving accuracy as you log more cycles.",
                    Keywords = new List<string> { "accuracy", "accurate", "how accurate", "period accuracy", "predict accurate", "prediction quality" }
                },
                new ChatFAQItem
                {
                    ModuleType = ModuleType.PeriodTracker,
                    Question = "Can I track symptoms with my period?",
                    Answer = "Yes, you can log symptoms like cramps, mood changes, and flow level during your period in the Period Tracker module.",
                    Keywords = new List<string> { "track symptoms", "period symptoms", "log cramps", "mood tracking", "flow tracking", "symptom tracker", "add symptoms", "pms tracking" }
                },
                new ChatFAQItem
                {
                    ModuleType = ModuleType.PeriodTracker,
                    Question = "Can I export my period data?",
                    Answer = "Currently, you can view and manually note your cycle data, with data export features coming in future updates.",
                    Keywords = new List<string> { "export data", "download data", "save data", "backup period data", "period export", "cycle export" }
                },
                new ChatFAQItem
                {
                    ModuleType = ModuleType.PeriodTracker,
                    Question = "How do I reset my period data?",
                    Answer = "If you wish to clear your period tracking data, go to Settings > Data Management > Reset Period Data. Note this will permanently delete your logged cycles.",
                    Keywords = new List<string> { "reset data", "clear data", "delete all periods", "reset period", "clear period history", "start over", "reset cycle data" }
                },
                new ChatFAQItem
                {
                    ModuleType = ModuleType.PeriodTracker,
                    Question = "Can I track spotting or light bleeding?",
                    Answer = "Yes, you can log spotting or light bleeding by specifying the flow level when adding your period entry.",
                    Keywords = new List<string> { "spotting", "light bleeding", "light period", "spot", "light flow", "track spotting", "log spotting" }
                },
                new ChatFAQItem
                {
                    ModuleType = ModuleType.PeriodTracker,
                    Question = "Can I track my period while on birth control?",
                    Answer = "Yes, you can still use Ovulae to track your periods while on birth control to monitor your cycle and symptoms.",
                    Keywords = new List<string> { "birth control", "pill", "contraceptive", "tracking period on pill", "track period birth control", "cycle and birth control" }
                },
                new ChatFAQItem
                {
                    ModuleType = ModuleType.PeriodTracker,
                    Question = "Why is my predicted period date changing?",
                    Answer = "Predicted dates may adjust as you log more cycles, especially if your periods are irregular, to improve prediction accuracy.",
                    Keywords = new List<string> { "period date changing", "why prediction changed", "predicted period wrong", "date shift", "prediction updated" }
                }
            };
        }

        private static List<ChatFAQItem> CreateOvulationFAQs()
        {
            return new List<ChatFAQItem>
            {
                new ChatFAQItem
                {
                    ModuleType = ModuleType.Ovulation,
                    Question = "How do I track my ovulation?",
                    Answer = "In the Ovulation module, you can log your basal body temperature, cervical mucus changes, and use ovulation tests to track your fertile window.",
                    Keywords = new List<string> { "track ovulation", "log ovulation", "ovulation tracking", "fertile window", "ovulation monitor", "record ovulation" }
                },
                new ChatFAQItem
                {
                    ModuleType = ModuleType.Ovulation,
                    Question = "Can Ovulae predict my ovulation?",
                    Answer = "Yes, Ovulae predicts your ovulation based on your cycle data and logged symptoms, notifying you when your fertile window is approaching.",
                    Keywords = new List<string> { "predict ovulation", "when do I ovulate", "ovulation prediction", "fertile days", "ovulation calculator", "ovulation forecast" }
                },
                new ChatFAQItem
                {
                    ModuleType = ModuleType.Ovulation,
                    Question = "How can I tell when I am ovulating?",
                    Answer = "Signs of ovulation include a rise in basal body temperature, changes in cervical mucus to a clear stretchy consistency, and sometimes mild pelvic pain (mittelschmerz).",
                    Keywords = new List<string> { "am I ovulating", "signs of ovulation", "ovulation symptoms", "ovulation pain", "mittelschmerz", "cervical mucus", "BBT" }
                },
                new ChatFAQItem
                {
                    ModuleType = ModuleType.Ovulation,
                    Question = "What is basal body temperature (BBT) tracking?",
                    Answer = "BBT tracking involves measuring your temperature each morning before getting out of bed to identify the slight increase after ovulation has occurred.",
                    Keywords = new List<string> { "BBT", "basal temperature", "body temperature", "temperature tracking", "temp chart", "chart ovulation", "bbt ovulation" }
                },
                new ChatFAQItem
                {
                    ModuleType = ModuleType.Ovulation,
                    Question = "How do I log cervical mucus changes?",
                    Answer = "You can log the texture and appearance of your cervical mucus in the Ovulation module, which helps predict fertile days.",
                    Keywords = new List<string> { "cervical mucus", "cm tracking", "mucus changes", "log mucus", "fertile mucus", "egg white mucus", "track cm" }
                },
                new ChatFAQItem
                {
                    ModuleType = ModuleType.Ovulation,
                    Question = "Can I use ovulation test strips with Ovulae?",
                    Answer = "Yes, you can log your ovulation test results in Ovulae to improve tracking accuracy for your fertile window.",
                    Keywords = new List<string> { "ovulation test", "opk", "test strips", "positive ovulation test", "lh test", "ovulation kit" }
                },
                new ChatFAQItem
                {
                    ModuleType = ModuleType.Ovulation,
                    Question = "Does Ovulae work for irregular cycles?",
                    Answer = "Yes, Ovulae adjusts its predictions using your logged data to help track ovulation, even with irregular cycles.",
                    Keywords = new List<string> { "irregular cycles", "irregular periods", "irregular ovulation", "tracking irregular", "uneven cycles", "different cycle lengths" }
                },
                new ChatFAQItem
                {
                    ModuleType = ModuleType.Ovulation,
                    Question = "Can I get pregnant during my period?",
                    Answer = "While unlikely, it is possible to get pregnant if you have a short cycle and ovulate soon after your period. Tracking helps clarify your fertile window.",
                    Keywords = new List<string> { "pregnant on period", "get pregnant period", "fertile during period", "period pregnancy", "pregnancy period" }
                },
                new ChatFAQItem
                {
                    ModuleType = ModuleType.Ovulation,
                    Question = "How long does ovulation last?",
                    Answer = "Ovulation itself lasts about 12-24 hours, but your fertile window (when pregnancy can occur) lasts around 5-6 days due to sperm survival.",
                    Keywords = new List<string> { "how long ovulation", "length of ovulation", "duration ovulation", "fertile window length", "when fertile" }
                },
                new ChatFAQItem
                {
                    ModuleType = ModuleType.Ovulation,
                    Question = "What is the fertile window?",
                    Answer = "The fertile window includes the 5 days before ovulation and the day of ovulation, which is the best time to try to conceive.",
                    Keywords = new List<string> { "fertile window", "fertile days", "when fertile", "best time to conceive", "fertility window" }
                },
                new ChatFAQItem
                {
                    ModuleType = ModuleType.Ovulation,
                    Question = "What if I don’t ovulate every cycle?",
                    Answer = "Some cycles can be anovulatory (no ovulation). Ovulae helps you track your patterns, but if you suspect consistent anovulation, consult your healthcare provider.",
                    Keywords = new List<string> { "anovulation", "no ovulation", "not ovulating", "skipped ovulation", "missing ovulation", "irregular ovulation" }
                },
                new ChatFAQItem
                {
                    ModuleType = ModuleType.Ovulation,
                    Question = "How accurate is Ovulae’s ovulation prediction?",
                    Answer = "Accuracy improves the more data you log. It combines cycle length, symptoms, and optional BBT or OPK logs to refine predictions.",
                    Keywords = new List<string> { "accuracy", "accurate ovulation", "prediction accuracy", "is prediction accurate", "predict ovulation accuracy" }
                },
                new ChatFAQItem
                {
                    ModuleType = ModuleType.Ovulation,
                    Question = "Can Ovulae help me conceive?",
                    Answer = "Yes, by helping you identify your fertile days, Ovulae can increase your chances of conceiving naturally.",
                    Keywords = new List<string> { "conceive", "get pregnant", "trying to conceive", "ttc", "help pregnancy", "fertility help", "pregnancy help" }
                },
                new ChatFAQItem
                {
                    ModuleType = ModuleType.Ovulation,
                    Question = "Can I track ovulation if I have PCOS?",
                    Answer = "Yes, though cycles may be irregular, tracking can help identify patterns over time. Consult your doctor for personalized PCOS management.",
                    Keywords = new List<string> { "PCOS", "polycystic ovary", "pcos tracking", "pcos ovulation", "track pcos", "irregular periods pcos" }
                },
                new ChatFAQItem
                {
                    ModuleType = ModuleType.Ovulation,
                    Question = "Can stress affect my ovulation?",
                    Answer = "Yes, stress can delay or impact ovulation. Logging your cycle and symptoms can help you monitor changes.",
                    Keywords = new List<string> { "stress ovulation", "does stress affect", "stress delay period", "stress delay ovulation", "impact of stress" }
                }
            };
        }

        private static List<ChatFAQItem> CreatePregnancyFAQs()
        {
            return new List<ChatFAQItem>
            {
                new ChatFAQItem
                {
                    ModuleType = ModuleType.Pregnancy,
                    Question = "How do I log my pregnancy in Ovulae?",
                    Answer = "Go to the Pregnancy Tracker module and enter your last period date or due date to start tracking your pregnancy week by week.",
                    Keywords = new List<string> { "log pregnancy", "add pregnancy", "track pregnancy", "record pregnancy", "start pregnancy tracking" }
                },
                new ChatFAQItem
                {
                    ModuleType = ModuleType.Pregnancy,
                    Question = "How does Ovulae calculate my due date?",
                    Answer = "Ovulae calculates your estimated due date based on your last menstrual period or the due date provided by your healthcare provider.",
                    Keywords = new List<string> { "due date", "calculate due date", "EDD", "expected delivery", "when is my baby due", "baby due date" }
                },
                new ChatFAQItem
                {
                    ModuleType = ModuleType.Pregnancy,
                    Question = "Can I track my symptoms during pregnancy?",
                    Answer = "Yes, you can log pregnancy symptoms such as nausea, fatigue, cravings, and emotional changes in the Pregnancy Tracker module.",
                    Keywords = new List<string> { "pregnancy symptoms", "track symptoms", "log symptoms", "nausea", "fatigue", "cravings", "symptom tracking" }
                },
                new ChatFAQItem
                {
                    ModuleType = ModuleType.Pregnancy,
                    Question = "How accurate is Ovulae’s pregnancy tracker?",
                    Answer = "Ovulae’s tracker provides general guidance based on standard pregnancy timelines, but always follow your healthcare provider’s specific advice.",
                    Keywords = new List<string> { "accuracy", "accurate tracking", "tracker accuracy", "is it accurate", "pregnancy tracking accuracy" }
                },
                new ChatFAQItem
                {
                    ModuleType = ModuleType.Pregnancy,
                    Question = "Can I see my baby’s development week by week?",
                    Answer = "Yes, Ovulae shows weekly updates on your baby’s growth, changes, and development milestones during your pregnancy.",
                    Keywords = new List<string> { "baby development", "baby growth", "week by week", "baby updates", "pregnancy weeks", "baby milestones" }
                },
                new ChatFAQItem
                {
                    ModuleType = ModuleType.Pregnancy,
                    Question = "How do I track weight gain during pregnancy?",
                    Answer = "You can log your weight in the Pregnancy Tracker to monitor healthy weight gain according to your pregnancy week.",
                    Keywords = new List<string> { "weight gain", "track weight", "pregnancy weight", "weight monitoring", "log weight" }
                },
                new ChatFAQItem
                {
                    ModuleType = ModuleType.Pregnancy,
                    Question = "Can I track fetal movements and kicks?",
                    Answer = "Yes, you can log your baby’s movements to track patterns and activity throughout your pregnancy.",
                    Keywords = new List<string> { "fetal movement", "baby kicks", "track kicks", "kick count", "movement log", "baby activity" }
                },
                new ChatFAQItem
                {
                    ModuleType = ModuleType.Pregnancy,
                    Question = "Is it normal to have irregular contractions?",
                    Answer = "Irregular contractions, known as Braxton Hicks, can be normal, but if you notice regular or painful contractions, contact your healthcare provider immediately.",
                    Keywords = new List<string> { "irregular contractions", "Braxton Hicks", "contractions", "pregnancy cramps", "labor signs", "false labor" }
                },
                new ChatFAQItem
                {
                    ModuleType = ModuleType.Pregnancy,
                    Question = "Can I use Ovulae if I have a high-risk pregnancy?",
                    Answer = "Ovulae can help you track general pregnancy information, but always follow your doctor’s advice if you have a high-risk pregnancy.",
                    Keywords = new List<string> { "high-risk pregnancy", "risk pregnancy", "complications", "medical advice", "doctor pregnancy" }
                },
                new ChatFAQItem
                {
                    ModuleType = ModuleType.Pregnancy,
                    Question = "What nutrition tips does Ovulae provide?",
                    Answer = "Ovulae offers general healthy eating tips during pregnancy, but consult your doctor or dietitian for personalized dietary guidance.",
                    Keywords = new List<string> { "nutrition", "pregnancy diet", "eating during pregnancy", "diet tips", "healthy eating", "food pregnancy" }
                },
                new ChatFAQItem
                {
                    ModuleType = ModuleType.Pregnancy,
                    Question = "Can Ovulae send me pregnancy reminders?",
                    Answer = "Yes, you can enable notifications under Settings to receive weekly updates and reminders related to your pregnancy.",
                    Keywords = new List<string> { "reminders", "notifications", "alerts", "pregnancy reminders", "weekly updates", "pregnancy alerts" }
                },
                new ChatFAQItem
                {
                    ModuleType = ModuleType.Pregnancy,
                    Question = "How can I prepare for birth using Ovulae?",
                    Answer = "Ovulae provides week-by-week guidance, including tips on preparing your hospital bag, understanding labor signs, and what to expect during delivery.",
                    Keywords = new List<string> { "prepare for birth", "delivery preparation", "hospital bag", "labor signs", "birth plan", "ready for birth" }
                },
                new ChatFAQItem
                {
                    ModuleType = ModuleType.Pregnancy,
                    Question = "Can I use Ovulae if I am pregnant after a miscarriage?",
                    Answer = "Yes, Ovulae can be used to track your current pregnancy, but please ensure you follow your doctor’s advice for additional care and monitoring.",
                    Keywords = new List<string> { "pregnancy after miscarriage", "rainbow baby", "pregnancy after loss", "track pregnancy after miscarriage" }
                },
                new ChatFAQItem
                {
                    ModuleType = ModuleType.Pregnancy,
                    Question = "What if I experience unusual symptoms?",
                    Answer = "If you notice unusual symptoms like heavy bleeding, severe pain, or reduced baby movement, contact your healthcare provider immediately.",
                    Keywords = new List<string> { "unusual symptoms", "emergency", "bleeding", "severe pain", "baby movement", "when to call doctor" }
                }
            };
        }

    }
}
