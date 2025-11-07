using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OvulaeApp.Helpers.Enums;
using OvulaeApp.ViewModels.Module;

namespace OvulaeApp.Services.LocalDataService.PeriodTrackerServices
{
    public class PeriodTrackerService : IPeriodTrackerService
    {
        public static List<ModulePhaseDataViewModel> PeriodDataStore => LoadAllPeriodData();

        public PeriodTrackerService() 
        {
            
        }

        public OvulationPhase GetOvulationPhase(int currentCycleDay)
        {
            if (currentCycleDay >= 1 && currentCycleDay <= 5) return OvulationPhase.Menstrual;
            else if (currentCycleDay >= 6 && currentCycleDay <= 13) return OvulationPhase.Follicular;
            else if (currentCycleDay >= 14 && currentCycleDay <= 16) return OvulationPhase.Ovulation;
            else return OvulationPhase.Luteal;
        }

        public ModulePhaseDataViewModel GetCurrentPhaseData(int currentCycleDay)
        {
            try
            {
                var phase = GetOvulationPhase(currentCycleDay);

                return PeriodDataStore.FirstOrDefault(x => x.ModulePhase == phase);
            }
            catch
            {
                return new();   
            }
        }

        public ModulePhaseDataViewModel GetPhaseData(OvulationPhase phase)
        {
            try
            {
                return PeriodDataStore.FirstOrDefault(x => x.ModulePhase == phase);
            }
            catch
            {
                return new();
            }
        }

        public List<ModulePhaseDataViewModel> GetAllPhaseData()
        {
            try
            {
                return PeriodDataStore;
            }
            catch
            {
                return new();
            }
        }

        public ModulePhaseDetailsViewModel GetCurrentPhaseDetails(int currentCycleDay)
        {
            try
            {
                var phase = GetOvulationPhase(currentCycleDay);

                return PeriodDataStore.FirstOrDefault(x => x.ModulePhase == phase)?.PhaseDetails ?? new();
            }
            catch
            {
                return new();
            }
        }

        public ModulePhaseDetailsViewModel GetCurrentPhaseDetailsByPhase(OvulationPhase phase)
        {
            try
            {
                return PeriodDataStore.FirstOrDefault(x => x.ModulePhase == phase)?.PhaseDetails ?? new();
            }
            catch
            {
                return new();
            }
        }

