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
using Backendless.Test;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BackendlessAPI.Test.Counters.AsyncTests
{
  [TestClass]
  public class TestsFrame : IAsyncTest
  {
    public const string DEFAULT_COUNTER_NAME = "Default";
    public const int DEFAULT_ADD_VALUE = 10;

    [TestInitialize]
    public void SetUp()
    {
      Backendless.InitApp( Defaults.TEST_APP_ID, Defaults.TEST_SECRET_KEY, Defaults.TEST_VERSION );
    }
  }
}