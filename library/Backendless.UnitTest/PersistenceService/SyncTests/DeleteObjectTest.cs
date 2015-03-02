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

using System;
using BackendlessAPI.Data;
using BackendlessAPI.Test.PersistenceService.Entities;
using BackendlessAPI.Test.PersistenceService.Entities.DeleteEntities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BackendlessAPI.Test.PersistenceService.SyncTests
{
  [TestClass]
  public class DeleteObjectTest : TestsFrame
  {
    [TestMethod]
    public void TestDeleteObjectWithWrongId()
    {
      WPPerson wpPerson = GetRandomWPPerson();
      Backendless.Persistence.Save( wpPerson );
      wpPerson.ObjectId = "foobar";

      try
      {
        Backendless.Persistence.Of<WPPerson>().Remove( wpPerson );
        Assert.Fail( "Server didn't throw an exception" );
      }
      catch( System.Exception e )
      {
        CheckErrorCode( 1033, e );
      }
    }

    [TestMethod]
    public void TestDeleteObject()
    {
      var entity = new BaseDeleteEntity {Name = "bot_#delete", Age = 20};
      IDataStore<BaseDeleteEntity> connection = Backendless.Persistence.Of<BaseDeleteEntity>();

      BaseDeleteEntity savedEntity = connection.Save( entity );
      connection.Remove( savedEntity );

      try
      {
        connection.FindById( savedEntity.ObjectId );
        Assert.Fail( "Server probably found a result" );
      }
      catch( System.Exception )
      {
      }
    }
  }
}