        public static List<ModulePhaseDataViewModel> LoadAllPeriodData()
        {
            var iconMap = PeriodPhaseIconMapping;

            var result = new List<ModulePhaseDataViewModel>
            {
                new ModulePhaseDataViewModel
                {
                    ModulePhase = OvulationPhase.Menstrual,
                    PhaseSummary = "Menstrual Phase: Time for rest, gentle care, and honoring your body’s renewal process.",
                    PhaseHighlights = new List<ModuleDashboardCard>
                    {
                        new ModuleDashboardCard { Title = "Rest & Hydrate 💧", Subtitle = "Your body is shedding the uterine lining.", Tip = "💡 Use a heating pad to ease cramps.", ImageSource = iconMap[OvulationPhase.Menstrual][0] },
                        new ModuleDashboardCard { Title = "Iron-Rich Foods 🥩", Subtitle = "Support iron loss with spinach, lentils, lean meats.", Tip = "💡 Pair with vitamin C for absorption.", ImageSource = iconMap[OvulationPhase.Menstrual][1] },
                        new ModuleDashboardCard { Title = "Track Your Flow 📊", Subtitle = "Logging helps predict and understand your cycle.", ImageSource = iconMap[OvulationPhase.Menstrual][2] },
                        new ModuleDashboardCard { Title = "Gentle Movement 🧘‍♀️", Subtitle = "Stretching can relieve tension and cramps.", Tip = "💡 Try light yoga or a short walk.", ImageSource = iconMap[OvulationPhase.Menstrual][3] },
                        new ModuleDashboardCard { Title = "Mood Awareness 🌧️", Subtitle = "Hormonal dips may affect mood, be gentle with yourself.", ImageSource = iconMap[OvulationPhase.Menstrual][4] }
                    },
                    PhaseDetails = new ModulePhaseDetailsViewModel
                    {
                        PhaseTitle = "Menstrual Phase 🩸",
                        Description = "The shedding of the uterine lining marks your cycle’s start. Energy may be low; rest and nourishment are essential as hormones reset.",
                        HeroImage = "menstrual_hero.png",
                        KeyChanges = new List<string>
                        {
                            "• Estrogen and progesterone are at their lowest",
                            "• Shedding of the uterine lining occurs",
                            "• Energy levels may be lower",
                            "• Iron levels may decrease"
                        },
                        Tips = new List<string>
                        {
                            "• Use heating pads or warm baths for cramp relief",
                            "• Prioritize hydration and iron-rich foods",
                            "• Allow rest and avoid overexertion",
                            "• Track bleeding patterns for future reference"
                        },
                        Insights = new List<string>
                        {
                            "• Your need for rest is valid — honor it.",
                            "• Tracking your cycle helps you understand your health."
                        }
                    }
                },
                new ModulePhaseDataViewModel
                {
                    ModulePhase = OvulationPhase.Follicular,
                    PhaseSummary = "Follicular Phase: A time of rising energy, creativity, and planning as your body prepares for ovulation.",
                    PhaseHighlights = new List<ModuleDashboardCard>
                    {
                        new ModuleDashboardCard { Title = "Energy Rising 🚀", Subtitle = "Great time to restart workouts or projects.", ImageSource = iconMap[OvulationPhase.Follicular][0] },
                        new ModuleDashboardCard { Title = "Eat Fresh 🥗", Subtitle = "Focus on veggies, lean protein, healthy fats.", ImageSource = iconMap[OvulationPhase.Follicular][1] },
                        new ModuleDashboardCard { Title = "Plan & Create ✍️", Subtitle = "Brainstorming and planning come easily.", ImageSource = iconMap[OvulationPhase.Follicular][2] },
                        new ModuleDashboardCard { Title = "Skin Glow ✨", Subtitle = "Estrogen rises, often improving skin and mood.", ImageSource = iconMap[OvulationPhase.Follicular][3] },
                        new ModuleDashboardCard { Title = "Stay Active 🏃‍♀️", Subtitle = "Energy supports cardio and strength training.", ImageSource = iconMap[OvulationPhase.Follicular][4] }
                    },
                    PhaseDetails = new ModulePhaseDetailsViewModel
                    {
                        PhaseTitle = "Follicular Phase 🌱",
                        Description = "After menstruation, your body prepares for ovulation. Estrogen rises, leading to higher energy, focus, and motivation for projects.",
                        HeroImage = "follicular_hero.png",
                        KeyChanges = new List<string>
                        {
                            "• Estrogen levels increase",
                            "• FSH stimulates follicle development",
                            "• Energy and mood typically improve",
                            "• Cervical mucus may begin increasing"
                        },
                        Tips = new List<string>
                        {
                            "• Engage in planning and creative activities",
                            "• Increase physical activity as energy rises",
                            "• Eat nutrient-dense foods to support hormone balance",
                            "• Track cervical mucus if monitoring fertility"
                        },
                        Insights = new List<string>
                        {
                            "• This is a great phase for new beginnings.",
                            "• Your body supports productivity naturally during this phase."
                        }
                    }
                },
                new ModulePhaseDataViewModel
                {
                    ModulePhase = OvulationPhase.Ovulation,
                    PhaseSummary = "Ovulation Phase: Your peak fertility window with high energy and confidence.",
                    PhaseHighlights = new List<ModuleDashboardCard>
                    {
                        new ModuleDashboardCard { Title = "Peak Fertility 🔍", Subtitle = "Best time for conception if trying to conceive.", ImageSource = iconMap[OvulationPhase.Ovulation][0] },
                        new ModuleDashboardCard { Title = "Confident Energy 💃", Subtitle = "High confidence and sociability are common.", ImageSource = iconMap[OvulationPhase.Ovulation][1] },
                        new ModuleDashboardCard { Title = "Libido Boost 🔥", Subtitle = "Sex drive often increases.", ImageSource = iconMap[OvulationPhase.Ovulation][2] },
                        new ModuleDashboardCard { Title = "Check Mucus 💧", Subtitle = "Look for clear, stretchy cervical mucus.", ImageSource = iconMap[OvulationPhase.Ovulation][3] },
                        new ModuleDashboardCard { Title = "Hydrate & Move 💦", Subtitle = "Stay active and hydrated for overall wellness.", ImageSource = iconMap[OvulationPhase.Ovulation][4] }
                    },
                    PhaseDetails = new ModulePhaseDetailsViewModel
                    {
                        PhaseTitle = "Ovulation Phase 💡",
                        Description = "Around mid-cycle, ovulation occurs with an egg release, high estrogen, and LH surge, bringing high energy, confidence, and peak fertility.",
                        HeroImage = "ovulation_hero.png",
                        KeyChanges = new List<string>
                        {
                            "• LH surge triggers egg release",
                            "• Estrogen peaks",
                            "• Cervical mucus becomes clear and stretchy",
                            "• Libido often increases"
                        },
                        Tips = new List<string>
                        {
                            "• Engage in social or high-energy activities",
                            "• Monitor cervical mucus for fertility tracking",
                            "• Hydrate well and nourish with whole foods",
                            "• Consider family planning during this window"
                        },
                        Insights = new List<string>
                        {
                            "• This is your most fertile window if trying to conceive.",
                            "• Many feel most confident and sociable during ovulation."
                        }
                    }
                },
                new ModulePhaseDataViewModel
                {
                    ModulePhase = OvulationPhase.Luteal,
                    PhaseSummary = "Luteal Phase: Time to slow down, focus on rest, and prepare for your next cycle as progesterone rises.",
                    PhaseHighlights = new List<ModuleDashboardCard>
                    {
                        new ModuleDashboardCard { Title = "Mood Support 🧘", Subtitle = "Hormonal shifts may affect mood.", ImageSource = iconMap[OvulationPhase.Luteal][0] },
                        new ModuleDashboardCard { Title = "Rest & Nourish 🍵", Subtitle = "Focus on warm, comforting foods.", ImageSource = iconMap[OvulationPhase.Luteal][1] },
                        new ModuleDashboardCard { Title = "Craving Awareness 🍫", Subtitle = "Balance cravings with protein and fiber.", ImageSource = iconMap[OvulationPhase.Luteal][2] },
                        new ModuleDashboardCard { Title = "Support Sleep 🌙", Subtitle = "Establish a calming bedtime routine.", ImageSource = iconMap[OvulationPhase.Luteal][3] },
                        new ModuleDashboardCard { Title = "Track PMS 📝", Subtitle = "Log symptoms to prepare for menstruation.", ImageSource = iconMap[OvulationPhase.Luteal][4] }
                    },
                    PhaseDetails = new ModulePhaseDetailsViewModel
                    {
                        PhaseTitle = "Luteal Phase 🌙",
                        Description = "After ovulation, progesterone rises to prepare for possible pregnancy. You may notice mood changes or cravings as your body prepares for menstruation.",
                        HeroImage = "luteal_hero.png",
                        KeyChanges = new List<string>
                        {
                            "• Progesterone increases",
                            "• Body temperature stays slightly elevated",
                            "• Premenstrual symptoms may appear",
                            "• Hormones drop if pregnancy does not occur"
                        },
                        Tips = new List<string>
                        {
                            "• Eat warm, grounding foods and stay hydrated",
                            "• Rest and avoid high stress where possible",
                            "• Track PMS symptoms to manage future cycles",
                            "• Consider magnesium or herbal teas to ease symptoms"
                        },
                        Insights = new List<string>
                        {
                            "• Mood changes are normal during this phase.",
                            "• Preparation and awareness can ease PMS symptoms."
                        }
                    }
                }
            };

            return result;
        }

