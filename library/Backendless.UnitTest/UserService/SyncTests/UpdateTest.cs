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
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Backendless.Test;
using BackendlessAPI.Exception;
using BackendlessAPI.Property;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BackendlessAPI.Test.UserService.SyncTests
{
  [TestClass]
  public class UpdateTest : TestsFrame
  {
    [TestMethod]
    public void TestUpdateUserWithEmptyCredentials()
    {
      try
      {
        Backendless.InitApp( Defaults.TEST_APP_ID, Defaults.TEST_SECRET_KEY, Defaults.TEST_VERSION );
        BackendlessUser user = GetRandomLoggedInUser();
        user.SetProperty(EMAIL_KEY, "");
        user.Password = "";

        Backendless.UserService.Update( user );
        Assert.Fail( "User with empty credentials accepted" );
      }
      catch( System.Exception t )
      {
        CheckErrorCode(3045, t);
      }
    }

    [TestMethod]
    public void TestUpdateUserWithNullCredentials()
    {
      try
      {
        Backendless.InitApp( Defaults.TEST_APP_ID, Defaults.TEST_SECRET_KEY, Defaults.TEST_VERSION );
        BackendlessUser user = GetRandomLoggedInUser();
        user.SetProperty(EMAIL_KEY, null);
        user.Password = null;

        Backendless.UserService.Update( user );
        Assert.Fail( "User with null credentials accepted" );
      }
      catch( System.Exception t )
      {
        CheckErrorCode(3045, t);
      }
    }

    [TestMethod]
    public void TestUpdateUserWithNullUserId()
    {
      try
      {
        Backendless.InitApp( Defaults.TEST_APP_ID, Defaults.TEST_SECRET_KEY, Defaults.TEST_VERSION );
        BackendlessUser user = GetRandomLoggedInUser();
        user.SetProperty( ID_KEY, null );

        Backendless.UserService.Update( user );
        Assert.Fail( "User with null id accepted" );
      }
      catch( ArgumentNullException )
      {
      }
      catch( System.Exception t )
      {
        CheckErrorCode(ExceptionMessage.WRONG_USER_ID, t);
      }
    }

    [TestMethod]
    public void TestUpdateUserWithEmptyUserId()
    {
      try
      {
        Backendless.InitApp( Defaults.TEST_APP_ID, Defaults.TEST_SECRET_KEY, Defaults.TEST_VERSION );
        BackendlessUser user = GetRandomLoggedInUser();
        user.SetProperty( ID_KEY, "" );

        Backendless.UserService.Update( user );
        Assert.Fail( "User with empty id accepted" );
      }
      catch( System.Exception t )
      {
        CheckErrorCode( ExceptionMessage.WRONG_USER_ID, t );
      }
    }

    [TestMethod]
    public void TestUpdateUserWithWrongUserId()
    {
      try
      {
        Backendless.InitApp( Defaults.TEST_APP_ID, Defaults.TEST_SECRET_KEY, Defaults.TEST_VERSION );
        BackendlessUser user = GetRandomLoggedInUser();
        user.SetProperty(ID_KEY, "foobar");

        Backendless.UserService.Update( user );
        Assert.Fail( "User with wrong id accepted" );
      }
      catch( BackendlessException t )
      {
        CheckErrorCode( 1000, t );
      }
    }
    
    [TestMethod]
    public void TestUpdateUserForVersionWithEnabledDynamicPropertis()
    {
      Backendless.InitApp( Defaults.TEST_APP_ID, Defaults.TEST_SECRET_KEY, Defaults.TEST_VERSION );
      BackendlessUser user = GetRandomLoggedInUser();
      string propertyKey = "somePropertyKey" + Random.Next();
      string propertyValue = "somePropertyValue" + Random.Next();
      user.SetProperty( propertyKey, propertyValue );

      foreach( string usedProperty in UsedProperties )
        user.SetProperty( usedProperty, "someValue" );

      Backendless.UserService.Update( user );
      Backendless.UserService.Login(user.Email, user.Password);

      UsedProperties.Add( propertyKey );

      List<UserProperty> userProperties = Backendless.UserService.DescribeUserClass();
      Assert.IsNotNull( userProperties, "Server returned null user properties" );
      Assert.IsTrue( userProperties.Count != 0, "Server returned empty user properties" );

      bool flag = false;

      foreach( UserProperty userProperty in userProperties )
        if( userProperty.Name.Equals( propertyKey ) )
        {
          flag = true;
          Assert.IsTrue( userProperty.Type.Equals( DateTypeEnum.STRING ),
          "Property had wrong type")
          ;
        }

      Assert.IsTrue( flag, "Expected property was not found" );
    }

    [TestMethod]
    public void TestUpdateRegisteredUserEmailAndPassword()
    {
      const string newpassword = "some_new_password";
      const string newemail = "some_new_email@gmail.com";
      Backendless.InitApp( Defaults.TEST_APP_ID, Defaults.TEST_SECRET_KEY, Defaults.TEST_VERSION );
      BackendlessUser user = GetRandomLoggedInUser();
      user.Password = newpassword;
      user.SetProperty( EMAIL_KEY, newemail );

      Backendless.UserService.Update( user );

      Assert.AreEqual( newpassword, user.Password, "Updated used has a wrong password" );
      Assert.AreEqual( newemail, user.GetProperty( EMAIL_KEY ), "Updated used has a wrong email" );
    }

    [TestMethod]
    public void TestUpdateRegisteredUserIdentity()
    {
      Backendless.InitApp( Defaults.TEST_APP_ID, Defaults.TEST_SECRET_KEY, Defaults.TEST_VERSION );
      BackendlessUser user = GetRandomLoggedInUser();
      user.SetProperty( LOGIN_KEY, "some_new_login_" + user.GetProperty( LOGIN_KEY ) );

      Backendless.UserService.Update( user );
    }
  }
}