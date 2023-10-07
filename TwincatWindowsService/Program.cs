using System.ServiceProcess;

namespace TwincatWindowsService
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                new TwincatService()
            };
            ServiceBase.Run(ServicesToRun);
        }
    }
}
