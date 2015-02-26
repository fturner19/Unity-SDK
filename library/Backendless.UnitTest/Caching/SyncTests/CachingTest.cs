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

namespace BackendlessAPI.Test.Caching.SyncTests
{
  [TestClass]
  public class CachingTest : TestsFrame
  {
    [TestMethod]
    public void TestGetAndIncrementGetAndDecrement()
    {
      CacheClass cacheClass = new CacheClass();
      cacheClass.Age = 19;
      cacheClass.Name = "Kang";
      Backendless.Cache.Put(DEFAULT_CACHE_KEY, cacheClass);
      Boolean isExist = Backendless.Cache.Contains(DEFAULT_CACHE_KEY);
      Assert.AreEqual(isExist, true, "Server returned a count with wrong value");
      CacheClass getCacheClass = Backendless.Cache.Get<CacheClass>(DEFAULT_CACHE_KEY);
      Assert.AreEqual(cacheClass.Age, getCacheClass.Age, "Server returned a count with wrong value");
      Assert.AreEqual(cacheClass.Name, getCacheClass.Name, "Server returned a count with wrong value");
      Backendless.Cache.Delete(DEFAULT_CACHE_KEY);
      isExist = Backendless.Cache.Contains(DEFAULT_CACHE_KEY);
      Assert.AreEqual(isExist, false, "Server returned a count with wrong value");
    }

  }
}