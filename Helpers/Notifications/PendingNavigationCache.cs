
namespace OvulaeApp.Helpers.Notifications
{
    public static class PendingNavigationCache
    {
        private static readonly object _lock = new();
        private static (string module, string entryId)? _pending;

        public static void Save(string module, string entryId)
        {
            if (string.IsNullOrEmpty(module) || string.IsNullOrEmpty(entryId)) 
                return;

            lock (_lock) 
            { 
                _pending = (module, entryId);
            }
        }

        public static (string module, string entryId)? Consume()
        {
            lock (_lock)
            {
                var p = _pending;
                _pending = null;
                return p;
            }
        }
    }
}
