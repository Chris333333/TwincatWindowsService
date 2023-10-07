using Domain;
using TwinCAT.Ads;
using Serilog;
using System.Collections.Generic;
using System;
using Data;

namespace Application.Helpers
{
    /// <summary>
    /// Advanced class witch is a TwinCat Client Connector. It sets up the entire class and sets up notfications based around Dictionary of Twincat GVL values.
    /// It also sends values to mysql database when ocurrs notification event (values in Twincat PLC changes)
    /// </summary>
    public class TwincatConnector :TwincatClient, IDisposable
    {
        /// <summary>
        /// AdsReader witch reads values when notification event occurs
        /// </summary>
        public AdsBinaryReader TwinCatClientAdsBinaryReader;

        private readonly string _mysqlDataBase;

        /// <summary>
        /// This connector is set up only to speak with mysql database automatyka
        /// </summary>
        public TwincatConnector(string mysqlDataBase, Dictionary<string, TwincatLayoutsDictionaryValues> LayoutDirectory, string netID, int port, ILogger eventLog) : base(eventLog, LayoutDirectory, netID, port) 
        {
            _mysqlDataBase = mysqlDataBase;
        }


        /// <summary>
        /// Event that happenes when value in PLC changes value. 
        /// Sends change as an update to mysql database. ConnectionString is defined strongly in this function
        /// </summary>
        /// <param name="sender">There is information who send this value</param>
        /// <param name="e">Arguments of the Ads Notification</param>
        protected override void PlcNotif_OnValueChanged(object sender, AdsNotificationEventArgs e)
        {
            TwinCatClientAdsBinaryReader = new AdsBinaryReader(e.DataStream);

            var valuesWithErrorLog = TwincatClientExtensions.TwincatReadstreamSwitchTypes(e, TwinCatClientAdsBinaryReader, _layoutDirectory);

            if (valuesWithErrorLog.ErrorLogDTO.IsError)
                _eventLog.Error(valuesWithErrorLog.ErrorLogDTO.Message);

            var mysqlSender = new MySqlDataSender(_eventLog);
            string connstring = Secrets.MySQLConnStringAutomatyka;

            mysqlSender.MysqlUpdateOneLayoutInDatabase(connstring, new LayoutValueNotifDTO(e.UserData.ToString(), valuesWithErrorLog.Value), _mysqlDataBase);
        }

        public new void Dispose()
        {
            if (TwinCatClient != null)
                TwinCatClient.Dispose();
            if (TwinCatClientReadStream != null)
                TwinCatClientReadStream.Dispose();
            if (TwinCatClientAdsBinaryReader != null)
                TwinCatClientAdsBinaryReader.Dispose();

        }
    }
}
