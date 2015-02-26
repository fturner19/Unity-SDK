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
using System.Threading;
using BackendlessAPI.Async;
using BackendlessAPI.Data;
using BackendlessAPI.Exception;
using BackendlessAPI.Geo;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BackendlessAPI.Test.Counters.AsyncTests
{
  [TestClass]
  public class CountersTest : TestsFrame
  {
    [TestMethod]
    public void TestGetAndIncrementGetAndDecrement()
    {
      RunAndAwait(() =>
        {
          Backendless.Counters.Reset(DEFAULT_COUNTER_NAME, new AsyncCallback<Object>(
            r =>
            {
              Backendless.Counters.GetAndIncrement(DEFAULT_COUNTER_NAME, new AsyncCallback<int>(
            r2 =>
            {
              Backendless.Counters.Get(DEFAULT_COUNTER_NAME, new AsyncCallback<int>(
            r3 =>
            {
              Assert.AreEqual(r3, r2 + 1, "Server returned a count with wrong value");

              Backendless.Counters.GetAndDecrement(DEFAULT_COUNTER_NAME, new AsyncCallback<int>(
            r4 =>
            {
              Backendless.Counters.Get(DEFAULT_COUNTER_NAME, new AsyncCallback<int>(
            r5 =>
            {
              Assert.AreEqual(r5, r4 - 1, "Server returned a count with wrong value");

              CountDown();
            },
            f5 => FailCountDownWith(f5)));
            },
            f4 => FailCountDownWith(f4)));
            },
            f3 => FailCountDownWith(f3)));
            },
            f2 => FailCountDownWith(f2)));
            },
            f => FailCountDownWith(f)));
        });
    }

    [TestMethod]
    public void TestIncrementAndGetDecrementAndGet()
    {
      RunAndAwait(() =>
      {
        Backendless.Counters.Reset(DEFAULT_COUNTER_NAME, new AsyncCallback<Object>(
          r =>
          {
            Backendless.Counters.IncrementAndGet(DEFAULT_COUNTER_NAME, new AsyncCallback<int>(
          r2 =>
          {
            Backendless.Counters.Get(DEFAULT_COUNTER_NAME, new AsyncCallback<int>(
          r3 =>
          {
            Assert.AreEqual(r3, r2, "Server returned a count with wrong value");

            Backendless.Counters.DecrementAndGet(DEFAULT_COUNTER_NAME, new AsyncCallback<int>(
          r4 =>
          {
            Backendless.Counters.Get(DEFAULT_COUNTER_NAME, new AsyncCallback<int>(
          r5 =>
          {
            Assert.AreEqual(r5, r4, "Server returned a count with wrong value");

            CountDown();
          },
            f5 => FailCountDownWith(f5)));
          },
            f4 => FailCountDownWith(f4)));
          },
            f3 => FailCountDownWith(f3)));
          },
            f2 => FailCountDownWith(f2)));
          },
            f => FailCountDownWith(f)));
      });
    }

    [TestMethod]
    public void TestGetAndAddAddAndGet()
    {
      RunAndAwait(() =>
      {
        Backendless.Counters.Reset(DEFAULT_COUNTER_NAME, new AsyncCallback<Object>(
          r =>
          {
            Backendless.Counters.GetAndAdd(DEFAULT_COUNTER_NAME, DEFAULT_ADD_VALUE, new AsyncCallback<int>(
          r2 =>
          {
            Assert.AreEqual(r2, 0, "Server returned a count with wrong value");

            Backendless.Counters.AddAndGet(DEFAULT_COUNTER_NAME, DEFAULT_ADD_VALUE, new AsyncCallback<int>(
          r3 =>
          {
            Assert.AreEqual(r3, DEFAULT_ADD_VALUE * 2, "Server returned a count with wrong value");

            CountDown();
          },
            f3 => FailCountDownWith(f3)));
          },
            f2 => FailCountDownWith(f2)));
          },
            f => FailCountDownWith(f)));
      });
    }

    [TestMethod]
    public void TestCompareAndSet()
    {
      RunAndAwait(() =>
      {
        Backendless.Counters.Reset(DEFAULT_COUNTER_NAME, new AsyncCallback<Object>(
          r =>
          {
            Backendless.Counters.AddAndGet(DEFAULT_COUNTER_NAME, DEFAULT_ADD_VALUE, new AsyncCallback<int>(
          r2 =>
          {
            Backendless.Counters.CompareAndSet(DEFAULT_COUNTER_NAME, DEFAULT_ADD_VALUE, DEFAULT_ADD_VALUE * 2, new AsyncCallback<bool>(
          r3 =>
          {
            Assert.AreEqual(r3, true, "Server returned a count with wrong value");
            Assert.AreEqual(DEFAULT_ADD_VALUE * 2, Backendless.Counters.Get(DEFAULT_COUNTER_NAME), "Server returned a count with wrong value");

            CountDown();
          },
            f3 => FailCountDownWith(f3)));
          },
            f2 => FailCountDownWith(f2)));
          },
            f => FailCountDownWith(f)));
      });
    }
  }
}