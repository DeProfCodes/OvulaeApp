using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OvulaeApp.Helpers.Enums;
using OvulaeApp.Helpers.Functions;
using OvulaeApp.ViewModels.Module;

namespace OvulaeApp.Services.LocalDataService.OvulationServices
{
    public class OvulationService : IOvulationService
    {
        public static List<ModulePhaseDataViewModel> OvulationDataStore => LoadAllOvulationData();

        public OvulationService() 
        {
            
        }

        public List<ModulePhaseDataViewModel> GetAllPhasesData()
        {
            try
            {
                return OvulationDataStore;
            }
            catch
            {
                return new();
            }
        }

        public ModulePhaseDataViewModel GetCurrentPhaseData(int currentCycleDay)
        {
            try
            {
                var phase = SharedCommonFunctions.GetOvulationPhase(currentCycleDay);

                return OvulationDataStore.FirstOrDefault(x => x.ModulePhase == phase);
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
                var phase = SharedCommonFunctions.GetOvulationPhase(currentCycleDay);

                return OvulationDataStore.FirstOrDefault(x => x.ModulePhase == phase)?.PhaseDetails ?? new();
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
                return OvulationDataStore.FirstOrDefault(x => x.ModulePhase == phase);
            }
            catch
            {
                return new();
            }
        }

        public List<ModulePhaseDataViewModel> GetAllOvulationData()
        {
            try
            {
                return OvulationDataStore;
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
                return OvulationDataStore.FirstOrDefault(x => x.ModulePhase == phase)?.PhaseDetails ?? new();
            }
            catch
            {
                return new();
            }
        }

