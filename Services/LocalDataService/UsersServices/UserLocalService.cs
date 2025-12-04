using OvulaeApp.Helpers.Functions;
using OvulaeApp.Models;
using OvulaeShared.Enums;
using OvulaeShared.Enums.App;
using OvulaeShared.Enums.Status;
using OvulaeShared.Enums.User;
using OvulaeShared.Helpers.CommonFunctions;
using OvulaeShared.Helpers.ModuleHelpers;
using OvulaeShared.Models.User;
using OvulaeShared.Models.WebApi;
using OvulaeShared.Services.APIs.Users;
using OvulaeShared.ViewModel.Account;
using OvulaeShared.ViewModel.User;

namespace OvulaeApp.Services.LocalDataService.UsersServices
{
    public class UserLocalService : IUserLocalService
    {
        private readonly IUsersApi _userApi;
        private const string UsersFile = "UserFullProfile.json";
        private const string ExtraExtDataFile = "ExtraExternalData.json";

        public event Action? CycleProfileChanged;

        public UserLocalService(IUsersApi userApi)
        {
            _userApi = userApi;
        }

        #region GetSetUpdate - SET

        public async Task<bool> SaveUserFullProfile(UserFullProfileViewModel data)
        {
            try
            {
                var saved = await FileReaderHelper.SaveDataAsync(data, UsersFile);

                return saved;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> SaveLocalData()
        {
            try
            {
                var data = new UserFullProfileViewModel
                {
                    UserDetails = LocalStorageService.UserDetails,
                    UserBodyMetrics = LocalStorageService.UserBodyMetrics,
                    UserProfileCycle = LocalStorageService.UserCycleProfile,
                    UserSubscription = LocalStorageService.UserSubscription,
                    PartnerDetails = LocalStorageService.PartnerDetails,
                    AffiliateDetailsOverview = LocalStorageService.AffiliateOverviewDetails,
                };

                return await SaveUserFullProfile(data);
            }
            catch
            {
                return false;
            }
        }

        #endregion

        #region GetSetUpdate - UPDATE

        public async Task<bool> UpdateProfileFromLogin(UserModel data, bool forceReload = false)
        {
            try
            {
                if (forceReload || LocalStorageService.UserBodyMetrics.Id == 0 || LocalStorageService.UserSubscription.Id == 0)
                {
                    var fullDBProfile = await _userApi.GetUserFullProfile(data.UserId, data.UserRole);

                    if (fullDBProfile != null)
                    {
                        LocalStorageService.UserDetails = data;
                        LocalStorageService.UserBodyMetrics = fullDBProfile.UserBodyMetrics;
                        LocalStorageService.UserCycleProfile = fullDBProfile.UserProfileCycle;
                        LocalStorageService.UserSubscription = fullDBProfile.UserSubscription;
                        LocalStorageService.PartnerDetails = fullDBProfile.PartnerDetails;
                    }
                }

                var payload = new UserFullProfileViewModel
                {
                    UserDetails = data,
                    UserBodyMetrics = LocalStorageService.UserBodyMetrics,
                    UserProfileCycle = LocalStorageService.UserCycleProfile,
                    UserSubscription = LocalStorageService.UserSubscription,
                    PartnerDetails = LocalStorageService.PartnerDetails,
                };

                if (LocalStorageService.UserCycleProfile.OvulaePrimaryGoal == ModuleType.Pregnancy.GetDisplayName())
                {
                    var lmp = LocalStorageService.UserCycleProfile.LastPeriodDate;
                    var dueDate = LocalStorageService.UserCycleProfile.EstimatedDueDate;
                    LocalStorageService.PregnancyData.PregnancyDataCurrentWeek = SharedCommonFunctions.GetCurrentPregnancyWeekFromLMPOrDueDate(lmp, dueDate);
                }

                var saved = await SaveUserFullProfile(payload);

                return saved;
            }
            catch
            {
                return false;
            }
        }

        //Update to API
        public async Task<bool> UpdateAppPrimaryGoal(ModuleType newPrimaryGoal)
        {
            try
            {
                var payload = new UserCycleProfile
                {
                    UserId = LocalStorageService.UserDetails.UserId,
                    OvulaePrimaryGoal = newPrimaryGoal.GetDisplayName(),
                    LastPeriodDate = LocalStorageService.UserCycleProfile.LastPeriodDate,
                    CycleLengthDays = LocalStorageService.UserCycleProfile.CycleLengthDays,
                    PeriodLengthDays = LocalStorageService.UserCycleProfile.PeriodLengthDays,
                    HealthConditions = LocalStorageService.UserCycleProfile.HealthConditions,
                    Treatments = LocalStorageService.UserCycleProfile.Treatments,
                    Symptoms = LocalStorageService.UserCycleProfile.Symptoms,
                    PeriodIrregularityType = LocalStorageService.UserCycleProfile.PeriodIrregularityType,
                };

                payload.AllOvulaeGoals.Add(newPrimaryGoal.GetDisplayName());

                var updateStatus = await _userApi.UpdateUserCycleProfile(payload);

                if (updateStatus.Success)
                {
                    LocalStorageService.UserCycleProfile.AllOvulaeGoals.Add(newPrimaryGoal.GetDisplayName());
                    LocalStorageService.UserCycleProfile.OvulaePrimaryGoal = newPrimaryGoal.GetDisplayName();
                    LocalStorageService.AppPrimaryGoal = newPrimaryGoal;

                    await SaveLocalData();

                    CycleProfileChanged?.Invoke();

                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> UpdateUserCycleProfile()
        {
            try
            {
                var updateStatus = await _userApi.UpdateUserCycleProfile(LocalStorageService.UserCycleProfile);

                if (updateStatus.Success)
                {
                    await SaveLocalData();
                    CycleProfileChanged?.Invoke();
                }

                await ReloadUserData();

                return updateStatus.Success;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> UpdateUserBodyMetrics()
        {
            try
            {
                var updateStatus = await _userApi.UpdateBodyMetrics(LocalStorageService.UserBodyMetrics, LocalStorageService.UserDetails.Email);

                if (updateStatus.Success)
                {
                    await SaveLocalData();
                }

                await ReloadUserData();

                return updateStatus.Success;
            }
            catch
            {
                return false;
            }
        }

        public async Task<GenericResult> UpdateUserDetails(UserDetailsUpdateViewModel updateViewModel)
        {
            try
            {
                var updateStatus = await _userApi.UpdateUserDetails(updateViewModel);

                if (updateStatus.Success)
                {
                    await SaveLocalData();
                }

                await ReloadUserData();

                return updateStatus;
            }
            catch
            {
                return new GenericResult();
            }
        }

        public async Task<GenericResult> UpdatePhoneNumber(string countryCode, string phoneNumber)
        {
            try
            {
                var payload = new PasswordResetViewModel
                {
                    Email = LocalStorageService.UserDetails.Email,
                    CountryCode = countryCode,
                    PhoneNumber = phoneNumber
                };

                var updateStatus = await _userApi.UpdateUserPhoneNumber(payload);

                if (updateStatus.Success)
                {
                    LocalStorageService.UserDetails.CountryCode = countryCode;
                    LocalStorageService.UserDetails.PhoneNumber = phoneNumber;
                    await SaveLocalData();
                }
                return updateStatus;
            }
            catch
            {
                return new GenericResult();
            }
        }

        public async Task<GenericResult> UpdateUserSubscription()
        {
            try
            {
                var updateStatus = await _userApi.UpdateUserSubscription(LocalStorageService.UserSubscription);
                if (updateStatus.Success)
                {
                    await ReloadUserData();
                    await SaveLocalData();
                }
                return updateStatus;
            }
            catch
            {
                return new GenericResult();
            }
        }

        public async Task<GenericResult> UpdateUserSubscription(StatusType newStatus)
        {
            var oldStatus = LocalStorageService.UserSubscription.Status;
            LocalStorageService.UserSubscription.Status = newStatus;
            
            var updated = await UpdateUserSubscription();
            
            if (!updated.Success)
                LocalStorageService.UserSubscription.Status = oldStatus;

            return updated;
        }

        #endregion

        #region GetSetUpdate - Get

        public async Task<bool> ReloadUserData(UserFullProfileViewModel profile = null)
        {
            try
            {
                if (profile == null)
                    profile = await FileReaderHelper.GetAllFileData<UserFullProfileViewModel>(UsersFile);

                var fullDBProfile = await _userApi.GetUserFullProfile(profile.UserDetails.UserId, profile.UserDetails.UserRole);

                LocalStorageService.UserDetails = fullDBProfile.UserDetails;
                LocalStorageService.UserBodyMetrics = fullDBProfile.UserBodyMetrics;
                LocalStorageService.UserCycleProfile = fullDBProfile.UserProfileCycle;
                LocalStorageService.UserSubscription = fullDBProfile.UserSubscription;
                LocalStorageService.PartnerDetails = fullDBProfile.PartnerDetails;
                LocalStorageService.AffiliateOverviewDetails = fullDBProfile.AffiliateDetailsOverview;

                return await SaveLocalData();
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> GetUserProfile(bool forceReload = false)
        {
            try
            {
                var profile = await FileReaderHelper.GetAllFileData<UserFullProfileViewModel>(UsersFile);

                forceReload = profile.UserDetails.UserRole == UserRoleType.PartnerShare ? true : forceReload;

                var relaod = forceReload || (profile != null && profile.UserSubscription != null && profile.UserSubscription.Status != StatusType.Active);

                if (relaod)
                {
                    await ReloadUserData(profile);
                    profile = await FileReaderHelper.GetAllFileData<UserFullProfileViewModel>(UsersFile);
                }

                if (profile != null)
                {
                    LocalStorageService.UserDetails = profile.UserDetails;
                    LocalStorageService.UserBodyMetrics = profile.UserBodyMetrics;
                    LocalStorageService.UserCycleProfile = profile.UserProfileCycle;
                    LocalStorageService.UserSubscription = profile.UserSubscription;
                    LocalStorageService.PartnerDetails = profile.PartnerDetails;
                    LocalStorageService.AffiliateOverviewDetails = profile.AffiliateDetailsOverview;


                    var userCycle = LocalStorageService.UserCycleProfile;

                    if (userCycle.OvulaePrimaryGoal == ModuleType.Pregnancy.GetDisplayName())
                    {
                        LocalStorageService.PregnancyData.PregnancyDataCurrentWeek = SharedCommonFunctions.GetCurrentPregnancyWeekFromLMPOrDueDate(userCycle.LastPeriodDate, userCycle.EstimatedDueDate);
                    }

                    LocalStorageService.PeriodTrackerSet = true;
                }
                else
                {
                    var emptyFile = new UserFullProfileViewModel
                    {
                        UserBodyMetrics = DefaultValueHelper.CreateWithDefaults<UserBodyMetric>(),
                        UserSubscription = DefaultValueHelper.CreateWithDefaults<UserSubscription>(),
                        UserDetails = DefaultValueHelper.CreateWithDefaults<UserModel>(),
                        UserProfileCycle = DefaultValueHelper.CreateWithDefaults<UserCycleProfile>(),
                        PartnerDetails = DefaultValueHelper.CreateWithDefaults<UserPartnerViewModel>()
                    };
                    await SaveUserFullProfile(emptyFile);
                }
                if (LocalStorageService.UserCycleProfile.LastPeriodDate == null)
                {
                    LocalStorageService.UserCycleProfile.LastPeriodDate = DateTime.Now.AddMonths(-1);
                    LocalStorageService.PeriodTrackerSet = true;
                }
                LocalStorageService.AppPrimaryGoal = EnumHelper.GetEnumValueFromName<ModuleType>(LocalStorageService.UserCycleProfile.OvulaePrimaryGoal);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> SaveAffiliateJoinLink()
        {
            try
            {
                var data = new ExtraExternalData
                {
                    AffiliateJoinLink = LocalStorageService.AffiliateJoinCode    
                };
                var saved = await FileReaderHelper.SaveDataAsync(data, ExtraExtDataFile);
                return saved;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> LoadAffiliateJoinLink()
        {
            try
            {
                var data = await FileReaderHelper.GetAllFileData<ExtraExternalData>(ExtraExtDataFile);

                LocalStorageService.AffiliateJoinCode = data.AffiliateJoinLink;

                return !string.IsNullOrEmpty(data.AffiliateJoinLink);
            }
            catch
            {
                return false;
            }
        }

        public async Task<UserCycleProfile> GetUserCycleDetails()
        {
            try
            {
                if (LocalStorageService.UserCycleProfile != null && LocalStorageService.UserCycleProfile.PeriodLengthDays != null)
                {
                    return LocalStorageService.UserCycleProfile;
                }
                else
                {
                    await ReloadUserData();

                    return LocalStorageService.UserCycleProfile;
                }
            }
            catch
            {
                return null;
            }
        }

        #endregion

        public async Task<bool> Logout()
        {
            try
            {
                LocalStorageService.UserDetails = DefaultValueHelper.CreateWithDefaults<UserModel>();
                LocalStorageService.UserBodyMetrics.Id = 0;

                return await SaveLocalData();
            }
            catch
            {
                return false;
            }
        }
    }
}
