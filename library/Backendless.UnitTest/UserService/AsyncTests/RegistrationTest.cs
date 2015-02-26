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
using BackendlessAPI.Exception;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BackendlessAPI.Test.UserService.AsyncTests
{
  [TestClass]
  public class RegistrationTest : TestsFrame
  {
    [TestMethod]
    public void TestRegisterNewUserFromNull()
    {
      RunAndAwait( () =>
        {
          Backendless.InitApp( Defaults.TEST_APP_ID, Defaults.TEST_SECRET_KEY, Defaults.TEST_VERSION );
          Backendless.UserService.Register( null,
                                            new ResponseCallback<BackendlessUser>( this )
                                              {
                                                ResponseHandler =
                                                  response => FailCountDownWith( "UserService accepted a null user" ),
                                                ErrorHandler = fault =>
                                                {
                                                  Assert.IsTrue(fault.ToString().Contains("Value cannot be null"));
                                                  CountDown();
                                                }
                                              } );
        } );
    }

    [TestMethod]
    public void TestRegisterNewUserFromEmptyUser()
    {
      RunAndAwait( () =>
        {
          Backendless.InitApp( Defaults.TEST_APP_ID, Defaults.TEST_SECRET_KEY, Defaults.TEST_VERSION );
          Backendless.UserService.Register( new BackendlessUser(),
                                            new ResponseCallback<BackendlessUser>( this )
                                              {
                                                ResponseHandler =
                                                  response => FailCountDownWith( "UserService accepted a null user" ),
                                                ErrorHandler = fault =>
                                                  {
                                                    Assert.IsTrue( fault.ToString().Contains( "Value cannot be null" ) );
                                                    CountDown();
                                                  }
                                              } );
        } );
    }

    [TestMethod]
    [ExpectedException( typeof( ArgumentNullException ) )]
    public void TestRegisterNewUserWithNulls()
    {
      RunAndAwait( () =>
        {
          Backendless.InitApp( Defaults.TEST_APP_ID, Defaults.TEST_SECRET_KEY, Defaults.TEST_VERSION );
          BackendlessUser user = new BackendlessUser();
          user.SetProperty( LOGIN_KEY, null );
          user.SetProperty( EMAIL_KEY, null );
          user.Password = null;
          user.SetProperty( null, "foo" );
          user.SetProperty( "foo", null );
          Backendless.UserService.Register( user,
                                            new ResponseCallback<BackendlessUser>( this )
                                              {
                                                ResponseHandler =
                                                  response => FailCountDownWith( "UserService accepted null values" )
                                              } );
        } );
    }
    
    [TestMethod]
    public void TestRegisterNewUserWithEmptyCredentials()
    {
      RunAndAwait( () =>
        {
          Backendless.InitApp( Defaults.TEST_APP_ID, Defaults.TEST_SECRET_KEY, Defaults.TEST_VERSION );
          BackendlessUser user = new BackendlessUser();
          user.SetProperty( LOGIN_KEY, "" );
          user.SetProperty( EMAIL_KEY, "" );
          user.Password = "";
          user.SetProperty( "", "foo" );
          user.SetProperty( "foo", "" );
          Backendless.UserService.Register( user,
                                            new ResponseCallback<BackendlessUser>( this )
                                              {
                                                ResponseHandler =
                                                  response => FailCountDownWith( "BackendlessUser accepted empty values" ),
                                                ErrorHandler = fault =>
                                                {
                                                  Assert.IsTrue(fault.ToString().Contains("Value cannot be null"));
                                                  CountDown();
                                                }
                                              } );
        } );
    }

    [TestMethod]
    public void TestRegisterNewUser()
    {
      RunAndAwait( () =>
        {
          Backendless.InitApp( Defaults.TEST_APP_ID, Defaults.TEST_SECRET_KEY, Defaults.TEST_VERSION );
          BackendlessUser user = GetRandomNotRegisteredUser();
          string propertyKey = "property_key_" + Random.Next();
          string propertyValue = "property_value_" + Random.Next();
          user.SetProperty( propertyKey, propertyValue );
          Backendless.UserService.Register( user,
                                            new ResponseCallback<BackendlessUser>( this )
                                              {
                                                ResponseHandler = response =>
                                                  {
                                                    UsedProperties.Add( propertyKey );
                                                    Assert.IsNotNull( response.UserId,
                                                                      "UserService.register didn't set user ID" );

                                                    foreach( String key in user.Properties.Keys )
                                                    {
                                                      Assert.IsTrue( response.Properties.ContainsKey( key ),
                                                                     "Registered user didn`t contain expected property " +
                                                                     key );
                                                      Assert.AreEqual( user.GetProperty( key ), response.GetProperty( key ),
                                                                       "UserService.register changed property " + key );
                                                    }

                                                    CountDown();
                                                  }
                                              } );
        } );
    }

    [TestMethod]
    public void TestRegisterNewUserWithDuplicateIdentity()
    {
      RunAndAwait( () =>
        {
          Backendless.InitApp( Defaults.TEST_APP_ID, Defaults.TEST_SECRET_KEY, Defaults.TEST_VERSION );
          Backendless.UserService.Register( GetRandomNotRegisteredUser(),
                                            new ResponseCallback<BackendlessUser>( this )
                                              {
                                                ResponseHandler = response =>
                                                  {
                                                    BackendlessUser fakeUser = GetRandomNotRegisteredUser();
                                                    fakeUser.SetProperty(EMAIL_KEY, response.GetProperty(EMAIL_KEY));

                                                    Backendless.UserService.Register( fakeUser,
                                                                                      new ResponseCallback<BackendlessUser>(
                                                                                        this )
                                                                                        {
                                                                                          ResponseHandler =
                                                                                            user =>
                                                                                            FailCountDownWith(
                                                                                              "Server accepted a user with id value" ),
                                                                                          ErrorHandler =
                                                                                            fault =>
                                                                                            CheckErrorCode( 3033, fault )
                                                                                        } );
                                                  }
                                              } );
        } );
    }

    [TestMethod]
    public void TestRegisterNewUserWithId()
    {
      RunAndAwait( () =>
        {
          Backendless.InitApp( Defaults.TEST_APP_ID, Defaults.TEST_SECRET_KEY, Defaults.TEST_VERSION );
          Backendless.UserService.Register( GetRandomNotRegisteredUser(),
                                            new ResponseCallback<BackendlessUser>( this )
                                              {
                                                ResponseHandler = response =>
                                                  {
                                                    BackendlessUser fakeUser = GetRandomNotRegisteredUser();
                                                    fakeUser.UserId = response.UserId;

                                                    Backendless.UserService.Register( fakeUser,
                                                                                      new ResponseCallback<BackendlessUser>(
                                                                                        this )
                                                                                        {
                                                                                          ResponseHandler =
                                                                                            user =>
                                                                                            FailCountDownWith(
                                                                                              "Server accepted a user with id value" ),
                                                                                          ErrorHandler =
                                                                                            fault =>
                                                                                            CheckErrorCode( 3039, fault )
                                                                                        } );
                                                  }
                                              } );
        } );
    }
  }
}