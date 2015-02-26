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
using Backendless.Test;
using BackendlessAPI.Async;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BackendlessAPI.Test.UserService.AsyncTests
{
  [TestClass]
  public class RestorePasswordTest : TestsFrame
  {
    [TestMethod]
    public void TestRestoreUserPassword()
    {
      RunAndAwait( () =>
        {
          Backendless.InitApp( Defaults.TEST_APP_ID, Defaults.TEST_SECRET_KEY, Defaults.TEST_VERSION );
          GetRandomRegisteredUser(new ResponseCallback<BackendlessUser>(this)
            {
              ResponseHandler =
                response =>
                Backendless.UserService.RestorePassword( (string) response.Email,
                                                         new ResponseCallback<object>( this ) )
            } );
        } );
    }

    [TestMethod]
    public void TestRestoreUserPasswordWithWrongLogin()
    {
      RunAndAwait( () =>
        {
          Backendless.InitApp( Defaults.TEST_APP_ID, Defaults.TEST_SECRET_KEY, Defaults.TEST_VERSION );
          GetRandomLoggedInUser( new ResponseCallback<BackendlessUser>( this )
            {
              ResponseHandler =
                response =>
                Backendless.UserService.RestorePassword( "fake_login_" + response.Email,
                                                         new AsyncCallback<object>(
                                                           o => FailCountDownWith( "Server accepted wrong login." ),
                                                           fault => CheckErrorCode( 3020, fault ) ) )
            } );
        } );
    }
  }
}