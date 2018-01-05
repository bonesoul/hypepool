using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Serilog;
using Serilog.Core;
using Serilog.Events;

namespace Hypepool.Core.Internals.Logging
{
    public class LogManager : ILogManager
    {
        private const string ConsoleLogFormat = "{Timestamp:HH:mm:ss} [{Level}] [{Source:l}] [{Pool:l}] {Message}{NewLine}{Exception}";

        public LogManager()
        {
            Log.Logger = new LoggerConfiguration()
                .Enrich.WithThreadId()
                .Enrich.With(new SourceEnricher()) // used for enriching logs with sources.
                .Enrich.With(new ComponentEnricher()) // used for enriching logs with compontents.
                .MinimumLevel.Verbose() // lower the default minimum level to verbose as sinks can only rise them but not lower.
                .WriteTo.ColoredConsole(LogEventLevel.Verbose, ConsoleLogFormat)
                .CreateLogger();
        }

        public class SourceEnricher : ILogEventEnricher
        {
            public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
            {
                logEvent.AddPropertyIfAbsent(logEvent.Properties.Keys.Contains("SourceContext")
                    ? propertyFactory.CreateProperty("Source",
                        logEvent.Properties["SourceContext"].ToString().Replace("\"", "").Split('.').Last())
                    : propertyFactory.CreateProperty("Source", "n/a"));
            }
        }

        public class ComponentEnricher : ILogEventEnricher
        {
            public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
            {
                logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("Pool", "-"));
            }
        }
    }
}
