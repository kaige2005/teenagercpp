using System;
using System.IO;
using System.Text;
using System.Threading;

namespace TeenCppEdu.Services.Logger
{
    /// <summary>
    /// 日志服务 - 记录程序运行日志
    /// 默认开启，支持多级别日志，自动文件管理
    /// </summary>
    public class LoggerService
    {
        private static LoggerService _instance;
        private static readonly object _lock = new object();

        private readonly string _logDirectory;
        private readonly string _logLevel;
        private readonly object _fileLock = new object();
        private string _currentLogFile;
        private DateTime _currentFileDate;

        // 日志级别：Debug < Info < Warning < Error < Fatal
        private readonly int _minLogLevel;

        /// <summary>
        /// 获取日志服务实例（单例）
        /// </summary>
        public static LoggerService Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        _instance ??= new LoggerService();
                    }
                }
                return _instance;
            }
        }

        /// <summary>
        /// 私有化构造函数
        /// </summary>
        private LoggerService()
        {
            // 日志目录: %AppData%/TeenCppEdu/logs/
            _logDirectory = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "TeenCppEdu",
                "logs");

            EnsureDirectoryExists();

            // 从配置文件读取日志级别，默认Info
            _logLevel = GetConfigLogLevel();
            _minLogLevel = ParseLogLevel(_logLevel);

            _currentFileDate = DateTime.Now.Date;
            _currentLogFile = GetLogFilePath();

            // 记录服务启动
            Info($"Logger initialized. Level={_logLevel}, Path={_logDirectory}");
        }

        /// <summary>
        /// 确保日志目录存在
        /// </summary>
        private void EnsureDirectoryExists()
        {
            if (!Directory.Exists(_logDirectory))
            {
                Directory.CreateDirectory(_logDirectory);
            }
        }

        /// <summary>
        /// 获取日志级别（DEBUG模式为Debug，Release模式为Info）
        /// </summary>
        private string GetConfigLogLevel()
        {
#if DEBUG
            return "Debug";
#else
            return "Info";
#endif
        }

        /// <summary>
        /// 解析日志级别为数值
        /// </summary>
        private int ParseLogLevel(string level)
        {
            return level?.ToLower() switch
            {
                "debug" => 0,
                "info" => 1,
                "warning" => 2,
                "warn" => 2,
                "error" => 3,
                "fatal" => 4,
                _ => 1  // 默认Info
            };
        }

        /// <summary>
        /// 获取当前日志文件路径（按日期分文件）
        /// </summary>
        private string GetLogFilePath()
        {
            var date = DateTime.Now.ToString("yyyyMMdd");
            return Path.Combine(_logDirectory, $"TeenCppEdu_{date}.log");
        }

        /// <summary>
        /// 检查是否需要切换日志文件（跨天时）
        /// </summary>
        private void CheckRotateFile()
        {
            var today = DateTime.Now.Date;
            if (today != _currentFileDate)
            {
                lock (_fileLock)
                {
                    _currentFileDate = today;
                    _currentLogFile = GetLogFilePath();
                }
                CleanOldLogs(); // 清理旧日志
            }
        }

        /// <summary>
        /// 清理7天前的日志文件
        /// </summary>
        private void CleanOldLogs()
        {
            try
            {
                var files = Directory.GetFiles(_logDirectory, "TeenCppEdu_*.log");
                var cutoff = DateTime.Now.AddDays(-7);

                foreach (var file in files)
                {
                    try
                    {
                        var fileInfo = new FileInfo(file);
                        if (fileInfo.LastWriteTime < cutoff)
                        {
                            fileInfo.Delete();
                        }
                    }
                    catch { /* 忽略单个文件删除失败 */ }
                }
            }
            catch { /* 忽略清理失败 */ }
        }

        /// <summary>
        /// 写日志核心方法
        /// </summary>
        private void WriteLog(string level, string message, Exception exception = null)
        {
            int levelValue = ParseLogLevel(level);
            if (levelValue < _minLogLevel)
                return;

            CheckRotateFile();

            var timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
            var threadId = Thread.CurrentThread.ManagedThreadId;
            var logEntry = new StringBuilder();

            logEntry.AppendLine($"[{timestamp}] [{level}] [T{threadId}] {message}");

            if (exception != null)
            {
                logEntry.AppendLine($"Exception: {exception.GetType().Name}: {exception.Message}");
                logEntry.AppendLine($"StackTrace: {exception.StackTrace}");
                if (exception.InnerException != null)
                {
                    logEntry.AppendLine($"Inner: {exception.InnerException.Message}");
                }
                logEntry.AppendLine("---");
            }

            var logLine = logEntry.ToString();

            lock (_fileLock)
            {
                try
                {
                    File.AppendAllText(_currentLogFile, logLine, Encoding.UTF8);
                }
                catch { /* 日志写入失败静默处理，避免循环 */ }
            }

#if DEBUG
            // 调试模式下同时输出到控制台
            System.Diagnostics.Debug.Write(logLine);
#endif
        }

        // ========== 公共日志方法 ==========

        /// <summary>
        /// 调试级别日志
        /// </summary>
        public void Debug(string message) => WriteLog("DEBUG", message);

        /// <summary>
        /// 信息级别日志
        /// </summary>
        public void Info(string message) => WriteLog("INFO", message);

        /// <summary>
        /// 警告级别日志
        /// </summary>
        public void Warning(string message, Exception ex = null) => WriteLog("WARNING", message, ex);

        /// <summary>
        /// 错误级别日志
        /// </summary>
        public void Error(string message, Exception ex = null) => WriteLog("ERROR", message, ex);

        /// <summary>
        /// 严重错误级别日志
        /// </summary>
        public void Fatal(string message, Exception ex = null) => WriteLog("FATAL", message, ex);

        // ========== 便捷方法 ==========

        /// <summary>
        /// 记录应用启动信息
        /// </summary>
        public void LogStartup(string version, string config, string osVersion)
        {
            Info("========================================");
            Info($"应用启动");
            Info($"版本: {version}");
            Info($"构建配置: {config}");
            Info($"操作系统: {osVersion}");
            Info($"日志级别: {_logLevel}");
            Info($"日志路径: {_logDirectory}");
            Info("========================================");
        }

        /// <summary>
        /// 记录用户操作
        /// </summary>
        public void LogUserAction(string action, string details = null)
        {
            var msg = $"[用户操作] {action}";
            if (!string.IsNullOrEmpty(details))
                msg += $" | {details}";
            Info(msg);
        }

        /// <summary>
        /// 记录课程相关操作
        /// </summary>
        public void LogLesson(string lessonId, string action, string result = null)
        {
            var msg = $"[课程] {lessonId} - {action}";
            if (!string.IsNullOrEmpty(result))
                msg += $" - {result}";
            Info(msg);
        }

        /// <summary>
        /// 记录代码检查
        /// </summary>
        public void LogCodeCheck(string lessonId, bool passed, int score, string approver = null)
        {
            var result = passed ? "通过" : "未通过";
            var msg = $"[检查] {lessonId} - {result} - 得分: {score}";
            if (!string.IsNullOrEmpty(approver))
                msg += $" - 导师: {approver}";
            Info(msg);
        }

        /// <summary>
        /// 获取当前日志信息（用于调试）
        /// </summary>
        public string GetLogPath() => _currentLogFile;
    }
}
