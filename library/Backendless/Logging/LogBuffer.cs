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
using System.Threading;
using System.Collections.Generic;

namespace BackendlessAPI.Logging
{
  internal class LogBuffer
  {
    private int numOfMessages;
    private int timeFrequency;
    private Dictionary<String, Dictionary<String, LinkedList<LogMessage>>> logBatches;
    private int messageCount;
    private Mutex mutex;
    private Timer timer;
    private TimerCallback timerCallback;

    internal LogBuffer()
    {
      mutex = new Mutex( false, "LogBufferMutex" );
      numOfMessages = 100;
      timeFrequency = 1000 * 60 * 5; // 5 minutes
      logBatches = new Dictionary<string,Dictionary<string,LinkedList<LogMessage>>>();
      messageCount = 0;
      setupTimer();
    }

    private void setupTimer()
    {
      if( timer != null )
      {
        timer.Dispose();
        timer = null;
      }

      if( timerCallback == null )
        timerCallback = new TimerCallback( FlushMessages );

      if( numOfMessages > 1 )
        timer = new Timer( timerCallback, null, 0, timeFrequency ); 
    }

    public void FlushMessages( Object stateInfo )
    {
      mutex.WaitOne();
      Flush();
      mutex.ReleaseMutex();
    }

    public void SetLogReportingPolicy( int numOfMessages, int timeFrequency )
    {
      if( numOfMessages > 1 && timeFrequency <= 0 )
       throw new System.Exception( "the time frequency argument must be greater than zero" );

      this.numOfMessages = numOfMessages;
      this.timeFrequency = timeFrequency;
      setupTimer();
    }

    internal void Enqueue( String logger, String logLevel, String message, System.Exception error )
    {
      if( numOfMessages == 1 )
      {
        Backendless.Logging.ReportSingleLogMessage( logger, logLevel, message, error );
      }
      else
      {
        Dictionary<String, LinkedList<LogMessage>> logLevels;
        LinkedList<LogMessage> messages;

        mutex.WaitOne();

        if( logBatches.ContainsKey( logger ) )
        {
          logLevels = logBatches[ logger ];
        }
        else
        {
          logLevels = new Dictionary<string,LinkedList<LogMessage>>();
          logBatches[ logger ] = logLevels;
        }

        if( logLevels.ContainsKey( logLevel ) )
        {
          messages = logLevels[ logLevel ];
        }
        else
        {
          messages = new LinkedList<LogMessage>();
          logLevels[ logLevel ] = messages;
        }

        messages.AddLast( new LogMessage( DateTime.Now, message, (error == null) ? null : error.StackTrace ) );
        messageCount++;

        if( messageCount == numOfMessages )
          Flush();

        mutex.ReleaseMutex();
      }
    }

    private void Flush()
    {
      LinkedList<LogBatch> allMessages = new LinkedList<LogBatch>();

      foreach( String logger in logBatches.Keys )
      {
        Dictionary<String, LinkedList<LogMessage>> logLevels = logBatches[ logger ];

        foreach( String logLevel in logLevels.Keys )
        {
          LogBatch logBatch = new LogBatch();
          logBatch.logger = logger;
          logBatch.logLevel = logLevel;
          logBatch.messages = logLevels[ logLevel ];
          allMessages.AddLast( logBatch );
        }
      }

      logBatches.Clear();

      LogBatch[] allMessagesArray = new LogBatch[ allMessages.Count ];
      allMessages.CopyTo( allMessagesArray, 0 );
      Backendless.Logging.ReportBatch( allMessagesArray );
    }
  }
}
