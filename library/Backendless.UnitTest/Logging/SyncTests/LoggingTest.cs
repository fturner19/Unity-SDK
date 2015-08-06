/*
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
using System.Linq;
using System.Text;
using BackendlessAPI.Exception;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using BackendlessAPI.Logging;

namespace BackendlessAPI.Test.Logging.SyncTests
{
  [TestClass]
  public class LoggingTest : TestsFrame
  {
    [TestMethod]
    public void TestWriteLog()
    {
      Backendless.Logging.SetLogReportingPolicy(9, 60);
      Logger logger = Backendless.Logging.GetLogger("com.mbaas.Logger");
      logger.trace("This is trace log.");
      logger.debug("This is debug log.");
      logger.info("This is info log.");
      logger.warn("This is warn log.");
      logger.error("This is error log.");
      logger.fatal("This is fatal log.");
      
      try
      {
        throw new System.Exception("This is exception message.");
      }
      catch (System.Exception e)
      {
        logger.warn("This is warn log.", e);
        logger.error("This is error log.", e);
        logger.fatal("This is fatal log.", e);
      }
    }

    [TestMethod]
    public void TestWriteSingleLog()
    {
      Backendless.Logging.SetLogReportingPolicy(1, 60);
      Logger logger = Backendless.Logging.GetLogger("com.mbaas.Logger");
      logger.trace("This is trace log.");
    }
  }
}