/*
Copyright 2015 Backendless Corp. All Rights Reserved.
Copyright 2015 Acrodea, Inc. All Rights Reserved.

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/

using System;
using System.Collections.Generic;
using BackendlessAPI.Engine;
using BackendlessAPI.LitJson;

namespace BackendlessAPI.Logging
{
  public class LoggingService
  {
    private LogBuffer buffer;
    private Dictionary<String, Logger> loggers;

    public LoggingService()
    {
      buffer = new LogBuffer();
      loggers = new Dictionary<string, Logger>();
    }

    public void SetLogReportingPolicy( int numOfMessages, int timeFrequencyMS )
    {
      buffer.SetLogReportingPolicy( numOfMessages, timeFrequencyMS );
    }

    public Logger GetLogger( Type loggerType )
    {
      return GetLogger( loggerType.Name );
    }

    public Logger GetLogger( String loggerName )
    {
      if( loggers.ContainsKey( loggerName ) )
        return loggers[ loggerName ];

      Logger logger = new Logger( loggerName );
      loggers[ loggerName ] = logger;
      return logger;
    }

    internal LogBuffer Buffer
    {
      get
      { 
       return buffer; 
      }
    }

    private class RequestLog
    {
      [JsonProperty("log-level")]
      public string LogLevel { get; set; }
      [JsonProperty("logger")]
      public string Logger { get; set; }
      [JsonProperty("timestamp")]
      public DateTime Timestamp { get; set; }
      [JsonProperty("message")]
      public string Message { get; set; }
      [JsonProperty("exception")]
      public string Exception { get; set; }
    }

    internal void ReportSingleLogMessage( String logger, String loglevel, String message, System.Exception error )
    {
      List<RequestLog> logs = new List<RequestLog>();
      RequestLog log = new RequestLog();
      log.LogLevel = loglevel;
      log.Logger = logger;
      log.Timestamp = DateTime.Now;
      log.Message = message;
      log.Exception = (error == null) ? null : error.StackTrace;
      logs.Add(log);

      Invoker.InvokeSync<object>(Invoker.Api.LOGGERSERVICE_PUT, new object[] { logs });
    }

    internal void ReportBatch( LogBatch[] logBatchs )
    {
      if (logBatchs.Length == 0)
        return;

      List<RequestLog> logs = new List<RequestLog>();
      foreach (LogBatch logBatch in logBatchs)
      {
        foreach (LogMessage message in logBatch.messages)
        {
          RequestLog log = new RequestLog();
          log.LogLevel = logBatch.logLevel;
          log.Logger = logBatch.logger;
          log.Timestamp = message.timestamp;
          log.Message = message.message;
          log.Exception = message.exception;
          logs.Add(log);
        }
      }

      Invoker.InvokeSync<object>(Invoker.Api.LOGGERSERVICE_PUT, new object[] { logs });
    }
  }
}
