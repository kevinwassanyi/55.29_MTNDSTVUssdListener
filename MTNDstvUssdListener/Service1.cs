using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MTNDstvUssdListener
{
    public partial class Service1 : ServiceBase
    {
        private Thread dstvUssdListener;
        private CancellationTokenSource cancellationTokenSource;
        public Service1()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            cancellationTokenSource = new CancellationTokenSource();
            dstvUssdListener = new Thread(() => ProcessUssdRequests(cancellationTokenSource.Token));
            dstvUssdListener.Start();
        }

        protected override void OnStop()
        {
            cancellationTokenSource.Cancel(); // Request cancellation
            dstvUssdListener.Join(); // Wait for thread to finish gracefully
        }

        public void ProcessUssdRequests(CancellationToken token)
        {
            try
            {
                TCPServer tCPServer = new TCPServer();
                while (!token.IsCancellationRequested)
                {
                    tCPServer.ListenAndProcess();
                }
            }
            catch (Exception ex)
            {
                LogExceptionToFile(ex);
            }
        }
        private void LogExceptionToFile(Exception ex)
        {
            try
            {
                string logDirectory = "E:\\Logs";
                Directory.CreateDirectory(logDirectory); // Ensure directory exists

                string logFileName = $"DstvUssdListenerServiceExceptions{DateTime.Now:yyyyMMdd}.txt";
                string logFilePath = Path.Combine(logDirectory, logFileName);

                string logMessage = $"{DateTime.Now}: {ex.Message}\n{ex.StackTrace}\n-------------------------\n";
                File.AppendAllText(logFilePath, logMessage);
            }
            catch
            {
                // Ignore logging failures
            }
        }
    }
}
