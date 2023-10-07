using Domain;
using MySql.Data.MySqlClient;
using Serilog;
using System;
using System.Collections.Generic;
using System.Text;

namespace Data.Helpers
{
    public static class MySqlExtension
    {
        public static ErrorLogDTO UpdateOneLayoutInDatabase(string connstring, LayoutValueNotifDTO values, string tableName)
        {
            if (values == null)
                return new ErrorLogDTO(true, "There are no values to set");

            if(string.IsNullOrEmpty(connstring))
                return new ErrorLogDTO(true, "Connection string not set");

            if(string.IsNullOrEmpty(values.LayoutName))
                return new ErrorLogDTO(true, "LayoutName is empty");

            MySqlConnection conn = null;

            try
            {
                conn = new MySqlConnection(connstring);
                conn.Open();
                MySqlCommand comm = conn.CreateCommand();
                StringBuilder sb = new StringBuilder();
                sb.Append("UPDATE ");
                sb.Append(tableName);
                sb.Append(" SET Value=@Value WHERE LayoutName=@LayoutName");
                comm.CommandText = sb.ToString();
                comm.Parameters.AddWithValue("@LayoutName", values.LayoutName);
                comm.Parameters.AddWithValue("@Value", values.ChangedValueNotif);
                comm.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("Error while updating data from service to MySQL in table ");
                sb.Append(tableName);
                sb.Append(". Layout: ");
                sb.Append(values.LayoutName);
                sb.Append(" Value: ");
                sb.AppendLine(values.ChangedValueNotif.ToString());
                sb.Append(" Exception: ");
                sb.Append(ex.Message.ToString());
                return new ErrorLogDTO(true, sb.ToString());
            }
            finally
            {
                if (conn != null)
                {
                    conn.Close();
                    conn.Dispose();
                }
            }

            return new ErrorLogDTO(false);
        }

        [Obsolete("Seting Up empty database")]
        public static void SettingUpDataBase(string connstring, Dictionary<string, TwincatLayoutsDictionaryValues> values, ILogger _eventLog, string tableName)
        {

            MySqlConnection conn = null;

            try
            {
                conn = new MySqlConnection(connstring);
                conn.Open();

                foreach (KeyValuePair<string, TwincatLayoutsDictionaryValues> pairs in values)
                {
                    MySqlCommand comm = conn.CreateCommand();
                    StringBuilder sb = new StringBuilder();
                    sb.Append("INSERT INTO ");
                    sb.Append(tableName);
                    sb.Append("(LayoutName,MainLayoutName,Value,TwincatDataType) VALUES(?LayoutName,?MainLayoutName,?Value, ?TwincatDataType) ");
                    comm.CommandText = sb.ToString();
                    comm.Parameters.Add("?LayoutName", MySqlDbType.VarString).Value = pairs.Key;
                    comm.Parameters.Add("?MainLayoutName", MySqlDbType.VarString).Value = pairs.Value.MainLayoutName;
                    comm.Parameters.Add("?Value", MySqlDbType.VarString).Value = pairs.Value.Value;
                    comm.Parameters.Add("?TwincatDataType", MySqlDbType.Int32).Value = (int)pairs.Value.TwincatDataType + 1;
                    comm.ExecuteNonQuery();
                }

            }
            catch (Exception e)
            {
                _eventLog.Error(e.ToString());
            }
            finally
            {
                if (conn != null)
                {
                    conn.Close();
                }
            }
        }
    }
}
