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

using BackendlessAPI.Async;
using BackendlessAPI.Data;
using BackendlessAPI.Test.PersistenceService.AsyncEntities;
using BackendlessAPI.Test.PersistenceService.AsyncEntities.DeleteEntities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BackendlessAPI.Test.PersistenceService.AsyncTests
{
  [TestClass]
  public class DeleteObjectTest : TestsFrame
  {
    [TestMethod]
    public void TestDeleteObjectWithWrongId()
    {
      RunAndAwait( () =>
        {
          WPPersonAsync wpPerson = GetRandomWPPerson();
          Backendless.Persistence.Save( wpPerson,
                                        new ResponseCallback<WPPersonAsync>( this )
                                          {
                                            ResponseHandler = response =>
                                              {
                                                wpPerson.ObjectId = "foobar";
                                                Backendless.Persistence.Of<WPPersonAsync>()
                                                           .Remove( wpPerson,
                                                                    new AsyncCallback<long>(
                                                                      l => Assert.Fail( "Server didn't throw an exception" ),
                                                                      fault => CheckErrorCode( 1033, fault ) ) );
                                              }
                                          } );
        } );
    }

    [TestMethod]
    public void TestDeleteObject()
    {
      RunAndAwait( () =>
        {
          var entity = new BaseDeleteEntityAsync {Name = "bot_#delete", Age = 20};
          IDataStore<BaseDeleteEntityAsync> connection = Backendless.Persistence.Of<BaseDeleteEntityAsync>();

          connection.Save( entity,
                           new ResponseCallback<BaseDeleteEntityAsync>( this )
                             {
                               ResponseHandler =
                                 savedEntity =>
                                 connection.Remove( savedEntity,
                                                    new ResponseCallback<long>( this )
                                                      {
                                                        ResponseHandler = response =>
                                                          {
                                                            connection.FindById( savedEntity.ObjectId,
                                                                             new AsyncCallback<BaseDeleteEntityAsync>(
                                                                               @async =>
                                                                               Assert.Fail( "Server probably found a result" ),
                                                                               fault => testLatch.Signal() ) );
                                                          }
                                                      } )
                             } );
        } );
    }
  }
}