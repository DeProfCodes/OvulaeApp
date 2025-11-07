using Android.Content;
using Com.Android.Installreferrer.Api;

namespace OvulaeApp.Services.Platform
{
    public class InstallReferrerService : Java.Lang.Object, IInstallReferrerStateListener
    {
        private InstallReferrerClient _referrerClient;
        private Action<string> _onReferrerReceived;

        public void GetReferrer(Context context, Action<string> onReferrerReceived)
        {
            try
            {
                _onReferrerReceived = onReferrerReceived;
                _referrerClient = InstallReferrerClient.NewBuilder(context).Build();
                _referrerClient.StartConnection(this);
            }
            catch
            {
                
            }
        }

        public void OnInstallReferrerSetupFinished(int responseCode)
        {
            try
            {
                if (responseCode == InstallReferrerClient.InstallReferrerResponse.Ok)
                {
                    var referrer = _referrerClient.InstallReferrer.InstallReferrer;
                    _onReferrerReceived?.Invoke(referrer);
                }
                else
                {
                    _onReferrerReceived?.Invoke(null);
                }
            }
            catch
            {
                
            }
        }

        public void OnInstallReferrerServiceDisconnected()
        {
            // Optional: handle disconnection
        }
    }


}
