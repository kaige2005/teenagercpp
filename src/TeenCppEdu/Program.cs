using System;
using System.Windows.Forms;
using TeenCppEdu.Services.Logger;
using TeenCppEdu.UI.Forms;

namespace TeenCppEdu
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // 初始化日志服务（首次访问会触发初始化）
            var logger = LoggerService.Instance;
            logger.LogStartup(
                GetVersion(),
#if DEBUG
                "DEBUG",
#else
                "RELEASE",
#endif
                Environment.OSVersion.ToString()
            );

            // 设置应用程序错误处理
            Application.ThreadException += (sender, e) =>
            {
                logger.Error("UI线程未处理异常", e.Exception);
                MessageBox.Show(
                    $"抱歉，程序遇到了一个问题：\n\n{e.Exception.Message}\n\n" +
                    $"日志文件：{logger.GetLogPath()}\n\n" +
                    "请重启程序再试，如果问题持续存在请联系管理员。",
                    "程序错误",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            };

            AppDomain.CurrentDomain.UnhandledException += (sender, e) =>
            {
                var ex = e.ExceptionObject as Exception;
                logger.Fatal("应用程序域未处理异常", ex);
            };

            try
            {
                logger.Info("启动主界面");
                Application.Run(new MainForm());
                logger.Info("应用程序正常退出");
            }
            catch (Exception ex)
            {
                logger.Fatal("应用程序运行异常", ex);
                MessageBox.Show(
                    $"程序遇到严重错误：\n{ex.Message}\n\n日志位置：{logger.GetLogPath()}",
                    "启动失败",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 获取应用程序版本
        /// </summary>
        private static string GetVersion()
        {
            return typeof(Program).Assembly.GetName().Version?.ToString() ?? "1.0.0";
        }
    }
}
