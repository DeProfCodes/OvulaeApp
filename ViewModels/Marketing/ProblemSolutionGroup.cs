using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OvulaeApp.ViewModels.Marketing
{
    public class ProblemSolutionGroup
    {
        public string ProblemIcon { get; set; } = "x_red_style_icon.png";

        public string Problem { get; set; }

        public string SolutionIcon { get; set; } = "green_tick_style_icon.png";

        public string Solution { get; set; }
    }
}
