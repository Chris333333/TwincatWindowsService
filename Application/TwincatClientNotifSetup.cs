using Application.Helpers;
using Domain;
using Serilog;
using System;
using System.Collections.Generic;

namespace Application
{
    public class TwincatClientNotifSetup : IDisposable
    {
        private readonly ILogger _eventLog;
        private TwincatConnector ExampleTwincatConnector;

        /// <summary>
        /// Constructor with Logger dependency injection
        /// </summary>
        /// <param name="eventLog"></param>
        public TwincatClientNotifSetup(ILogger eventLog)
        {
            _eventLog = eventLog;
        }
        /// <summary>
        /// Function running all twincats setups
        /// </summary>
        public void RunAll()
        {
            ExampleSetup();
        }

        public void Dispose()
        {
            if(ExampleTwincatConnector != null) ExampleTwincatConnector.Dispose();
        }

        private void ExampleSetup()
        {
            var LayoutDirectory = new Dictionary<string, TwincatLayoutsDictionaryValues>
            {
                {"Global_Variables.iErrorQty", new TwincatLayoutsDictionaryValues("Global_Variables.iErrorQty",TwincatDataTypes.INT) },
            };
            ExampleTwincatConnector = new TwincatConnector(Secrets.ExampleDataDumpTableName,LayoutDirectory, Secrets.ExampleAmsNetID, Secrets.ExamplePortNumber , _eventLog);
            ExampleTwincatConnector.PlcNotifSetup();
        }

    }
}
