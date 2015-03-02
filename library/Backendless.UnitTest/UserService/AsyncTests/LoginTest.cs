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
using Backendless.Test;
using BackendlessAPI.Async;
using BackendlessAPI.Exception;
using BackendlessAPI.Property;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BackendlessAPI.Test.UserService.AsyncTests
{
  [TestClass]
  public class LoginTest : TestsFrame
  {
    [TestMethod]
    public void TestLoginWithNullLogin()
    {
      RunAndAwait(() =>
        {
          Backendless.InitApp(Defaults.TEST_APP_ID, Defaults.TEST_SECRET_KEY, Defaults.TEST_VERSION);
          GetRandomRegisteredUser(new ResponseCallback<BackendlessUser>(this)
            {
              ResponseHandler =
                response =>
                Backendless.UserService.Login(null, response.Password,
                                               new AsyncCallback<BackendlessUser>(
                                                 user => FailCountDownWith("UserService accepted null login"),
                                                 fault => CheckErrorCode(ExceptionMessage.NULL_LOGIN, fault)))
            });
        });
    }

    [TestMethod]
    public void TestLoginWithNullPassword()
    {
      RunAndAwait(() =>
        {
          Backendless.InitApp(Defaults.TEST_APP_ID, Defaults.TEST_SECRET_KEY, Defaults.TEST_VERSION);
          GetRandomRegisteredUser(new ResponseCallback<BackendlessUser>(this)
            {
              ResponseHandler =
                response =>
                Backendless.UserService.Login((string)response.Email, null,
                                               new AsyncCallback<BackendlessUser>(
                                                 user => FailCountDownWith("UserService accepted null password"),
                                                 fault => CheckErrorCode(ExceptionMessage.NULL_PASSWORD, fault)))
            });
        });
    }

    [TestMethod]
    public void TestLoginWithWrongCredentials()
    {
      RunAndAwait(() =>
        {
          Backendless.InitApp(Defaults.TEST_APP_ID, Defaults.TEST_SECRET_KEY, Defaults.TEST_VERSION);
          GetRandomRegisteredUser(new ResponseCallback<BackendlessUser>(this)
            {
              ResponseHandler =
                response =>
                Backendless.UserService.Login(response.Email + "foobar", response.Password + "foobar",
                                               new AsyncCallback<BackendlessUser>(
                                                 user => FailCountDownWith("Server accepted wrong credentials"),
                                                 fault => CheckErrorCode(3003, fault)))
            });
        });
    }
    
    [TestMethod]
    public void TestLoginWithProperCredentials()
    {
      RunAndAwait(() =>
        {
          Backendless.InitApp(Defaults.TEST_APP_ID, Defaults.TEST_SECRET_KEY, Defaults.TEST_VERSION);
          GetRandomRegisteredUser(new ResponseCallback<BackendlessUser>(this)
            {
              ResponseHandler =
                response =>
                Backendless.UserService.Login((string)response.Email, response.Password,
                                               new ResponseCallback<BackendlessUser>(this)
                                                 {
                                                   ResponseHandler =
                                                     user =>
                                                     Backendless.UserService.DescribeUserClass(
                                                       new ResponseCallback<List<UserProperty>>(this))
                                                 })
            });
        });
    }

    [TestMethod]
    public void TestLoginWithoutFailedLoginsLock()
    {
      RunAndAwait(() =>
        {
          Backendless.InitApp(Defaults.TEST_APP_ID, Defaults.TEST_SECRET_KEY, Defaults.TEST_VERSION);
          GetRandomRegisteredUser(new ResponseCallback<BackendlessUser>(this)
            {
              ResponseHandler =
                response =>
                Backendless.UserService.Login((string)response.Email, response.Password + "foo",
                                               new AsyncCallback<BackendlessUser>(
                                                 user => FailCountDownWith("Server didn't locked login"), fault =>
                                                   {
                                                     Backendless.UserService.Login(
                                                       (string)response.Email, response.Password,
                                                       new ResponseCallback<BackendlessUser>(this)
                                                         {
                                                           ResponseHandler = user => CountDown()
                                                         });
                                                   }))
            });
        });
    }

  }
}