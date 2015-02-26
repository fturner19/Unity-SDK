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

using BackendlessAPI.Data;
using BackendlessAPI.Test.PersistenceService.AsyncEntities.UpdateEntities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BackendlessAPI.Test.PersistenceService.AsyncTests
{
  [TestClass]
  public class UpdateRecordTest : TestsFrame
  {
    [TestMethod]
    public void TestBasicUpdate()
    {
      RunAndAwait( () =>
        {
          BaseUpdateEntityAsync baseUpdateEntity = new BaseUpdateEntityAsync();
          baseUpdateEntity.Name = "foobar";
          baseUpdateEntity.Age = 20;

          Backendless.Persistence.Save( baseUpdateEntity,
                                        new ResponseCallback<BaseUpdateEntityAsync>( this )
                                          {
                                            ResponseHandler = savedEntity =>
                                              {
                                                savedEntity.Name = "foobar1";
                                                savedEntity.Age = 21;

                                                Backendless.Persistence.Save( savedEntity,
                                                                              new ResponseCallback<BaseUpdateEntityAsync>(
                                                                                this )
                                                                                {
                                                                                  ResponseHandler = response =>
                                                                                    {
                                                                                      Backendless.Persistence
                                                                                                 .Of<BaseUpdateEntityAsync>()
                                                                                                 .Find(
                                                                                                   new ResponseCallback
                                                                                                     <
                                                                                                     BackendlessCollection
                                                                                                     <BaseUpdateEntityAsync>
                                                                                                     >( this )
                                                                                                     {
                                                                                                       ResponseHandler =
                                                                                                         collection =>
                                                                                                           {
                                                                                                             BaseUpdateEntityAsync
                                                                                                               foundEntity =
                                                                                                                 collection
                                                                                                                   .GetCurrentPage
                                                                                                                   ()[0];
                                                                                                             Assert.AreEqual
                                                                                                               ( savedEntity,
                                                                                                                 foundEntity,
                                                                                                                 "Server didn't update an entity" );
                                                                                                             Assert
                                                                                                               .IsNotNull(
                                                                                                                 foundEntity
                                                                                                                   .Updated,
                                                                                                                 "Server didn't set an updated field value" );

                                                                                                             CountDown();
                                                                                                           }
                                                                                                     } );
                                                                                    }
                                                                                } );
                                              }
                                          } );
        } );
    }
  }
}