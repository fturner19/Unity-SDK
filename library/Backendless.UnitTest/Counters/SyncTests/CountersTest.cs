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
using BackendlessAPI.Geo;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BackendlessAPI.Test.Counters.SyncTests
{
  [TestClass]
  public class CountersTest : TestsFrame
  {
    [TestMethod]
    public void TestGetAndIncrementGetAndDecrement()
    {
      int value = 0;
      Backendless.Counters.Reset(DEFAULT_COUNTER_NAME);
      value = Backendless.Counters.GetAndIncrement(DEFAULT_COUNTER_NAME);
      Assert.AreEqual(value + 1, Backendless.Counters.Get(DEFAULT_COUNTER_NAME), "Server returned a count with wrong value");
      value = Backendless.Counters.GetAndDecrement(DEFAULT_COUNTER_NAME);
      Assert.AreEqual(value - 1, Backendless.Counters.Get(DEFAULT_COUNTER_NAME), "Server returned a count with wrong value");
    }

    [TestMethod]
    public void TestIncrementAndGetDecrementAndGet()
    {
      int value = 0;
      Backendless.Counters.Reset(DEFAULT_COUNTER_NAME);
      value = Backendless.Counters.IncrementAndGet(DEFAULT_COUNTER_NAME);
      Assert.AreEqual(value, Backendless.Counters.Get(DEFAULT_COUNTER_NAME), "Server returned a count with wrong value");
      value = Backendless.Counters.DecrementAndGet(DEFAULT_COUNTER_NAME);
      Assert.AreEqual(value, Backendless.Counters.Get(DEFAULT_COUNTER_NAME), "Server returned a count with wrong value");
    }

    [TestMethod]
    public void TestGetAndAddAddAndGet()
    {
      int value = 0;
      Backendless.Counters.Reset(DEFAULT_COUNTER_NAME);
      value = Backendless.Counters.GetAndAdd(DEFAULT_COUNTER_NAME, DEFAULT_ADD_VALUE);
      Assert.AreEqual(value, 0, "Server returned a count with wrong value");
      value = Backendless.Counters.AddAndGet(DEFAULT_COUNTER_NAME, DEFAULT_ADD_VALUE);
      Assert.AreEqual(value, DEFAULT_ADD_VALUE*2, "Server returned a count with wrong value");
    }

    [TestMethod]
    public void TestCompareAndSet()
    {
      int value = 0;
      Backendless.Counters.Reset(DEFAULT_COUNTER_NAME);
      value = Backendless.Counters.AddAndGet(DEFAULT_COUNTER_NAME, DEFAULT_ADD_VALUE);
      bool result = Backendless.Counters.CompareAndSet(DEFAULT_COUNTER_NAME, DEFAULT_ADD_VALUE, DEFAULT_ADD_VALUE*2);
      Assert.AreEqual(result, true, "Server returned a count with wrong value");
      Assert.AreEqual(DEFAULT_ADD_VALUE * 2, Backendless.Counters.Get(DEFAULT_COUNTER_NAME), "Server returned a count with wrong value");
    }
  }
}