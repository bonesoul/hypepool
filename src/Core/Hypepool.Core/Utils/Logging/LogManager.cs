#region license
// 
//      hypepool
//      https://github.com/bonesoul/hypepool
// 
//      Copyright (c) 2013 - 2018 Hüseyin Uslu
// 
//      Permission is hereby granted, free of charge, to any person obtaining a copy
//      of this software and associated documentation files (the "Software"), to deal
//      in the Software without restriction, including without limitation the rights
//      to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//      copies of the Software, and to permit persons to whom the Software is
//      furnished to do so, subject to the following conditions:
// 
//      The above copyright notice and this permission notice shall be included in all
//      copies or substantial portions of the Software.
// 
//      THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//      IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//      FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//      AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//      LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//      OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
//      SOFTWARE.
#endregion

using System.Linq;
using Serilog;
using Serilog.Core;
using Serilog.Events;

namespace Hypepool.Core.Utils.Logging
{
    public class LogManager : ILogManager
    {
        private const string ConsoleLogTemplate = "{Timestamp:HH:mm:ss} [{Level:u3}] [{Pool:l}] [{Source:l}] {Message}{NewLine}{Exception}";

        public LogManager()
        {
            Log.Logger = new LoggerConfiguration()
                .Enrich.WithThreadId()
                .Enrich.With(new SourceEnricher()) // used for enriching logs with sources.
                .Enrich.With(new ComponentEnricher()) // used for enriching logs with compontents.
                .MinimumLevel.Verbose() // lower the default minimum level to verbose as sinks can only rise them but not lower.
                .WriteTo.ColoredConsole(LogEventLevel.Verbose, ConsoleLogTemplate)
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
