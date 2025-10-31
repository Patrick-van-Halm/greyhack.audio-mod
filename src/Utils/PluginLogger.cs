using BepInEx.Logging;

namespace Utils.Logging
{
    class PluginLogger
    {
        private readonly ManualLogSource _logger;
        internal void LogInfo(string text) => _logger.LogInfo(text);
        internal void LogError(string text) => _logger.LogError(text);
        internal void LogDebug(string text) => _logger.LogDebug(text);

        public PluginLogger(ManualLogSource logger)
        {
            _logger = logger;
        }
    }
}