        public static List<ModulePhaseDataViewModel> LoadAllOvulationData()
        {
            var result = new List<ModulePhaseDataViewModel>
            {
                new ModulePhaseDataViewModel
                {
                    ModulePhase = OvulationPhase.Menstrual,
                    PhaseSummary = "Menstrual Phase: A time of release and renewal — rest, hydrate, and be gentle with yourself.",
                    PhaseHighlights = new List<ModuleDashboardCard>
                    {
                        new ModuleDashboardCard { Title = "Rest & Recover 🛌", Subtitle = "Prioritize rest and hydration during your period.", Tip = "💡 A warm bath or heating pad helps relieve cramps.", ImageSource = "phase_menst_sleep.png" },
                        new ModuleDashboardCard { Title = "Iron Boost 💪", Subtitle = "Eat iron-rich foods like spinach and lentils.", Tip = "💡 Pair with vitamin C to improve absorption.", ImageSource = "phase_menst_food.png" },
                        new ModuleDashboardCard { Title = "Track Flow 📊", Subtitle = "Logging flow helps predict your full cycle accurately.", ImageSource = "phase_menst_track.png" },
                        new ModuleDashboardCard { Title = "Stay Hydrated 💧", Subtitle = "Water helps reduce bloating and supports circulation.", Tip = "💡 Aim for 6–8 glasses of water daily.", ImageSource = "phase_menst_water.png" },
                        new ModuleDashboardCard { Title = "Gentle Movement 🧎‍♀️", Subtitle = "Light stretching or walking eases cramps.", Tip = "💡 Try short yoga flows tailored for periods.", ImageSource = "phase_menst_stretch.png" },
                    },
                    PhaseDetails = new ModulePhaseDetailsViewModel
                    {
                        PhaseTitle = "Menstrual Phase 🩸",
                        Description = "This marks the start of your cycle, when the uterine lining is shed. Energy may be low, and rest is essential. Hormones like estrogen and progesterone are at their lowest levels.",
                        HeroImage = "ovulation_phase_menst_hero.png",
                        KeyChanges = new List<string>
                        {
                            "Estrogen and progesterone drop sharply",
                            "Uterine lining is shed — menstruation begins",
                            "Energy may be lower, rest is important",
                            "Iron levels may dip, affecting strength or mood"
                        },
                        Tips = new List<string>
                        {
                            "Get extra rest — naps, gentle routines, and no pressure to be productive",
                            "Increase iron intake with spinach, lentils, or lean red meat",
                            "Apply heat to ease cramps — hot water bottle or warm bath",
                            "Track your period’s start date to help predict ovulation"
                        },
                        Insights = new List<string>
                        {
                            "You’re not lazy — your body is doing deep internal work. Honor the need for recovery.",
                            "Bleeding duration and volume vary — track for 3–6 months to spot patterns or issues."
                        }
                    }
                },
                new ModulePhaseDataViewModel
                {
                    ModulePhase = OvulationPhase.Follicular,
                    PhaseSummary = "Follicular Phase: Energy returns, creativity sparks — ideal for planning, workouts, and fresh starts.",
                    PhaseHighlights = new List<ModuleDashboardCard>
                    {
                        new ModuleDashboardCard { Title = "Energy Rising 🚀", Subtitle = "Start light workouts — your energy will increase.", ImageSource = "phase_foca_gym.png" },
                        new ModuleDashboardCard { Title = "Eat Smart 🥦", Subtitle = "Focus on leafy greens, legumes, and healthy fats.", ImageSource = "phase_foca_food.png" },
                        new ModuleDashboardCard { Title = "Plan Creatively ✍️", Subtitle = "This is a great time for decision-making and planning.", ImageSource = "phase_foca_log.png" },
                        new ModuleDashboardCard { Title = "Glow-Up Time ✨", Subtitle = "Estrogen rise often improves mood and skin.", Tip = "💡 Focus on skin care and hydration routines.", ImageSource = "phase_foca_glow.png" },
                        new ModuleDashboardCard { Title = "Boost Focus 🧠", Subtitle = "Cognitive sharpness improves this week.", Tip = "💡 Plan your most creative or strategic tasks now.", ImageSource = "phase_foca_focus.png" },
                    },
                    PhaseDetails = new ModulePhaseDetailsViewModel
                    {
                        PhaseTitle = "Follicular Phase 🌱",
                        Description = "This is your body’s prep phase for ovulation. Estrogen rises, follicles develop, and you’ll likely feel an energy and mood boost. A great time for creativity and new projects.",
                        HeroImage = "ovulation_phase_follicular_hero.png",
                        KeyChanges = new List<string>
                        {
                            "Estrogen rises — improving mood and energy",
                            "Follicle-stimulating hormone (FSH) helps eggs mature",
                            "Cervical mucus may become creamy or sticky",
                            "Motivation, focus, and libido may begin rising"
                        },
                        Tips = new List<string>
                        {
                            "Start strength training or moderate cardio — energy is on the rise",
                            "Fuel with healthy fats and leafy greens to support estrogen production",
                            "Use this phase for brainstorming, planning, or goal-setting",
                            "Track cervical mucus to estimate your fertile window ahead"
                        },
                        Insights = new List<string>
                        {
                            "You might find yourself more sociable — this is hormonally supported!",
                            "Skin often glows during this phase due to higher estrogen"
                        }
                    }
                },
                new ModulePhaseDataViewModel
                {
                    ModulePhase = OvulationPhase.Ovulation,
                    PhaseSummary = "Ovulation Phase: Fertility peaks. You may feel more confident and energetic — a prime time for intimacy and connection.",
                    PhaseHighlights = new List<ModuleDashboardCard>
                    {
                        new ModuleDashboardCard { Title = "Fertile Window 🔍", Subtitle = "Now is your most fertile time this cycle.", ImageSource = "phase_ovul_window.png" },
                        new ModuleDashboardCard { Title = "Intercourse Timing ❤️", Subtitle = "Best time for conception if trying to conceive.", Tip = "💡 Try every 1–2 days during this window.", ImageSource = "phase_ovul_window.png"  },
                        new ModuleDashboardCard { Title = "Cervical Mucus Check 💧", Subtitle = "Look for clear, stretchy discharge — a sign of ovulation.", ImageSource = "phase_ovul_drop.png" },
                        new ModuleDashboardCard { Title = "Ovulation Pain Awareness ⚠️", Subtitle = "Some may feel slight pelvic pain — known as Mittelschmerz.", Tip = "💡 Track it to better understand your cycle.", ImageSource = "phase_ovul_pain.png" },
                        new ModuleDashboardCard { Title = "High Libido 🔥", Subtitle = "Many experience increased sexual desire.", Tip = "💡 This is a natural sign of fertility.", ImageSource = "phase_ovul_libido.png" },
                    },
                    PhaseDetails = new ModulePhaseDetailsViewModel
                    {
                        PhaseTitle = "Ovulation Phase 💡",
                        Description = "This is your fertile peak. A mature egg is released and survives ~12–24 hours. Estrogen and LH levels are high, and many people experience increased libido, clearer skin, and high confidence.",
                        HeroImage = "ovulation_phase_ovulation_hero.png",
                        KeyChanges = new List<string>
                        {
                            "Luteinizing hormone (LH) surges — triggering ovulation",
                            "Cervical mucus becomes clear, stretchy (egg-white)",
                            "Libido often peaks",
                            "Basal body temperature may rise after ovulation"
                        },
                        Tips = new List<string>
                        {
                            "If trying to conceive — this is the best window for intercourse",
                            "Track ovulation pain or signs to better understand timing",
                            "Stay hydrated and support your body with nutrient-rich meals",
                            "Plan social events or presentations — confidence is often higher"
                        },
                        Insights = new List<string>
                        {
                            "Some may feel a subtle twinge on one side of the abdomen — called ‘mittelschmerz’",
                            "Your face may look more symmetrical or glowy — nature’s subtle fertility signal"
                        }
                    }
                },
                new ModulePhaseDataViewModel
                {
                    ModulePhase = OvulationPhase.Luteal,
                    PhaseSummary = "Luteal Phase: Hormonal shifts may bring cravings or mood swings. Wind down and prepare for your next cycle.",
                    PhaseHighlights = new List<ModuleDashboardCard>
                    {
                        new ModuleDashboardCard { Title = "Mood Care 🧘", Subtitle = "Hormones may fluctuate — try journaling or meditation.", ImageSource = "phase_luteal_meditate.png" },
                        new ModuleDashboardCard { Title = "Cravings Ahead 🍫", Subtitle = "It’s okay to indulge — just balance with protein & fiber.", ImageSource = "phase_luteal_choc.png" },
                        new ModuleDashboardCard { Title = "Prep for Period 🩸", Subtitle = "Keep pads or tampons ready. PMS may begin soon.", ImageSource = "phase_luteal_period.png" },
                        new ModuleDashboardCard { Title = "Support Sleep 🌙", Subtitle = "Hormonal shifts may affect your rest.", Tip = "Avoid caffeine after 2pm and set a bedtime routine.", ImageSource = "phase_luteal_sleep.png" },
                        new ModuleDashboardCard { Title = "Gut Check 🥗", Subtitle = "Bloating and digestion may be affected.", Tip = "Add probiotics or fiber-rich foods like oats and berries.", ImageSource = "phase_luteal_gut.png" },
                    },
                    PhaseDetails = new ModulePhaseDetailsViewModel
                    {
                        PhaseTitle = "Luteal Phase 🌙",
                        Description = "After ovulation, your body shifts toward possible pregnancy. Progesterone increases to support uterine lining. You may notice PMS symptoms, mood swings, or cravings.",
                        HeroImage = "ovulation_phase_luteal_hero.png",
                        KeyChanges = new List<string>
                        {
                            "Progesterone rises — calming, but may also trigger fatigue",
                            "Body temp stays slightly elevated",
                            "Premenstrual symptoms may appear — bloating, mood swings",
                            "If no pregnancy, hormones drop toward menstruation"
                        },
                        Tips = new List<string>
                        {
                            "Prioritize rest, grounding foods, and mood-supporting practices like journaling",
                            "Reduce sugar and caffeine to avoid worsened PMS symptoms",
                            "Use magnesium supplements or bath soaks to ease cramping or tension",
                            "Try meditation or deep breathing to regulate emotional shifts"
                        },
                        Insights = new List<string>
                        {
                            "You’re not overly emotional — it’s a real hormonal response. Be kind to yourself.",
                            "PMS symptoms are common but not mandatory — nutrition and sleep help reduce them."
                        }
                    }
                }
            };

            result.ForEach(x => 
            {
                x.PhaseDetails.KeyChanges = x.PhaseDetails.KeyChanges.Select(t => $"• {t}").ToList();

                x.PhaseDetails.Tips = x.PhaseDetails.Tips.Select(t => $"• {t}").ToList();

                x.PhaseDetails.Insights = x.PhaseDetails.Insights.Select(i => $"• {i}").ToList();
            });

            return result;
        }
    }
}
