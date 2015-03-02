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
using BackendlessAPI.Test.PersistenceService.Entities.UpdateEntities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BackendlessAPI.Test.PersistenceService.SyncTests
{
  [TestClass]
  public class UpdateRecordTest : TestsFrame
  {
    [TestMethod]
    public void TestBasicUpdate()
    {
      BaseUpdateEntity baseUpdateEntity = new BaseUpdateEntity();
      baseUpdateEntity.Name = "foobar";
      baseUpdateEntity.Age = 20;

      BaseUpdateEntity savedEntity = Backendless.Persistence.Save( baseUpdateEntity );
      savedEntity.Name = "foobar1";
      savedEntity.Age = 21;

      Backendless.Persistence.Save( savedEntity );

      BaseUpdateEntity foundEntity = Backendless.Persistence.Of<BaseUpdateEntity>().Find().GetCurrentPage()[0];

      Assert.AreEqual( savedEntity, foundEntity, "Server didn't update an entity" );
      Assert.IsNotNull( foundEntity.Updated, "Server didn't set an updated field value" );
    }
  }
}