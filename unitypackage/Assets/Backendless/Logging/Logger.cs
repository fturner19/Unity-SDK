﻿/*
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

namespace BackendlessAPI.Logging
{
  public class Logger
  {
    private String loggerName;

    internal Logger( String loggerName )
    {
      this.loggerName = loggerName;
    }

    public void trace(String message)
    {
      Backendless.Logging.Buffer.Enqueue(loggerName, "TRACE", message, null);
    }

    public void debug(String message)
    {
      Backendless.Logging.Buffer.Enqueue(loggerName, "DEBUG", message, null);
    }

    public void info(String message)
    {
      Backendless.Logging.Buffer.Enqueue(loggerName, "INFO", message, null);
    }

    public void warn(String message)
    {
      Backendless.Logging.Buffer.Enqueue(loggerName, "WARN", message, null);
    }

    public void warn(String message, System.Exception e)
    {
      Backendless.Logging.Buffer.Enqueue(loggerName, "WARN", message, e);
    }

    public void error(String message)
    {
      Backendless.Logging.Buffer.Enqueue(loggerName, "ERROR", message, null);
    }

    public void error(String message, System.Exception e)
    {
      Backendless.Logging.Buffer.Enqueue(loggerName, "ERROR", message, e);
    }

    public void fatal(String message)
    {
      Backendless.Logging.Buffer.Enqueue(loggerName, "FATAL", message, null);
    }

    public void fatal(String message, System.Exception e)
    {
      Backendless.Logging.Buffer.Enqueue(loggerName, "FATAL", message, e);
    }
  }
}
