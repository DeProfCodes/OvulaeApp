using OvulaeApp.Services.LocalDataService.EducationServices;
using OvulaeShared.Enums.App;
using OvulaeShared.Models.Education;
using OvulaeShared.ViewModel.Education;

namespace OvulaeApp.ViewModels.Education
{
    public class EducationDetailsViewModel
    {
        public EducationGroupItemsViewModel EduData { get; set; }

        public EducationDetailsViewModel(IEducationService eduServ, int bookId)
        {
            try
            {
                EduData = eduServ.GetEducationBookDetails(bookId);

                if (EduData.Diagrams != null)
                {
                    EduData.Diagrams.ForEach(item =>
                    {
                        if (item != null)
                        {
                            item.FileName = $"Eudcation/diagrams/{item.FileName}";
                        }
                    });
                }
            }
            catch 
            {

            }
        }
    }
}
