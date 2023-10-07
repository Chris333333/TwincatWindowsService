using System.ServiceProcess;
using System.Diagnostics;
using System.Timers;
using Serilog;
using Application;
using System.IO;

namespace TwincatWindowsService
{
    public partial class TwincatService : ServiceBase
    {
        private int eventId = 1;
        private TwincatClientNotifSetup TwinCatNotifClient;
        public TwincatService()
        {
            InitializeComponent();
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.File(Path.Combine("C:\\","Logs","TwincatService","Logfile-.txt"),rollingInterval:RollingInterval.Day,outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
                .CreateLogger();
            Log.Information("Logging Setup finished");
            
        }

        protected override void OnStart(string[] args)
        {
            Log.Information("TwincatService started");
            Timer timer = new Timer();
            timer.Interval = 60000*30;
            timer.Elapsed += new ElapsedEventHandler(this.OnTimer);
            timer.Start();

            TwinCatNotifClient = new TwincatClientNotifSetup(Log.Logger);
            TwinCatNotifClient.RunAll();
        }

        protected override void OnStop()
        {
            if(TwinCatNotifClient != null) TwinCatNotifClient.Dispose();
            Log.Information("TwincatService stoped");
            Log.CloseAndFlush();
        }

        public void OnTimer(object sender, ElapsedEventArgs args)
        {
            Log.Information("TwincatService monitoring the system  ", EventLogEntryType.Information, eventId++);
        }

        
    }
}
