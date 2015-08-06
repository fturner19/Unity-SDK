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
using Backendless.Test;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BackendlessAPI.Test.UserService.SyncTests
{
  [TestClass]
  public class LoginTest: TestsFrame
  {
    [TestMethod]
  public void TestLoginWithNullLogin()
  {
      try
      {
      Backendless.InitApp( Defaults.TEST_APP_ID, Defaults.TEST_SECRET_KEY, Defaults.TEST_VERSION );
      Backendless.UserService.Login( null, GetRandomRegisteredUser().Password );
      Backendless.UserService.Logout();
      Assert.Fail( "UserService accepted null Login" );
    }
    catch( System.Exception t )
    {
     CheckErrorCode( Exception.ExceptionMessage.NULL_LOGIN, t );
    }
  }

  [TestMethod]
  public void TestLoginWithNullPassword() 
  {
    try
    {
      Backendless.InitApp( Defaults.TEST_APP_ID, Defaults.TEST_SECRET_KEY, Defaults.TEST_VERSION );
      Backendless.UserService.Login((string)GetRandomRegisteredUser().Email, null);
      Assert.Fail( "UserService accepted null password" );
    }
    catch( System.Exception t )
    {
      CheckErrorCode( BackendlessAPI.Exception.ExceptionMessage.NULL_PASSWORD, t );
    }
  }

  [TestMethod]
  public void TestLoginWithWrongCredentials()
  {
    try
    {
      Backendless.InitApp( Defaults.TEST_APP_ID, Defaults.TEST_SECRET_KEY, Defaults.TEST_VERSION );
      var user = GetRandomRegisteredUser();
      Backendless.UserService.Login(user.Email + "foobar", user.Password + "foobar");
      Assert.Fail( "Server accepted wrong credentials" );
    }
    catch( System.Exception t )
    {
     CheckErrorCode( 3003, t );
    }
  }

  [TestMethod]
  public void TestLoginWithProperCredentials() 
  {
    try
    {
      Backendless.InitApp( Defaults.TEST_APP_ID, Defaults.TEST_SECRET_KEY, Defaults.TEST_VERSION );
      BackendlessUser user = GetRandomRegisteredUser();
      Backendless.UserService.Login((string) user.Email, user.Password);
      Backendless.UserService.DescribeUserClass();
    }
    catch( System.Exception t )
    {
      Assert.Fail( t.Message );
    }
  }

  [TestMethod]
  public void TestLoginWithoutFailedLoginsLock() 
  {
    Backendless.InitApp(Defaults.TEST_APP_ID, Defaults.TEST_SECRET_KEY, Defaults.TEST_VERSION);
    BackendlessUser user = GetRandomRegisteredUser();

    try
    {
      Backendless.UserService.Login( (string) user.Email, user.Password + "foo" );
    }
    catch( System.Exception /* t */ )
    {
    }

    try
    {
      Backendless.UserService.Login( (string) user.Email, user.Password );
    }
    catch( System.Exception t )
    {
      Assert.Fail( t.Message );
    }
  }

  }
}
