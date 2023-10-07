using Data.Helpers;
using Domain;
using Serilog;

namespace Data
{
    public class MySqlDataSender
    {
        private readonly ILogger _eventLog;

        public MySqlDataSender(ILogger eventLog)
        {
            _eventLog = eventLog;
        }

        public void MysqlUpdateOneLayoutInDatabase(string connstring, LayoutValueNotifDTO values, string databaseName)
        {
            var errorLog = MySqlExtension.UpdateOneLayoutInDatabase(connstring, values, databaseName);
            if (errorLog.IsError || errorLog.Message != null)
                _eventLog.Error(errorLog.Message);

            _eventLog.Information(errorLog.Message);
        }
    }
}
