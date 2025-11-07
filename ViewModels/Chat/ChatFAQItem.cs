
using OvulaeShared.Enums.App;

namespace OvulaeApp.ViewModels.Chat
{
    public class ChatFAQItem
    {
        public ModuleType ModuleType { get; set; }

        public List<string> Keywords { get; set; }

        public string Question { get; set; }

        public string Answer { get; set; }
    }
}
