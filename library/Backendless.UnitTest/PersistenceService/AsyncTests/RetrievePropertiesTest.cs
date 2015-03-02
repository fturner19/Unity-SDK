/*
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

using System.Collections.Generic;
using BackendlessAPI.Async;
using BackendlessAPI.Property;
using BackendlessAPI.Test.PersistenceService.AsyncEntities;
using BackendlessAPI.Test.PersistenceService.Entities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BackendlessAPI.Test.PersistenceService.AsyncTests
{
  [TestClass]
  public class RetrievePropertiesTest : TestsFrame
  {
    [TestMethod]
    public void TestRetrieveEntityProperties()
    {
      RunAndAwait( () =>
        {
          WPPersonAsync wpPerson = GetRandomWPPerson();
          Backendless.Persistence.Save( wpPerson,
                                        new ResponseCallback<WPPersonAsync>( this )
                                          {
                                            ResponseHandler =
                                              response =>
                                              Backendless.Persistence.Describe( typeof( WPPersonAsync ).Name,
                                                                                new ResponseCallback<List<ObjectProperty>>
                                                                                  ( this )
                                                                                  {
                                                                                    ResponseHandler = properties =>
                                                                                      {
                                                                                        Assert.IsNotNull( properties,
                                                                                                          "Server returned null" );
                                                                                        Assert.AreEqual(
                                                                                          properties.Count, 6,
                                                                                          "Server returned unexpected amount of properties" );

                                                                                        foreach(
                                                                                          ObjectProperty property in
                                                                                            properties )
                                                                                        {
                                                                                          if( property.Name.Equals( "Age" ) )
                                                                                          {
                                                                                            Assert.AreEqual(
                                                                                              DateTypeEnum.INT,
                                                                                              property.Type,
                                                                                              "Property was of unexpected type" );
                                                                                            Assert.IsFalse(
                                                                                              property.IsRequired,
                                                                                              "Property had a wrong required value" );
                                                                                          }
                                                                                          else if(
                                                                                            property.Name.Equals( "Name" ) )
                                                                                          {
                                                                                            Assert.AreEqual(
                                                                                              DateTypeEnum.STRING,
                                                                                              property.Type,
                                                                                              "Property was of unexpected type" );
                                                                                            Assert.IsFalse(
                                                                                              property.IsRequired,
                                                                                              "Property had a wrong required value" );
                                                                                          }
                                                                                          else if(
                                                                                            property.Name.Equals(
                                                                                              "created" ) )
                                                                                          {
                                                                                            Assert.AreEqual(
                                                                                              DateTypeEnum.DATETIME,
                                                                                              property.Type,
                                                                                              "Property was of unexpected type" );
                                                                                            Assert.IsFalse(
                                                                                              property.IsRequired,
                                                                                              "Property had a wrong required value" );
                                                                                          }
                                                                                          else if(
                                                                                            property.Name.Equals(
                                                                                              "objectId" ) )
                                                                                          {
                                                                                            Assert.AreEqual(
                                                                                              DateTypeEnum.STRING_ID,
                                                                                              property.Type,
                                                                                              "Property was of unexpected type" );
                                                                                            Assert.IsFalse(
                                                                                              property.IsRequired,
                                                                                              "Property had a wrong required value" );
                                                                                          }
                                                                                          else if(
                                                                                            property.Name.Equals(
                                                                                              "updated" ) )
                                                                                          {
                                                                                            Assert.AreEqual(
                                                                                              DateTypeEnum.DATETIME,
                                                                                              property.Type,
                                                                                              "Property was of unexpected type" );
                                                                                            Assert.IsFalse(
                                                                                              property.IsRequired,
                                                                                              "Property had a wrong required value" );
                                                                                          }
                                                                                          else if (
                                                                                            property.Name.Equals(
                                                                                              "ownerId"))
                                                                                          {
                                                                                            Assert.AreEqual(
                                                                                              DateTypeEnum.STRING,
                                                                                              property.Type,
                                                                                              "Property was of unexpected type");
                                                                                            Assert.IsFalse(
                                                                                              property.IsRequired,
                                                                                              "Property had a wrong required value");
                                                                                          }
                                                                                          else
                                                                                          {
                                                                                            Assert.Fail(
                                                                                              "Got unexpected property: " +
                                                                                              property.Name );
                                                                                          }
                                                                                        }

                                                                                        CountDown();
                                                                                      }
                                                                                  } )
                                          } );
        } );
    }

    [TestMethod]
    public void TestRetrievePropertiesForUnknownObject()
    {
      RunAndAwait(
        () =>
        Backendless.Persistence.Describe( this.GetType().Name,
                                          new AsyncCallback<List<ObjectProperty>>(
                                            response => Assert.Fail( "Server didn't throw an exception" ),
                                            fault => CheckErrorCode( 1009, fault ) ) ) );

    }
  }
}