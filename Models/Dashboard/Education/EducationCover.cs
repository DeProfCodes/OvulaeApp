using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OvulaeShared.Enums.App;

namespace OvulaeApp.Models.Dashboard.Education
{
    public class EducationCover
    {
        public int BookId { get; set; }

        public string ThumbnailSource { get; set; }

        public bool Bookmarked { get; set; }

        public string Type { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public List<string> Categories { get; set; }
    }
}
