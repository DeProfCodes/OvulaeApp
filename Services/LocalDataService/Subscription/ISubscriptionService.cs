
namespace OvulaeApp.Services.LocalDataService.Subscription
{
    public interface ISubscriptionService
    {
        public Task RunSubscriptionCheck(bool forceActivate = false);
    }
}
