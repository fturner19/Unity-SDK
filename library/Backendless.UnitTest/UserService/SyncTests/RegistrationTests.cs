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
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Backendless.Test;
using BackendlessAPI.Exception;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BackendlessAPI.Test.UserService.SyncTests
{
  [TestClass]
  public class RegistrationTests : TestsFrame
  {
    [TestMethod]
    public void TestRegisterNewUserFromNull()
    {
      try
      {
        Backendless.InitApp( Defaults.TEST_APP_ID, Defaults.TEST_SECRET_KEY, Defaults.TEST_VERSION );
        Backendless.UserService.Register( null );
        Assert.Fail( "UserService accepted a null user" );
      }
      catch( System.Exception t )
      {
        CheckErrorCode( ExceptionMessage.NULL_USER, t );
      }
    }

    [TestMethod]
    public void TestRegisterNewUserFromEmptyUser()
    {
      try
      {
        Backendless.InitApp( Defaults.TEST_APP_ID, Defaults.TEST_SECRET_KEY, Defaults.TEST_VERSION );
        Backendless.UserService.Register( new BackendlessUser() );
        Assert.Fail( "UserService accepted a null user" );
      }
      catch( System.Exception t )
      {
        CheckErrorCode( ExceptionMessage.NULL_PASSWORD, t );
      }
    }

    [TestMethod]
    public void TestRegisterNewUserWithNulls()
    {
      try
      {
        Backendless.InitApp(Defaults.TEST_APP_ID, Defaults.TEST_SECRET_KEY, Defaults.TEST_VERSION);
        BackendlessUser user = new BackendlessUser();
        user.SetProperty(LOGIN_KEY, null);
        user.SetProperty(EMAIL_KEY, null);
        user.Password = null;
        user.SetProperty(null, "foo");
        user.SetProperty("foo", null);

        Backendless.UserService.Register(user);
        Assert.Fail("UserService accepted null values");
      }
      catch( ArgumentNullException )
      {
      }
    }
    
    [TestMethod]
    public void TestRegisterNewUserWithEmptyCredentials()
    {
      try
      {
        Backendless.InitApp(Defaults.TEST_APP_ID, Defaults.TEST_SECRET_KEY, Defaults.TEST_VERSION);
        BackendlessUser user = new BackendlessUser();
        user.SetProperty(LOGIN_KEY, "");
        user.SetProperty(EMAIL_KEY, "");
        user.Password = "";
        user.SetProperty("", "foo");
        user.SetProperty("foo", "");

        Backendless.UserService.Register(user);
        Assert.Fail("BackendlessUser accepted empty values");
      }
      catch( ArgumentNullException)
      {
      }
    }

    [TestMethod]
    public void TestRegisterNewUser()
    {
      Backendless.InitApp( Defaults.TEST_APP_ID, Defaults.TEST_SECRET_KEY, Defaults.TEST_VERSION );
      BackendlessUser user = GetRandomNotRegisteredUser();
      String propertyKey = "property_key_" + Random.Next();
      String propertyValue = "property_value_" + Random.Next();
      user.SetProperty( propertyKey, propertyValue );
      BackendlessUser registeredUser = Backendless.UserService.Register( user );

      UsedProperties.Add( propertyKey );

      Assert.IsNotNull(registeredUser.UserId, "UserService.register didn't set user ID");

      foreach( string key in user.Properties.Keys )
      {
        Assert.IsTrue( registeredUser.Properties.ContainsKey( key ),
                       "Registered user didn`t contain expected property " + key );
        Assert.AreEqual( user.GetProperty( key ), registeredUser.GetProperty( key ),
                         "UserService.register changed property " + key );
      }
    }

    [TestMethod]
    public void TestRegisterNewUserWithDuplicateIdentity()
    {
      try
      {
        Backendless.InitApp( Defaults.TEST_APP_ID, Defaults.TEST_SECRET_KEY, Defaults.TEST_VERSION );
        BackendlessUser user = Backendless.UserService.Register( GetRandomNotRegisteredUser() );

        BackendlessUser fakeUser = GetRandomNotRegisteredUser();
        fakeUser.SetProperty(EMAIL_KEY, user.GetProperty(EMAIL_KEY));

        Backendless.UserService.Register( fakeUser );

        Assert.Fail( "Server accepted a user with id value" );
      }
      catch( System.Exception t )
      {
        CheckErrorCode( 3033, t );
      }
    }

    [TestMethod]
    public void TestRegisterNewUserWithId()
    {
      try
      {
        Backendless.InitApp( Defaults.TEST_APP_ID, Defaults.TEST_SECRET_KEY, Defaults.TEST_VERSION );
        BackendlessUser user = Backendless.UserService.Register( GetRandomNotRegisteredUser() );

        BackendlessUser fakeUser = GetRandomNotRegisteredUser();
        fakeUser.SetProperty( ID_KEY, user.GetProperty( ID_KEY ) );

        Backendless.UserService.Register( fakeUser );
        Assert.Fail( "Server accepted a user with id value" );
      }
      catch( System.Exception t )
      {
        CheckErrorCode( 3039, t );
      }
    }
  }
}