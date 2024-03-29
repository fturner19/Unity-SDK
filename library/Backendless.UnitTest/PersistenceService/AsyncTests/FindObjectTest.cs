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
using System.Collections.Generic;
using System.Threading;
using BackendlessAPI.Async;
using BackendlessAPI.Data;
using BackendlessAPI.Exception;
using BackendlessAPI.Persistence;
using BackendlessAPI.Test.PersistenceService.AsyncEntities.FindEntities;
using BackendlessAPI.Test.PersistenceService.AsyncEntities.PrimitiveEntities;
using BackendlessAPI.Test.PersistenceService.Entities.FindEntities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BackendlessAPI.Test.PersistenceService.AsyncTests
{
  [TestClass]
  public class FindObjectTest : TestsFrame
  {
    [TestMethod]
    public void TestFindRecordById()
    {
      RunAndAwait( () =>
        {
          var entity = new StringEntityAsync {StringField = "foobar"};
          Backendless.Persistence.Save( entity,
                                        new ResponseCallback<StringEntityAsync>( this )
                                          {
                                            ResponseHandler = savedEntity =>
                                              {
                                                Assert.IsNotNull( savedEntity, "Server returned a null result" );
                                                Assert.IsNotNull( savedEntity.StringField,
                                                                  "Returned object doesn't have expected field" );
                                                Assert.IsNotNull( savedEntity.ObjectId,
                                                                  "Returned object doesn't have expected field id" );
                                                Assert.IsNotNull( savedEntity.Created,
                                                                  "Returned object doesn't have expected field created" );
                                                Assert.AreEqual( entity.StringField, savedEntity.StringField,
                                                                 "Returned object has wrong field value" );

                                                var foundEntity =
                                                  Backendless.Persistence.Of<StringEntityAsync>()
                                                             .FindById( savedEntity.ObjectId );
                                                Assert.AreEqual( savedEntity.Created, foundEntity.Created,
                                                                 "Found object contain wrong created date" );
                                                Assert.AreEqual( savedEntity.ObjectId, foundEntity.ObjectId,
                                                                 "Found object contain wrong objectId" );
                                                Assert.AreEqual( savedEntity.StringField, foundEntity.StringField,
                                                                 "Found object contain wrong field value" );

                                                CountDown();
                                              }
                                          } );
        } );
    }

    [TestMethod]
    public void TestRetrieveObjectWithWrongObjectId()
    {
      RunAndAwait(
        () =>
        Backendless.Persistence.Save( new StringEntityAsync(),
                                      new ResponseCallback<StringEntityAsync>( this )
                                        {
                                          ResponseHandler =
                                            response =>
                                            Backendless.Persistence.Of<StringEntityAsync>()
                                                       .FindById( "foobar",
                                                                  new AsyncCallback<StringEntityAsync>(
                                                                    @async =>
                                                                    Assert.Fail( "Server didn't throw an exception" ),
                                                                    fault => CheckErrorCode( 1000, fault ) ) )
                                        } ) );
    }

    [TestMethod]
    public void TestFindAllEntities()
    {
      RunAndAwait( () =>
        {
          var entities = new List<FindAllEntityAsync>();
          var latch = new CountdownEvent( 10 );
          for( int i = 0; i < 10; i++ )
          {
            var findAllEntity = new FindAllEntityAsync {Name = "bot_#" + i, Age = 20 + i};
            Backendless.Persistence.Save( findAllEntity, new AsyncCallback<FindAllEntityAsync>( response =>
              {
                entities.Add( findAllEntity );
                latch.Signal();
              }, fault =>
                {
                  for( int j = 0; j < latch.CurrentCount; j++ )
                    latch.Signal();

                  FailCountDownWith( fault );
                } ) );
          }
          latch.Wait();

          Backendless.Persistence.Of<FindAllEntityAsync>()
                     .Find( new ResponseCallback<BackendlessCollection<FindAllEntityAsync>>( this )
                       {
                         ResponseHandler =
                           backendlessCollection => AssertArgumentAndResultCollections( entities, backendlessCollection )
                       } );
        } );
    }

    [TestMethod]
    public void TestFindWithWhereEntities()
    {
      RunAndAwait( () =>
        {
          var entities = new List<FindWithWhereEntityAsync>();
          var latch = new CountdownEvent( 10 );

          for( int i = 0; i < 10; i++ )
          {
            var findWithWhereEntity = new FindWithWhereEntityAsync();
            findWithWhereEntity.Name = "bot_#" + i;
            findWithWhereEntity.Age = 20 + i;

            Backendless.Persistence.Save( findWithWhereEntity, new AsyncCallback<FindWithWhereEntityAsync>( response => latch.Signal(), fault =>
                {
                  for( int j = 0; j < latch.CurrentCount; j++ )
                    latch.Signal();

                  FailCountDownWith( fault );
                } ) );

            if (i < 5)
              entities.Add(findWithWhereEntity);
          }
          latch.Wait();

          BackendlessDataQuery dataQuery = new BackendlessDataQuery( "Age < 25" );
          Backendless.Persistence.Of<FindWithWhereEntityAsync>()
                     .Find( dataQuery,
                            new ResponseCallback<BackendlessCollection<FindWithWhereEntityAsync>>( this )
                              {
                                ResponseHandler =
                                  backendlessCollection =>
                                  AssertArgumentAndResultCollections( entities, backendlessCollection )
                              } );
        } );
    }

    [TestMethod]
    public void TestFindWithNegativeOffset()
    {
      RunAndAwait(
        () =>
        Backendless.Persistence.Of<Object>()
                   .Find( new BackendlessDataQuery( new QueryOptions( 2, -1 ) ),
                          new AsyncCallback<BackendlessCollection<object>>(
                            response => Assert.Fail( "Client accepted a negative offset" ),
                            fault => CheckErrorCode( 1009, fault ) ) ) );
    }

    [TestMethod]
    public void TestFindWithNegativePageSize()
    {
      RunAndAwait(
        () =>
        Backendless.Persistence.Of<Object>()
                   .Find( new BackendlessDataQuery( new QueryOptions( -1, 2 ) ),
                          new AsyncCallback<BackendlessCollection<object>>(
                            response => Assert.Fail( "Client accepted a negative pagesize" ),
                            fault => CheckErrorCode(1009, fault))));
    }

    [TestMethod]
    public void TestFindWithMissingProperties()
    {
      RunAndAwait( () =>
        {
          var findWithPropertiesEntity = new FindWithPropertiesEntityAsync {Name = "bot_#foobar", Age = 20};
          Backendless.Persistence.Save( findWithPropertiesEntity,
                                        new ResponseCallback<FindWithPropertiesEntityAsync>( this )
                                          {
                                            ResponseHandler = response =>
                                              {
                                                var properties = new List<string> {"foobar"};
                                                var dataQuery = new BackendlessDataQuery( properties );

                                                Backendless.Persistence.Of<FindWithPropertiesEntityAsync>()
                                                           .Find( dataQuery,
                                                                  new AsyncCallback
                                                                    <BackendlessCollection<FindWithPropertiesEntityAsync>>
                                                                    ( collection =>
                                                                      Assert.Fail( "Server didn't throw an exception" ),
                                                                      fault => CheckErrorCode( 1006, fault ) ) );
                                              }
                                          } );
        } );
    }

    [TestMethod]
    public void TestFindWithNullPropertiesQuery()
    {
      RunAndAwait( () =>
        {
          var findWithPropertiesEntity = new FindWithPropertiesEntityAsync {Name = "bot_#foobar", Age = 20};
          Backendless.Persistence.Save( findWithPropertiesEntity,
                                        new ResponseCallback<FindWithPropertiesEntityAsync>( this )
                                          {
                                            ResponseHandler = response =>
                                              {
                                                List<String> properties = null;
                                                var dataQuery = new BackendlessDataQuery( properties );

                                                Backendless.Persistence.Of<FindWithPropertiesEntityAsync>()
                                                           .Find( dataQuery,
                                                                  new ResponseCallback
                                                                    <BackendlessCollection<FindWithPropertiesEntityAsync>>
                                                                    ( this )
                                                                    {
                                                                      ResponseHandler = backendlessCollection =>
                                                                        {
                                                                          Assert.IsTrue(
                                                                            backendlessCollection.TotalObjects > 0,
                                                                            "Server found wrong number of objects" );
                                                                          Assert.IsTrue(
                                                                            backendlessCollection.GetCurrentPage().Count > 0,
                                                                            "Server returned wrong number of objects" );

                                                                          foreach( FindWithPropertiesEntityAsync entity in
                                                                            backendlessCollection.GetCurrentPage() )
                                                                          {
                                                                            Assert.IsTrue( entity.Age > 0,
                                                                                           "Server result contained wrong age field value" );
                                                                            Assert.IsNotNull( entity.Name,
                                                                                              "Server result contained non null field" );
                                                                            Assert.IsNotNull( entity.ObjectId,
                                                                                              "Server result contained non null field" );
                                                                            Assert.IsNotNull( entity.Created,
                                                                                              "Server result contained non null field" );
                                                                          }

                                                                          CountDown();
                                                                        }
                                                                    } );
                                              }
                                          } );
        } );
    }

    [TestMethod]
    public void TestFindWithExtraPropertiesQuery()
    {
      RunAndAwait( () =>
        {
          var findWithPropertiesEntity = new FindWithPropertiesEntityAsync {Name = "bot_#foobar", Age = 20};
          Backendless.Persistence.Save( findWithPropertiesEntity,
                                        new ResponseCallback<FindWithPropertiesEntityAsync>( this )
                                          {
                                            ResponseHandler = response =>
                                              {
                                                var properties = new List<string> {"Age"};
                                                var dataQuery = new BackendlessDataQuery( properties );
                                                Backendless.Persistence.Of<FindWithPropertiesEntityAsync>()
                                                           .Find( dataQuery,
                                                                  new ResponseCallback
                                                                    <BackendlessCollection<FindWithPropertiesEntityAsync>>
                                                                    ( this )
                                                                    {
                                                                      ResponseHandler = backendlessCollection =>
                                                                        {
                                                                          Assert.IsTrue(
                                                                            backendlessCollection.TotalObjects > 0,
                                                                            "Server found wrong number of objects" );
                                                                          Assert.IsTrue(
                                                                            backendlessCollection.GetCurrentPage().Count > 0,
                                                                            "Server returned wrong number of objects" );

                                                                          foreach( FindWithPropertiesEntityAsync entity in
                                                                            backendlessCollection.GetCurrentPage() )
                                                                          {
                                                                            Assert.IsTrue( entity.Age > 0,
                                                                                           "Server result contained wrong age field value" );
                                                                            Assert.IsNull( entity.Name,
                                                                                           "Server result contained non null field" );
                                                                            Assert.IsNull( entity.ObjectId,
                                                                                           "Server result contained non null field" );
                                                                            Assert.IsNull( entity.Created,
                                                                                           "Server result contained non null field" );
                                                                          }

                                                                          CountDown();
                                                                        }
                                                                    } );
                                              }
                                          } );
        } );
    }

    [TestMethod]
    public void TestFindFirstEntity()
    {
      RunAndAwait( () =>
        {
          var firstEntity = new FindFirstEntityAsync {Name = "bot_#first", Age = 20};
          var secondEntity = new FindFirstEntityAsync {Name = "bot_#second", Age = 30};

          Backendless.Persistence.Save( firstEntity,
                                        new ResponseCallback<FindFirstEntityAsync>( this )
                                          {
                                            ResponseHandler =
                                              response =>
                                              Backendless.Persistence.Save( secondEntity,
                                                                            new ResponseCallback<FindFirstEntityAsync>(
                                                                              this )
                                                                              {
                                                                                ResponseHandler = @async =>
                                                                                  {
                                                                                    FindFirstEntityAsync foundEntity =
                                                                                      Backendless.Persistence
                                                                                                 .Of<FindFirstEntityAsync>()
                                                                                                 .FindFirst();
                                                                                    Assert.AreEqual( firstEntity,
                                                                                                     foundEntity,
                                                                                                     "Server found unexpected entity. Expected: {" + firstEntity.Age + ":" + firstEntity.Name + "}, found: {" + foundEntity.Age + ":" + foundEntity.Name + "}" );
                                                                                    CountDown();
                                                                                  }
                                                                              } )
                                          } );
        } );
    }

    [TestMethod]
    public void TestFindFirstUnknownEntity()
    {
      RunAndAwait(
        () =>
        Backendless.Persistence.Of<FindObjectTest>()
                   .FindFirst(
                     new AsyncCallback<FindObjectTest>( response => Assert.Fail( "Server didn't throw an exception" ),
                                                        fault => CheckErrorCode( 1009, fault ) ) ) );
    }

    [TestMethod]
    public void TestFindLastEntity()
    {
      RunAndAwait( () =>
        {
          var firstEntity = new FindLastEntityAsync {Name = "bot_#first", Age = 20};
          var secondEntity = new FindLastEntityAsync {Name = "bot_#second", Age = 30};

          Backendless.Persistence.Save( firstEntity,
                                        new ResponseCallback<FindLastEntityAsync>( this )
                                          {
                                            ResponseHandler =
                                              response =>
                                              Backendless.Persistence.Save( secondEntity,
                                                                            new ResponseCallback<FindLastEntityAsync>(
                                                                              this )
                                                                              {
                                                                                ResponseHandler = @async => Backendless.Persistence
                                                                                                                       .Of<FindLastEntityAsync>()
                                                                                                                       .FindLast(new ResponseCallback<FindLastEntityAsync>(this)
                                                                                                                         {
                                                                                                                           ResponseHandler = foundEntity
                                                                                                                                             =>
                                                                                                                             {
                                                                                                                               Assert.AreEqual(
                                                                                                                                 secondEntity,
                                                                                                                                 foundEntity,
                                                                                                                                 "Server found unexpected entity" );
                                                                                                                               CountDown();
                                                                                                                             }
                                                                                                                         })} )
                                          } );
        } );
    }

    [TestMethod]
    public void TestFindLastUnknownEntity()
    {
      RunAndAwait(
        () =>
        Backendless.Persistence.Of<FindObjectTest>()
                   .FindLast(
                     new AsyncCallback<FindObjectTest>( response => Assert.Fail( "Server didn't throw an exception" ),
                                                        fault => CheckErrorCode( 1009, fault ) ) ) );
    }

    [TestMethod]
    public void TestFindFirstEntityInEmptyTable()
    {
      RunAndAwait( () =>
        {
          var entity = new FindEmptyTableEntityAsync {Name = "bot_#first", Age = 20};
          IDataStore<FindEmptyTableEntityAsync> connection = Backendless.Persistence.Of<FindEmptyTableEntityAsync>();

          connection.Save( entity,
                           new ResponseCallback<FindEmptyTableEntityAsync>( this )
                             {
                               ResponseHandler =
                                 foundEntity =>
                                 connection.Remove( foundEntity,
                                                    new ResponseCallback<long>( this )
                                                      {
                                                        ResponseHandler =
                                                          response =>
                                                          connection.FindFirst(
                                                            new AsyncCallback<FindEmptyTableEntityAsync>(
                                                              tableEntity =>
                                                              Assert.Fail( "Server didn't throw an exception" ),
                                                              fault => CheckErrorCode( 1010, fault ) ) )
                                                      } )
                             } );
        } );
    }

    [TestMethod]
    public void TestFindLastEntityInEmptyTable()
    {
      RunAndAwait( () =>
        {
          var entity = new FindEmptyTableEntityAsync {Name = "bot_#last", Age = 20};
          IDataStore<FindEmptyTableEntityAsync> connection = Backendless.Persistence.Of<FindEmptyTableEntityAsync>();

          connection.Save( entity,
                           new ResponseCallback<FindEmptyTableEntityAsync>( this )
                             {
                               ResponseHandler =
                                 foundEntity =>
                                 connection.Remove( foundEntity,
                                                    new ResponseCallback<long>( this )
                                                      {
                                                        ResponseHandler =
                                                          response =>
                                                          connection.FindLast(
                                                            new AsyncCallback<FindEmptyTableEntityAsync>(
                                                              tableEntity =>
                                                              Assert.Fail( "Server didn't throw an exception" ),
                                                              fault => CheckErrorCode( 1010, fault ) ) )
                                                      } )
                             } );
        } );
    }
  }
}