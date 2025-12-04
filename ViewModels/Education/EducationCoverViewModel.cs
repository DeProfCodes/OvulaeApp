using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OvulaeApp.Services.LocalDataService.EducationServices;
using OvulaeApp.Services.LocalDataService;
using OvulaeApp.Views.Components.Modals;
using OvulaeShared.Enums.App;
using OvulaeApp.Views.Education;
using OvulaeShared.ViewModel.Education;

namespace OvulaeApp.ViewModels.Education
{
    public class EducationCoverViewModel : BaseViewModel
    {
        private List<EducationCoverGroup> AllEducationGroupsCovers { get; set; } // Keep unfiltered data

        public List<EducationCoverGroup> EducationGroupsCovers { get; set; }

        public List<string> Categories { get; set; } = new List<string>() { "All" };

        public string OurGynacologist { get; set; }

        public Command<EducationCover> OnCoverTappedCommand { get; }

        private SpinnerLoader spinner {get; set;}

        public Command<string> OnCategoryTappedCommand { get; }

        public EducationCoverViewModel(IEducationService eduServ, SpinnerLoader spinner)
        {
            LoadPageData(eduServ);
            OnCoverTappedCommand = new Command<EducationCover>(OnCoverTapped);
            this.spinner = spinner;
            OnCategoryTappedCommand = new Command<string>(OnCategoryTapped);
        }

        private void LoadPageData(IEducationService eduServ)
        {
            try
            {
                var moduleType = LocalStorageService.AppPrimaryGoal;
                AllEducationGroupsCovers = eduServ.GetEducationGroupsCovers(moduleType);
                EducationGroupsCovers = eduServ.GetEducationGroupsCovers(moduleType);

                var categories = EducationGroupsCovers.SelectMany(e => e.EducationCovers.SelectMany(x => x.Categories)).Distinct().ToList();

                Categories.AddRange(categories);

                //OurGynacologist = $"Dr. {LocalStorageService.OvulaeGynacologist.Firstname} {LocalStorageService.OvulaeGynacologist.Lastname}, OB/GYN";
            }
            catch(Exception ex) 
            {
                Console.WriteLine($"Failed to load education data: {ex.Message}");
                Application.Current.MainPage.DisplayAlert("Error", "Something went wrong while loading education data.", "OK");
            }
        }

        private async void OnCoverTapped(EducationCover cover)
        {
            try
            {
                if (cover == null)
                    return;

                int bookId = cover.BookId;

                await spinner.ShowSpinnerAsync();

                await Shell.Current.GoToAsync($"{nameof(EducationDetailsPage)}?bookId={bookId}");

                await spinner.HideSpinnerAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to load education data: {ex.Message}");
                await Application.Current.MainPage.DisplayAlert("Error", "Something went wrong while loading education data.", "OK");
            }
        }

        public void ApplySearchFilter(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                EducationGroupsCovers = new List<EducationCoverGroup>(AllEducationGroupsCovers);
            }
            else
            {
                var lowerQuery = query.ToLowerInvariant();

                EducationGroupsCovers = AllEducationGroupsCovers
                    .Select(group => new EducationCoverGroup
                    {
                        CoversHeading = group.CoversHeading,
                        EducationCovers = group.EducationCovers
                            .Where(cover =>
                                (!string.IsNullOrEmpty(cover.Title) && cover.Title.ToLowerInvariant().Contains(lowerQuery)) ||
                                (cover.Categories != null && cover.Categories.Any(c => c.ToLowerInvariant().Contains(lowerQuery))) ||
                                (!string.IsNullOrEmpty(cover.Type) && cover.Type.ToLowerInvariant().Contains(lowerQuery))
                            )
                            .ToList()
                    })
                    .Where(group => group.EducationCovers.Any())
                    .ToList();
            }

            OnPropertyChanged(nameof(EducationGroupsCovers)); // Notify UI
        }

        private void OnCategoryTapped(string category)
        {
            Console.WriteLine($"Category tapped: {category}");

            if (category == "All")
            {
                EducationGroupsCovers = AllEducationGroupsCovers.ToList();
            }
            else
            {
                EducationGroupsCovers = AllEducationGroupsCovers
                    .Select(group => new EducationCoverGroup
                    {
                        CoversHeading = group.CoversHeading,
                        EducationCovers = group.EducationCovers
                            .Where(cover => cover.Categories.Contains(category))
                            .ToList()
                    })
                    .Where(group => group.EducationCovers.Any())
                    .ToList();
            }

            OnPropertyChanged(nameof(EducationGroupsCovers));
        }


    }
}
