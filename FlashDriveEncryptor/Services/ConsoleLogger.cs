namespace FlashDriveEncryptor.Services
{
    enum LogLevel
    {
        ERROR = -1,
        INFORMATION = 1
    }
    internal class ConsoleLogger : ILogger
    {
        /// <summary>
        /// Locker object. Designed to synchronize the logging method.
        /// </summary>
        private readonly object _lock = new object();

        /// <summary>
        /// Message output stream.
        /// </summary>
        private readonly TextWriter _output;

        /// <summary>
        /// Initializes ConsoleLogger with an indication of the message output stream.
        /// </summary>
        /// <param name="output">Message output stream.</param>
        public ConsoleLogger(TextWriter output = null)
        {
            _output = output ?? Console.Out;
        }

        /// <summary>
        /// The main method of logging.
        /// </summary>
        private void Log(LogLevel logLevel, string message)
        {
            lock (_lock)
            {
                var dateTimeNow = DateTime.Now;
                var formattedMessage = BuildMessage(dateTimeNow, logLevel.ToString(), message);
                _output.WriteLine(formattedMessage);
            }
        }

        /// <summary>
        /// Error logging method.
        /// </summary>
        /// <param name="message"></param>
        public void Error(string message)
        {
            Log(LogLevel.ERROR, message);
        }

        /// <summary>
        /// A method for error logging with reference to an exception.
        /// </summary>
        /// <param name="ex">An error occurred.</param>
        /// <param name="message"></param>
        public void Error(string message, Exception ex)
        {
            Log(LogLevel.ERROR, $"{message} | {ex}");
        }

        /// <summary>
        /// A method for logging information messages.
        /// </summary>
        /// <param name="message"></param>
        public void Info(string message)
        {
            Log(LogLevel.INFORMATION, message);
        }

        /// <summary>
        /// A method for composing a message.
        /// </summary>
        /// <param name="dateTimeNow">Date of logging.</param>
        /// <param name="logLevel"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        private string BuildMessage(DateTime dateTimeNow, string logLevel, string message)
        {
            return $"[{dateTimeNow}] [{logLevel}]: {message}";
        }
    }
}
