using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OvulaeApp.ViewModels.Settings
{
    public class AffiliateEntryViewModel : BaseViewModel
    {
        private string _handle;
        public string Handle 
        { 
            get => _handle;
            set
            {
                _handle = value;
                OnPropertyChanged();
            }
        }

        private int _followersCount;
        public int FollowersCount 
        { 
            get => _followersCount;
            set
            {
                _followersCount = value;
                OnPropertyChanged();
            }
        }

        private string _type;
        public string Type
        {
            get => _type;
            set
            {
                _type = value;
                OnPropertyChanged();
            }
        }

        public void Update(string handlle="", int followers = 0, string type="")
        {
            Handle = handlle;
            FollowersCount = followers;
            Type = type;
        }
    }
}
