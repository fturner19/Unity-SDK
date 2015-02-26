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

namespace BackendlessAPI.Test.Caching.AsyncTests
{
  [TestClass]
  public class CachingTest : TestsFrame
  {
    [TestMethod]
    public void TestGetPointsByMetadata()
    {
      RunAndAwait( () =>
        {
          CacheClass cacheClass = new CacheClass();
          cacheClass.Age = 19;
          cacheClass.Name = "Kang";
          Backendless.Cache.Put(DEFAULT_CACHE_KEY, cacheClass, new AsyncCallback<object>(
            r =>
            {
              Backendless.Cache.Contains(DEFAULT_CACHE_KEY, new AsyncCallback<Boolean>(
                r2 =>
                {
                  Assert.AreEqual(r2, true, "Server returned a count with wrong value");
                  Backendless.Cache.Get<CacheClass>(DEFAULT_CACHE_KEY, new AsyncCallback<CacheClass>(
                    r3 =>
                    {
                      Assert.AreEqual(cacheClass.Age, r3.Age, "Server returned a count with wrong value");
                      Assert.AreEqual(cacheClass.Name, r3.Name, "Server returned a count with wrong value");
                      Backendless.Cache.Delete(DEFAULT_CACHE_KEY, new AsyncCallback<object>(
                        r4 =>
                        {
                          Backendless.Cache.Contains(DEFAULT_CACHE_KEY, new AsyncCallback<Boolean>(
                          r5 =>
                          {
                            Assert.AreEqual(r5, false, "Server returned a count with wrong value");
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
        } );
    }
  }
}