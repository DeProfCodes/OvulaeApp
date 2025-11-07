using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OvulaeShared.Models.User;
using OvulaeShared.ViewModel.User;

namespace OvulaeApp.Services.LocalDataService
{
    public interface ILocalDbService
    {
        public Task<bool> LoadStartupData();

    }
}
