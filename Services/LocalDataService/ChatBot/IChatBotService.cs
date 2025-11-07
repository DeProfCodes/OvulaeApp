using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OvulaeApp.ViewModels.Chat;
using OvulaeShared.Enums.App;

namespace OvulaeApp.Services.LocalDataService.ChatBot
{
    public interface IChatBotService
    {
        public List<ChatFAQItem> GetAllModulesFAQs();

        public List<ChatFAQItem> GetModuleFAQs(ModuleType moduleType);

    }
}
