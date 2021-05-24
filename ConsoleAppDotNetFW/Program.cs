using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLog;

namespace ConsoleAppDotNetFW
{
    class Program
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        static void Main(string[] args)
        {
            try
            {
                Logger.Info("Hello world Client");
                System.Console.ReadKey();
            }
            catch (Exception ex)
            {
                Logger.Debug(ex, "Goodbye cruel world");
            }

            NLog.LogManager.Shutdown();
           
        }
    }
}

namespace Service
{
    class testService
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        static void service_tester(string[] args)
        {
            try
            {
                Logger.Info("Hello world Service");
                System.Console.ReadKey();
            }
            catch (Exception ex)
            {
                Logger.Debug(ex, "Goodbye cruel world");
            }

            NLog.LogManager.Shutdown();
            Console.WriteLine("Service Execute");
        }
    }
}