using Domain;
using Serilog;
using System;
using System.Collections.Generic;
using System.Text;
using System.Timers;
using TwinCAT.Ads;

namespace Application.Helpers
{
    abstract public class TwincatClient : IDisposable
    {
        /// <summary>
        /// Just a Serilog logger :)
        /// </summary>
        protected ILogger _eventLog;
        /// <summary>
        /// Main TwincatClient
        /// </summary>
        protected TcAdsClient TwinCatClient;
        /// <summary>
        /// AdsStream for collecting data
        /// </summary>
        protected AdsStream TwinCatClientReadStream;
        /// <summary>
        /// Dictionary of GVL values and types. With ability to hold optional value setup in string form
        /// </summary>
        protected Dictionary<string, TwincatLayoutsDictionaryValues> _layoutDirectory;
        /// <summary>
        /// Timer Monitoring the state of PLC and connection via TcClient
        /// </summary>
        private Timer MonitoringTimer;
        /// <summary>
        /// NetID of PLC
        /// </summary>
        protected readonly string _netID;
        /// <summary>
        /// Port of PLC
        /// </summary>
        protected readonly int _port;
        /// <summary>
        /// Just some debuging numbers for the log
        /// </summary>
        private int ConnectionErrorsCount = 0;
        /// <summary>
        /// This is a flag that tries to bypass problems with TcClient "losing" notifications after PLC restart or connection errors
        /// </summary>
        private bool SuspectedConnectionBreak;

        protected TwincatClient(ILogger eventLog, Dictionary<string, TwincatLayoutsDictionaryValues> layoutDirectory, string netID, int port)
        {
            _eventLog = eventLog;
            _layoutDirectory = layoutDirectory;
            _netID = netID;
            _port = port;
        }



        /// <summary>
        /// Seting Up entire logic of TwinCat client and seting up event
        /// </summary>
        public void PlcNotifSetup()
        {

            MonitoringTimer = new Timer(3000);
            MonitoringTimer.Elapsed += MonitoringTimer_Elapsed;
            MonitoringTimer.AutoReset = true;
            MonitoringTimer.Enabled = false;

            MonitoringTimer.Start();

            ClientSetup();

        }

        private void ClientSetup()
        {
            TwinCatClient = new TcAdsClient();


            ErrorLogDTO errorLog = new ErrorLogDTO(false);
            TwinCatClientReadStream = new AdsStream(150);

            errorLog = TwincatClientExtensions.ConnectTwinCatClient(TwinCatClient, _netID, _port);

            if (errorLog.IsError)
            {
                _eventLog.Error(errorLog.Message);
            }
            else
            {
                NotificationSetup();
            }
        }

        private void NotificationSetup()
        {
            TwinCatClient.AdsNotification += PlcNotif_OnValueChanged;

            var errorLog = TwincatClientExtensions.SetupTwinCatNotifications(_layoutDirectory, TwinCatClient, TwinCatClientReadStream);
            if(errorLog.IsError)
            {
                _eventLog.Error(errorLog.Message);
            }

        }


        private void MonitoringTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            AdsState adsTwincatState = AdsState.Invalid;

            if (TwinCatClient.IsConnected)
            {
                try
                {
                    adsTwincatState = TwinCatClient.ReadState().AdsState;
                }
                catch 
                {
                    ConnectionErrorsCount++;
                    SuspectedConnectionBreak = true;
                }

                if (adsTwincatState != AdsState.Run)
                {
                    ConnectionErrorsCount++;
                    SuspectedConnectionBreak = true;

                }

                if (SuspectedConnectionBreak && (adsTwincatState == AdsState.Run))
                {
                    try
                    {
                        TwinCatClient.Dispose();
                        TwinCatClientReadStream.Dispose();

                        SuspectedConnectionBreak = false;
                        ClientSetup();
                        StringBuilder sb = new StringBuilder();
                        sb.Append("Restarted after suspected connection break.");
                        sb.Append(" Error values: ");
                        sb.Append(ConnectionErrorsCount.ToString());
                        _eventLog.Information(sb.ToString());
                        ConnectionErrorsCount = 0;
                    }
                    catch (Exception ex)
                    {
                        StringBuilder sb = new StringBuilder();
                        sb.AppendLine("Error while restarting notification after suspected connection break.");
                        sb.AppendLine(" Exception: ");
                        sb.Append(ex.ToString());
                        _eventLog.Information(sb.ToString());
                    }
                }

            }
            else
            {
                SuspectedConnectionBreak = true;
            }
        }

        protected abstract void PlcNotif_OnValueChanged(object sender, AdsNotificationEventArgs e);

        public void Dispose()
        {
            if (TwinCatClient != null)
            {
                try { TwinCatClient.AdsNotification -= PlcNotif_OnValueChanged; } catch { }
                try { TwinCatClient.Dispose(); } catch { }

            }
            if (TwinCatClientReadStream != null) { try { TwinCatClientReadStream.Dispose(); } catch { } };
            if (_layoutDirectory != null) { try { _layoutDirectory.Clear(); } catch { } };
        }
    }
}
