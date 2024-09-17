using System.Text;

namespace OpenRoadHelper
{
    public static class Logger
    {
        private static readonly StringBuilder messages = new();
        private static string _logDir = string.Empty;
        private static bool _running = false;
        private static DateTime _lastWrite;
        private static int _logDelay = 3;

        public static async Task Shutdown()
        {
            _running = false;
            while (DateTime.Now.CompareTo(_lastWrite.AddSeconds(_logDelay)) <= 0)
            {
                await Task.Yield();
            }
            WriteLog();
        }

        public static void Info(string? message)
        {
            message ??= string.Empty;
            message = $"{GetTimeStamp()}|INFO: {message}";
            Console.WriteLine(message);
            AppendLog(message);
        }

        public static void Warn(string? message)
        {
            message ??= string.Empty;
            message = $"{GetTimeStamp()}|WARN: {message}";
            Console.WriteLine(message);
            AppendLog(message);
        }

        public static void Error(string? message)
        {
            message ??= string.Empty;
            message = $"{GetTimeStamp()}|ERROR: {message}";
            Console.WriteLine(message);
            AppendLog(message);
        }

        private static void AppendLog(string message)
        {
            messages.AppendLine($"{message}");
        }

        private static async Task UpdateLog()
        {
            await Task.Yield();
            if (_running)
            {
                return;
            }
            _running = true;
            WriteLog();
            while (_running)
            {
                await Task.Yield();
                if (DateTime.Now.CompareTo(_lastWrite.AddSeconds(_logDelay)) > 0)
                {
                    WriteLog();
                }
            }
        }

        private static void WriteLog()
        {
            _lastWrite = DateTime.Now;
            File.AppendAllText(_logDir + "latest.log", messages.ToString());
            messages.Clear();
        }

        public static void Initialize(int delay)
        {
            _logDelay = delay;
            _logDir = @$".\log\";
            Directory.CreateDirectory(_logDir);

            string latestLog = _logDir + "latest.log";
            if (File.Exists(latestLog))
            {
                string newFileName = GetTimeStamp(File.GetCreationTime(latestLog), true);
                File.Copy(latestLog, _logDir + newFileName + ".log");
                File.Delete(latestLog);
            }
            Task.Run(UpdateLog);
        }

        private static string GetTimeStamp(DateTime dateTime, bool fileSafe = false)
        {
            string timestamp = dateTime.ToString("yyyy-mm-dd hh:mm:ss");
            if (fileSafe)
            {
                timestamp = dateTime.ToString("yyyy-mm-dd_hh-mm-ss");
            }
            return timestamp;
        }

        private static string GetTimeStamp()
        {
            return DateTime.Now.ToString("<yyyy-mm-dd hh:mm:ss>");
        }
    }
}
