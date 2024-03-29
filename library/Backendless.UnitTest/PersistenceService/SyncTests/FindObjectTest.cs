﻿/*
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
using BackendlessAPI.Exception;
using BackendlessAPI.Persistence;
using BackendlessAPI.Test.PersistenceService.Entities.FindEntities;
using BackendlessAPI.Test.PersistenceService.Entities.PrimitiveEntities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BackendlessAPI.Test.PersistenceService.SyncTests
{
  [TestClass]
  public class FindObjectTest : TestsFrame
  {
    [TestMethod]
    public void TestFindRecordById()
    {
      StringEntity entity = new StringEntity();
      entity.StringField = "foobar";

      StringEntity savedEntity = Backendless.Persistence.Save( entity );
      Assert.IsNotNull( savedEntity, "Server returned a null result" );
      Assert.IsNotNull( savedEntity.StringField, "Returned object doesn't have expected field" );
      Assert.IsNotNull( savedEntity.ObjectId, "Returned object doesn't have expected field id" );
      Assert.IsNotNull( savedEntity.Created, "Returned object doesn't have expected field created" );
      Assert.AreEqual( entity.StringField, savedEntity.StringField, "Returned object has wrong field value" );

      var foundEntity = Backendless.Persistence.Of<StringEntity>().FindById( savedEntity.ObjectId );
      Assert.AreEqual( savedEntity.Created, foundEntity.Created, "Found object contain wrong created date" );
      Assert.AreEqual( savedEntity.ObjectId, foundEntity.ObjectId, "Found object contain wrong objectId" );
      Assert.AreEqual( savedEntity.StringField, foundEntity.StringField, "Found object contain wrong field value" );
    }

    [TestMethod]
    public void TestRetrieveObjectWithWrongObjectId()
    {
      try
      {
        Backendless.Persistence.Save( new StringEntity() );
        Backendless.Persistence.Of<StringEntity>().FindById( "foobar" );
        Assert.Fail( "Server didn't throw an exception" );
      }
      catch( System.Exception e )
      {
        CheckErrorCode( 1000, e );
      }
    }

    [TestMethod]
    public void TestFindAllEntities()
    {
      var entities = new List<FindAllEntity>();

      for( int i = 0; i < 10; i++ )
      {
        var findAllEntity = new FindAllEntity {Name = "bot_#" + i, Age = 20 + i};
        Backendless.Persistence.Save( findAllEntity );
        entities.Add( findAllEntity );
      }

      var backendlessCollection = Backendless.Persistence.Of<FindAllEntity>().Find();
      AssertArgumentAndResultCollections( entities, backendlessCollection );
    }

    [TestMethod]
    public void TestFindWithWhereEntities()
    {
      var entities = new List<FindWithWhereEntity>();

      for( int i = 0; i < 10; i++ )
      {
        var findWithWhereEntity = new FindWithWhereEntity();
        findWithWhereEntity.Name = "bot_#" + i;
        findWithWhereEntity.Age = 20 + i;

        Backendless.Persistence.Save( findWithWhereEntity );

        if( i < 5 )
          entities.Add( findWithWhereEntity );
      }

      BackendlessDataQuery dataQuery = new BackendlessDataQuery( "Age < 25" );
      BackendlessCollection<FindWithWhereEntity> backendlessCollection =
        Backendless.Persistence.Of<FindWithWhereEntity>().Find( dataQuery );

      AssertArgumentAndResultCollections( entities, backendlessCollection );
    }

    [TestMethod]
    public void TestFindWithNegativeOffset()
    {
      try
      {
        Backendless.Persistence.Of<Object>().Find( new BackendlessDataQuery( new QueryOptions( 2, -1 ) ) );
        Assert.Fail( "Client accepted a negative offset" );
      }
      catch( System.Exception e )
      {
        CheckErrorCode( ExceptionMessage.WRONG_OFFSET, e );
      }
    }

    [TestMethod]
    public void TestFindWithNegativePageSize()
    {
      try
      {
        Backendless.Persistence.Of<Object>().Find( new BackendlessDataQuery( new QueryOptions( -1, 2 ) ) );
        Assert.Fail( "Client accepted a negative pagesize" );
      }
      catch( System.Exception e )
      {
        CheckErrorCode( ExceptionMessage.WRONG_PAGE_SIZE, e );
      }
    }

    [TestMethod]
    public void TestFindWithMissingProperties()
    {
      var findWithPropertiesEntity = new FindWithPropertiesEntity {Name = "bot_#foobar", Age = 20};
      Backendless.Persistence.Save( findWithPropertiesEntity );

      var properties = new List<string> {"foobar"};
      var dataQuery = new BackendlessDataQuery( properties );

      try
      {
        Backendless.Persistence.Of<FindWithPropertiesEntity>().Find( dataQuery );
        Assert.Fail( "Server didn't throw an exception" );
      }
      catch( System.Exception e )
      {
        CheckErrorCode( 1006, e );
      }
    }

    [TestMethod]
    public void TestFindWithNullPropertiesQuery()
    {
      var findWithPropertiesEntity = new FindWithPropertiesEntity {Name = "bot_#foobar", Age = 20};
      Backendless.Persistence.Save( findWithPropertiesEntity );

      List<String> properties = null;
      var dataQuery = new BackendlessDataQuery( properties );

      BackendlessCollection<FindWithPropertiesEntity> backendlessCollection =
        Backendless.Persistence.Of<FindWithPropertiesEntity>().Find( dataQuery );

      Assert.IsTrue( backendlessCollection.TotalObjects > 0, "Server found wrong number of objects" );
      Assert.IsTrue( backendlessCollection.GetCurrentPage().Count > 0, "Server returned wrong number of objects" );

      foreach( FindWithPropertiesEntity entity in backendlessCollection.GetCurrentPage() )
      {
        Assert.IsTrue( entity.Age > 0, "Server result contained wrong age field value" );
        Assert.IsNotNull( entity.Name, "Server result contained non null field" );
        Assert.IsNotNull( entity.ObjectId, "Server result contained non null field" );
        Assert.IsNotNull( entity.Created, "Server result contained non null field" );
      }
    }

    [TestMethod]
    public void TestFindWithExtraPropertiesQuery()
    {
      var findWithPropertiesEntity = new FindWithPropertiesEntity {Name = "bot_#foobar", Age = 20};
      Backendless.Persistence.Save( findWithPropertiesEntity );

      var properties = new List<string> {"Age"};
      var dataQuery = new BackendlessDataQuery( properties );
      var backendlessCollection = Backendless.Persistence.Of<FindWithPropertiesEntity>().Find( dataQuery );

      Assert.IsTrue( backendlessCollection.TotalObjects > 0, "Server found wrong number of objects" );
      Assert.IsTrue( backendlessCollection.GetCurrentPage().Count > 0, "Server returned wrong number of objects" );

      foreach( FindWithPropertiesEntity entity in backendlessCollection.GetCurrentPage() )
      {
        Assert.IsTrue( entity.Age > 0, "Server result contained wrong age field value" );
        Assert.IsNull( entity.Name, "Server result contained non null field" );
        Assert.IsNull(entity.ObjectId, "Server result contained non null field");
        Assert.IsNull(entity.Created, "Server result contained non null field");
      }
    }

    [TestMethod]
    public void TestFindFirstEntity()
    {
      var firstEntity = new FindFirstEntity {Name = "bot_#first", Age = 20};
      var secondEntity = new FindFirstEntity {Name = "bot_#second", Age = 30};

      Backendless.Persistence.Save( firstEntity );
      Thread.Sleep( 3000 );
      Backendless.Persistence.Save( secondEntity );

      FindFirstEntity foundEntity = Backendless.Persistence.Of<FindFirstEntity>().FindFirst();
      Assert.AreEqual( firstEntity, foundEntity, "Server found unexpected entity" );
    }

    [TestMethod]
    public void TestFindFirstUnknownEntity()
    {
      try
      {
        Backendless.Persistence.Of<FindObjectTest>().FindFirst();
        Assert.Fail( "Server didn't throw an exception" );
      }
      catch( System.Exception e )
      {
        CheckErrorCode( 1009, e );
      }
    }

    [TestMethod]
    public void TestFindLastEntity()
    {
      var firstEntity = new FindLastEntity {Name = "bot_#first", Age = 20};
      var secondEntity = new FindLastEntity {Name = "bot_#second", Age = 30};

      Backendless.Persistence.Save( firstEntity );
      Thread.Sleep( 3000 );
      Backendless.Persistence.Save( secondEntity );

      FindLastEntity foundEntity = Backendless.Persistence.Of<FindLastEntity>().FindLast();
      Assert.AreEqual( secondEntity, foundEntity, "Server found unexpected entity" );
    }

    [TestMethod]
    public void TestFindLastUnknownEntity()
    {
      try
      {
        Backendless.Persistence.Of<FindObjectTest>().FindLast();
        Assert.Fail( "Server didn't throw an exception" );
      }
      catch( System.Exception e )
      {
        CheckErrorCode( 1009, e );
      }
    }

    [TestMethod]
    public void TestFindFirstEntityInEmptyTable()
    {
      var entity = new FindEmptyTableEntity {Name = "bot_#first", Age = 20};
      IDataStore<FindEmptyTableEntity> connection = Backendless.Persistence.Of<FindEmptyTableEntity>();

      FindEmptyTableEntity foundEntity = connection.Save( entity );
      connection.Remove( foundEntity );

      try
      {
        connection.FindFirst();
        Assert.Fail( "Server didn't throw an exception" );
      }
      catch( System.Exception e )
      {
        CheckErrorCode( 1010, e );
      }
    }

    [TestMethod]
    public void TestFindLastEntityInEmptyTable()
    {
      var entity = new FindEmptyTableEntity {Name = "bot_#last", Age = 20};
      IDataStore<FindEmptyTableEntity> connection = Backendless.Persistence.Of<FindEmptyTableEntity>();

      FindEmptyTableEntity foundEntity = connection.Save( entity );
      connection.Remove( foundEntity );

      try
      {
        Object o = connection.FindLast();
        Assert.Fail( "Server didn't throw an exception" );
      }
      catch( System.Exception e )
      {
        CheckErrorCode( 1010, e );
      }
    }
  }
}