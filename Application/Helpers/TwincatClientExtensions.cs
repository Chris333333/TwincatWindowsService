using Application.DTOs;
using Domain;
using System;
using System.Collections.Generic;
using System.Text;
using TwinCAT.Ads;

namespace Application.Helpers
{
    public static class TwincatClientExtensions
    {
        /// <summary>
        /// Setting up the specific TcAdsClient to connect to specific netID and Port
        /// </summary>
        /// <param name="twinCatClient">Defined new TcAdsClient that needs connecting</param>
        /// <param name="netId">Net Id of PLC</param>
        /// <param name="port">Port of PLC project</param>
        /// <returns>ErrorLogDTO with information of is there was an error or if connection was establieshed</returns>
        public static ErrorLogDTO ConnectTwinCatClient(TcAdsClient twinCatClient, string netId, int port)
        {
            if (twinCatClient == null)
                return new ErrorLogDTO(true, "TwinCat client isn't declared");

            try
            {
                twinCatClient.Connect(netId, port);
            }
            catch (Exception ex)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("Error while connecting to Twincat. NetId:");
                sb.AppendLine(netId);
                sb.Append(" Port:");
                sb.AppendLine(port.ToString());
                sb.Append("Error: ");
                sb.Append(ex.ToString());
                return new ErrorLogDTO(true, sb.ToString());
            }
            return new ErrorLogDTO(false);
        }
        /// <summary>
        /// Setting Up Twincat Notifications
        /// </summary>
        /// <param name="LayoutDirectory">Dictionary of GVL values and types. With ability to hold optional value setup in string form</param>
        /// <param name="twincatClient"></param>
        /// <param name="twincatReadStream"></param>
        /// <returns></returns>
        public static ErrorLogDTO SetupTwinCatNotifications(Dictionary<string, TwincatLayoutsDictionaryValues> LayoutDirectory, TcAdsClient twincatClient, AdsStream twincatReadStream)
        {
            if (twincatClient == null)
                return new ErrorLogDTO(true, "TwinCat client isn't declared");

            if (!twincatClient.IsConnected)
                return new ErrorLogDTO(true, "TwinCat client isn't connected");

            if (twincatReadStream == null)
                return new ErrorLogDTO(true, "TwinCat AdsReadStream isn't declared");

            string notifName = null;

            try
            {
                foreach (KeyValuePair<string, TwincatLayoutsDictionaryValues> k in LayoutDirectory)
                {
                    notifName = k.Key;
                    twincatClient.AddDeviceNotification(k.Key, twincatReadStream, AdsTransMode.OnChange, 100, 0, k.Key);
                }
            }
            catch (Exception ex)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("Error while setting up notification ");
                sb.Append(notifName);
                sb.Append(" AmsAddress: ");
                sb.AppendLine(twincatClient.Address.ToString());
                sb.AppendLine(" Exception: ");
                sb.Append(ex.ToString());
                return new ErrorLogDTO(true, sb.ToString());
            }
            

            return new ErrorLogDTO(false);
        }
        public static ErrorLogWithValueToStringDTO TwincatReadstreamSwitchTypes(AdsNotificationEventArgs arg, AdsBinaryReader reader, Dictionary<string, TwincatLayoutsDictionaryValues> LayoutDirectory)
        {
            if (arg == null)
                return new ErrorLogWithValueToStringDTO(new ErrorLogDTO(true, "AdsNotificationsArguments are empty"));
            if (reader == null)
                return new ErrorLogWithValueToStringDTO(new ErrorLogDTO(true, "AdsBinary Reader isn't setup"));
            if (LayoutDirectory == null)
                return new ErrorLogWithValueToStringDTO(new ErrorLogDTO(true, "LayoutDirectory is empty"));

            if (!LayoutDirectory.ContainsKey(arg.UserData.ToString()))
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("Dictionary doesn't contains layout");
                sb.AppendLine(arg.UserData.ToString());
                return new ErrorLogWithValueToStringDTO(new ErrorLogDTO(true, sb.ToString()));
            }

            string value = null;

            try
            {
                switch (LayoutDirectory[arg.UserData.ToString()].TwincatDataType)
                {
                    case TwincatDataTypes.BOOL:
                        value = reader.ReadBoolean().ToString();
                        break;
                    case TwincatDataTypes.STRING:
                        if(LayoutDirectory[arg.UserData.ToString()].StringLength <= 0)
                            return new ErrorLogWithValueToStringDTO(new ErrorLogDTO(true, "There is no length declared for this string"));

                        value = new string( reader.ReadChars((LayoutDirectory[arg.UserData.ToString()].StringLength)));
                        break;
                    case TwincatDataTypes.INT:
                        value = reader.ReadInt16().ToString();
                        break;
                    case TwincatDataTypes.UDINT:
                        value = reader.ReadUInt32().ToString();
                        break;
                    default:
                        return new ErrorLogWithValueToStringDTO(new ErrorLogDTO(true, "Not supported Twincat Data Type"));
                }
            }
            catch (Exception ex)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("Error while reading string values of notif. Reader:");
                sb.AppendLine(reader.ToString());
                sb.Append(" Exception: ");
                sb.Append(ex.ToString());
                return new ErrorLogWithValueToStringDTO(new ErrorLogDTO(true, sb.ToString()));
            }

            LayoutDirectory[arg.UserData.ToString()].Value = value;

            return new ErrorLogWithValueToStringDTO(new ErrorLogDTO(false), value);
        }

    }
}