using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OvulaeApp.Models.Shared
{
    public class BodyMetric
    {
        public int BirthYear { get; set; } = 2005;

        public double Weight { get; set; } = 60;

        public string WeightMeasure { get; set; } = "kg";

        public double Height { get; set; } = 160;

        public string HeightMeasure { get; set; } = "cm";
    }
}
