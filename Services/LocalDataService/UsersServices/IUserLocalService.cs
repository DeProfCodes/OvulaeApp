using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OvulaeShared.Enums.App;
using OvulaeShared.Enums.Status;
using OvulaeShared.Models.User;
using OvulaeShared.Models.WebApi;
using OvulaeShared.ViewModel.User;

namespace OvulaeApp.Services.LocalDataService.UsersServices
{
    public interface IUserLocalService
    {
        //Saves
        public Task<bool> SaveUserFullProfile(UserFullProfileViewModel data);

        public Task<bool> SaveLocalData();

        //Updates
        public Task<bool> UpdateAppPrimaryGoal(ModuleType newPrimaryGoal);

        public Task<bool> UpdateProfileFromLogin(UserModel data, bool forceReload = false);

        public Task<bool> UpdateUserCycleProfile();

        public Task<bool> UpdateUserBodyMetrics();

        public Task<GenericResult> UpdateUserDetails(UserDetailsUpdateViewModel updateViewModel);

        public Task<GenericResult> UpdatePhoneNumber(string countryCode, string phoneNumber);

        public Task<GenericResult> UpdateUserSubscription();

        public Task<GenericResult> UpdateUserSubscription(StatusType newStatus);
        
        //Gets
        public Task<bool> ReloadUserData(UserFullProfileViewModel profile = null);

        public Task<bool> GetUserProfile(bool forceReload = false);

        public Task<bool> SaveAffiliateJoinLink();

        public Task<bool> LoadAffiliateJoinLink();
        
        public Task<UserCycleProfile> GetUserCycleDetails();

        
        public Task<bool> Logout();
        
    }
}
