using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;

namespace LoggingModule
{
    public class Logger
    {
        public static string ServerPath;

        private bool enableLogging = false;
        private string filePath = string.Empty;

        private readonly object lockObj = new object();

        private Logger()
        {
            enableLogging = WebSiteSettings.GetSettingValue<bool>(SettingKeys.ENABLE_LOGGING);
            if (enableLogging)
            {
                filePath = ServerPath + WebSiteSettings.GetSettingValue<string>(SettingKeys.LOG_FILE_NAME);
            }
        }

        private static Logger _instance = null;

        public static Logger Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new Logger();
                }

                return _instance;
            }
        }

        private void WriteLine(string message)
        {
            if (!enableLogging || _instance == null || string.IsNullOrWhiteSpace(filePath))
            {
                return;
            }

            lock(lockObj)
            {
                using (StreamWriter streamWriter = new StreamWriter(filePath))
                {
                    streamWriter.WriteLine(message);

                    streamWriter.Close();
                }
            }
        }

        public void LogError(string error, Exception ex)
        {
            if (_instance != null)
            {
                WriteLine($"{DateTime.Now.ToString()}: ERROR\n{error}\n{ex.Message}\n{ex.StackTrace}");
            }
        }

        public void LogWarning(string warninng)
        {
            if (_instance != null)
            {
                WriteLine($"{DateTime.Now.ToString()}: WARNING\n{warninng}");
            }
        }

        public void LogInfo(string info)
        {
            if (_instance != null)
            {
                WriteLine($"{DateTime.Now.ToString()}: INFO\n{info}");
            }
        }
    }
}
