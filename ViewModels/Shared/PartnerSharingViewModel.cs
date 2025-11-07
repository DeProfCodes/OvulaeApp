using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OvulaeApp.Services.LocalDataService;
using OvulaeShared.Enums;
using OvulaeShared.Enums.App;
using OvulaeShared.Enums.Status;

namespace OvulaeApp.ViewModels.Shared
{
    public class PartnerSharingViewModel : BaseViewModel
    {
        private bool _isSharing;
        public bool IsSharing
        {
            get => _isSharing;
            set
            {
                _isSharing = value;
                OnPropertyChanged();
            }
        }

        private string _fullname;
        public string Fullname
        {
            get => _fullname;
            set
            {
                _fullname = value;
                OnPropertyChanged();
            }
        }

        private string _email;
        public string Email
        {
            get => _email;
            set
            {
                _email = value;
                OnPropertyChanged();
            }
        }

        private string _phoneNumber;
        public string PhoneNumber
        {
            get => _phoneNumber;
            set
            {
                _phoneNumber = value;
                OnPropertyChanged();
            }
        }

        private string _date;
        public string Date
        {
            get => _date;
            set
            {
                _date = value;
                OnPropertyChanged();
            }
        }

        private string _revokeDate;
        public string RevokeDate
        {
            get => _revokeDate;
            set
            {
                _revokeDate = value;
                OnPropertyChanged();
            }
        }

        private string _status;
        public string Status
        {
            get => _status;
            set
            {
                _status = value;
                OnPropertyChanged();
            }
        }

        private bool _revoked;
        public bool Revoked
        {
            get => _revoked;
            set
            {
                _revoked = value;
                OnPropertyChanged();
            }
        }

        public string Title { get; set; }
        public string HeroImage { get; set; }
        public string SubTitleNotSharing { get; set; }
        public string SubTitleSharing { get; set; }

        public PartnerSharingViewModel()
        {
            ReloadSharingInformation();
        }

        public void ReloadSharingInformation()
        {
            try
            {
                var moduleType = LocalStorageService.AppPrimaryGoal;

                Title = moduleType switch
                {
                    ModuleType.Pregnancy => "Pregnancy Partner Sharing",
                    ModuleType.Ovulation => "Ovulation Partner Sharing",
                    ModuleType.PeriodTracker => "Period Partner Sharing",
                    ModuleType.MenopauseTracker => "Menopause Partner Sharing",
                    _ => "Partner Sharing"
                };

                HeroImage = moduleType switch
                {
                    ModuleType.Pregnancy => "pregnancy_partner_sharing_hero.png",
                    ModuleType.Ovulation => "ovulation_partner_sharing.png",
                    ModuleType.PeriodTracker => "period_partner_sharing.png",
                    ModuleType.MenopauseTracker => "menopause_partner_sharing_hero.png",
                    _ => "Partner Sharing"
                };

                SubTitleNotSharing = moduleType switch
                {
                    ModuleType.Pregnancy => "You’re currently not sharing your Pregnancy Tracking!",
                    ModuleType.Ovulation => "You’re currently not sharing your Ovulation Tracking!",
                    ModuleType.PeriodTracker => "You’re currently not sharing your Menstrual Tracking!",
                    ModuleType.MenopauseTracker => "You’re currently not sharing your Menopause Tracking!",
                    _ => "You’re currently not sharing your Tracking!"
                };

                SubTitleSharing = moduleType switch
                {
                    ModuleType.Pregnancy => "You’re sharing your Pregnancy Tracker",
                    ModuleType.Ovulation => "You’re sharing your Ovulation Tracker",
                    ModuleType.PeriodTracker => "You’re sharing your Period Tracker",
                    ModuleType.MenopauseTracker => "You’re sharing your Menopause Tracker",
                    _ => "You’re sharing your Tracking!"
                };


                var partnerDetails = LocalStorageService.PartnerDetails;
                IsSharing = partnerDetails != null && partnerDetails.AccessStatus != AccountStatusType.Deleted;

                if (IsSharing)
                {
                    Fullname = $"{partnerDetails.Firstname} {partnerDetails.Lastname}";
                    Email = partnerDetails.Email;
                    PhoneNumber = $"{partnerDetails.CountryCode} {partnerDetails.PhoneNumber}";
                    Date = $"{partnerDetails.InviteDate: dd-MMM-yyyy}";
                    RevokeDate = $"{partnerDetails.LastUpdateDate: dd-MMM-yyyy}";
                    Revoked = partnerDetails.AccessStatus == AccountStatusType.Revoked;
                    Status = Revoked ? "REVOKED" : "ACTIVE";
                }
            }
            catch
            {
                
            }
        }
    }
}
