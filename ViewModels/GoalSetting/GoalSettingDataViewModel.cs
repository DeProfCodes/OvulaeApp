using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OvulaeShared.Enums.App;

namespace OvulaeApp.ViewModels.GoalSetting
{
    public class GoalSettingDataViewModel
    {
        public List<ModuleType> Goals { get; set; } = new();

        public List<QnA> QuestionAndAnswers { get; set; } = new();
    }

    public class QnA
    {
        public string Question { get; set; } = "";

        public string Answer { get; set; } = "";

        public DateTime DateAnswer { get; set; }
    }
}