        public static Dictionary<OvulationPhase, List<string>> PeriodPhaseIconMapping = new()
        {
            [OvulationPhase.Menstrual] = new List<string>
            {
                "phase_menst_sleep.png",      // Rest & Hydrate 💧
                "phase_menst_food.png",       // Iron-Rich Foods 🥩
                "phase_menst_track.png",      // Track Your Flow 📊
                "phase_menst_stretch.png",    // Gentle Movement 🧘‍♀️
                "phase_menst_water.png"       // Mood Awareness 🌧️ (placeholder)
            },
            [OvulationPhase.Follicular] = new List<string>
            {
                "phase_foca_gym.png",         // Energy Rising 🚀
                "phase_foca_food.png",        // Eat Fresh 🥗
                "phase_foca_log.png",         // Plan & Create ✍️
                "phase_foca_glow.png",        // Skin Glow ✨
                "phase_foca_focus.png"        // Stay Active 🏃‍♀️
            },
            [OvulationPhase.Ovulation] = new List<string>
            {
                "phase_ovul_window.png",      // Peak Fertility 🔍
                "phase_ovul_libido.png",      // Confident Energy 💃
                "phase_ovul_libido.png",      // Libido Boost 🔥
                "phase_ovul_drop.png",        // Check Mucus 💧
                "phase_menst_water.png"       // Hydrate & Move 💦 (reused hydration icon)
            },
            [OvulationPhase.Luteal] = new List<string>
            {
                "phase_luteal_meditate.png",  // Mood Support 🧘
                "phase_luteal_period.png",    // Rest & Nourish 🍵
                "phase_luteal_choc.png",      // Craving Awareness 🍫
                "phase_luteal_sleep.png",     // Support Sleep 🌙
                "phase_luteal_gut.png"        // Track PMS 📝
            }
        };
    }
}
