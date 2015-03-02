/*
Copyright 2015 Backendless Corp. All Rights Reserved.

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
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using BackendlessAPI.Data;
using BackendlessAPI.Persistence;
using BackendlessAPI.Test.PersistenceService.Entities.CollectionEntities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BackendlessAPI.Test.PersistenceService.SyncTests
{
  public class Person: BackendlessEntity
  {
    public Address Address { get; set; }
    public string Name { get; set; }
  }

  public class Address: BackendlessEntity
  {
    public string Street { get; set; }
  }

  [TestClass]
  public class CollectionTest: TestsFrame
  {
    [TestMethod]
    public void TestCollectionNextPage()
    {
      var nextPageEntities = new List<NextPageEntity>();

      for( int i = 10; i < 30; i++ )
      {
        var entity = new NextPageEntity {Name = "name#" + i, Age = 20 + i};
        Backendless.Persistence.Save( entity );

        if( i >= 20 )
          nextPageEntities.Add( entity );

        Thread.Sleep( 1000 );
      }

      var dataQuery = new BackendlessDataQuery( new QueryOptions( 10, 0, "Age" ) );
      var collection = Backendless.Persistence.Of<NextPageEntity>().Find( dataQuery ).NextPage();

      Assert.IsNotNull( collection, "Next page returned a null object" );
      Assert.IsNotNull( collection.GetCurrentPage(), "Next page contained a wrong data size" );
      Assert.AreEqual( nextPageEntities.Count, collection.GetCurrentPage().Count, "Next page returned a wrong size" );

      foreach( NextPageEntity entity in nextPageEntities )
        Assert.IsTrue( collection.GetCurrentPage().Contains( entity ), "Server result didn't contain expected entity" );
    }

    [TestMethod]
    public void TestCollectionGetPage()
    {
      var getPageEntities = new List<GetPageEntity>();

      for( int i = 10; i < 30; i++ )
      {
        var entity = new GetPageEntity {Name = "name#" + i, Age = 20 + i};
        Backendless.Persistence.Save( entity );

        if( i > 19 && i < 30 )
          getPageEntities.Add( entity );

        Thread.Sleep( 1000 );
      }

      var dataQuery = new BackendlessDataQuery( new QueryOptions( 10, 0, "Age" ) );
      var collection = Backendless.Persistence.Of<GetPageEntity>().Find( dataQuery ).GetPage( 10, 10 );

      Assert.IsNotNull( collection, "Next page returned a null object" );
      Assert.IsNotNull( collection.GetCurrentPage(), "Next page contained a wrong data size" );
      Assert.AreEqual( getPageEntities.Count, collection.GetCurrentPage().Count, "Next page returned a wrong size" );

      foreach( GetPageEntity entity in getPageEntities )
        Assert.IsTrue( collection.GetCurrentPage().Contains( entity ), "Server result didn't contain expected entity" );
    }
  }
}