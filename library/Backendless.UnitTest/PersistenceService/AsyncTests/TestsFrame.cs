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
using BackendlessAPI.Data;
using BackendlessAPI.Test.PersistenceService.AsyncEntities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BackendlessAPI.Test.PersistenceService.AsyncTests
{
  [TestClass]
  public class TestsFrame: IAsyncTest
  {
    private Random random = new Random();
    public const string LOGIN_KEY = "login";
    public const string EMAIL_KEY = "email";
    public const string PASSWORD_KEY = "password";
    public const string ID_KEY = "objectId";

    public WPPersonAsync GetRandomWPPerson()
    {
      return new WPPersonAsync {Age = random.Next( 80 ), Name = "bot_" + DateTime.Now.Ticks};
    }

    public void AssertArgumentAndResultCollections<T>( List<T> entities, BackendlessCollection<T> backendlessCollection )
    {
      Assert.AreEqual( entities.Count, backendlessCollection.TotalObjects, "Server found wrong number of objects" );
      Assert.AreEqual( entities.Count, backendlessCollection.GetCurrentPage().Count,
                       "Server returned wrong number of objects" );

      foreach( T entity in entities )
        Assert.IsTrue( backendlessCollection.GetCurrentPage().Contains( entity ),
                       "Server result didn't contain expected entity" );

      CountDown();
    }

    [TestInitialize]
    public void SetUp()
    {
      Backendless.InitApp( Defaults.TEST_APP_ID, Defaults.TEST_SECRET_KEY, Defaults.TEST_VERSION );
    }
  }
}