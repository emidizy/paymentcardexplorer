using System;
using System.Collections.Generic;
using System.Text;

namespace Utilities
{
    public class Logger
    {
        private static readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        public void LogInfo(string message)
        {
            _logger.Info(message);
        }
        public void LogError(string message)
        {
            _logger.Error(message);
        }
    }
}
