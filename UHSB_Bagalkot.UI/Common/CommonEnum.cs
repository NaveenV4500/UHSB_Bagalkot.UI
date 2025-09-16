namespace UHSB_Bagalkot.UI.Common
{
    public class CommonEnum
    {
        private static readonly string logFilePath = Path.Combine(AppContext.BaseDirectory, "Logs", "login_logs.txt");

        public static void WriteLog(string message)
        {
            try
            {
                string logDir = Path.Combine(AppContext.BaseDirectory, "Logs");
                if (!Directory.Exists(logDir))
                    Directory.CreateDirectory(logDir);

                string logMessage = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - {message}";
                File.AppendAllText(logFilePath, logMessage + Environment.NewLine);
            }
            catch
            {
                // Prevent any exception in logging from breaking the app
            }
        }
    }
}
