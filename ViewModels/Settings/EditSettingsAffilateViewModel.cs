
using OvulaeApp.Services.LocalDataService;
using OvulaeShared.Enums;
using OvulaeShared.Enums.Status;
using OvulaeShared.Helpers.CommonFunctions;
using OvulaeShared.ViewModel.Affiliates;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace OvulaeApp.ViewModels.Settings
{
    public class EditSettingsAffilateViewModel : BaseViewModel
    {
        public AffiliateEntryViewModel AffiliateFacebook { get; set; } = new();
        public AffiliateEntryViewModel AffiliateX { get; set; } = new();
        public AffiliateEntryViewModel AffiliateInstagram { get; set; } = new();
        public AffiliateEntryViewModel AffiliateYoutube { get; set; } = new();
        public AffiliateEntryViewModel AffiliateTiktok { get; set; } = new();

        public AffiliateOverviewDetails AffiliateDetails { get; set; } = new();

        public bool IsAffiliateMember { get; set; }
        public string ApplicationDateDate { get; set; }
        public string ApplicationStatus { get; set; }

        public EditSettingsAffilateViewModel()
        {
            LoadData();
        }

        public void LoadData()
        {
            try
            {
                if (LocalStorageService.AffiliateOverviewDetails != null)
                {
                    var affiliateData = LocalStorageService.AffiliateOverviewDetails;
                    AffiliateDetails.AndroidJoins = affiliateData.AndroidJoins;
                    AffiliateDetails.AndroidJoinLink = affiliateData.AndroidJoinLink;
                    AffiliateDetails.IOSJoinLink = affiliateData.IOSJoinLink;
                    AffiliateDetails.IOSJoins = affiliateData.IOSJoins;
                    AffiliateDetails.TotalRevenue = affiliateData.TotalRevenue;
                    
                    IsAffiliateMember = true;

                    ApplicationDateDate = $"{affiliateData.AffiliateProfile.CreateDate: dd MMM yyyy}";
                    ApplicationStatus = affiliateData.AffiliateProfile.ProfileStatus.GetDisplayName();

                    if (affiliateData.AffiliateProfile != null && affiliateData.AffiliateProfile.ProfileStatus == StatusType.Pending)
                    {
                        var profile = affiliateData.AffiliateProfile;
                        AffiliateFacebook.Update(profile.FacebookHandleLink, profile.FacebookFollowers, "Facebook");
                        AffiliateInstagram.Update(profile.InstagramHandleLink, profile.InstagramFollowers, "Instagram");
                        AffiliateTiktok.Update(profile.TiktokHandleLink, profile.TiktokFollowers, "Tiktok");
                        AffiliateYoutube.Update(profile.YouTubeHandleLink, profile.YouTubeFollowers, "YouTube");
                        AffiliateX.Update(profile.XHandleLink, profile.XFollowers, "X");
                    }
                }
                else
                {
                    AffiliateDetails = DefaultValueHelper.CreateWithDefaults<AffiliateOverviewDetails>();

                    AffiliateFacebook.Update("", 0, "Facebook");
                    AffiliateInstagram.Update("", 0, "Instagram");
                    AffiliateTiktok.Update("", 0, "Tiktok");
                    AffiliateYoutube.Update("", 0, "YouTube");
                    AffiliateX.Update("", 0, "X");
                }
            }
            catch
            {
                
            }
        }
    }
}
