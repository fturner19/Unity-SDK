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
using System.Collections.Generic;
using BackendlessAPI.Async;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BackendlessAPI.Test.UserService.AsyncTests
{
  [Ignore]
  [TestClass]
  public class TestsFrame : IAsyncTest
  {
    public Random Random = new Random();
    public List<String> UsedProperties = new List<String>();

    public const string LOGIN_KEY = "login";
    public const string EMAIL_KEY = "email";
    public const string PASSWORD_KEY = "password";
    public const string ID_KEY = "objectId";

    public BackendlessUser GetRandomNotRegisteredUser()
    {
      var timestamp = (DateTime.UtcNow.Ticks + Random.Next()).ToString();
      BackendlessUser result = new BackendlessUser();
      result.SetProperty( LOGIN_KEY, "bot" + timestamp );
      result.SetProperty( EMAIL_KEY, result.GetProperty( LOGIN_KEY ) + "@gmail.com" );
      result.Password = "somepass_" + timestamp;

      return result;
    }

    public void GetRandomRegisteredUser( AsyncCallback<BackendlessUser> callback )
    {
      Backendless.UserService.Register( GetRandomNotRegisteredUser(), callback );
    }

    public void GetRandomLoggedInUser( AsyncCallback<BackendlessUser> callback )
    {
      GetRandomRegisteredUser(
        new AsyncCallback<BackendlessUser>(
          response =>
          Backendless.UserService.Login((string)response.Email, response.Password,
                                         new AsyncCallback<BackendlessUser>( r =>
                                           {
                                             if( callback != null )
                                             {
                                               callback.ResponseHandler.Invoke( response );
                                             }
                                           }, fault =>
                                             {
                                               if( callback != null )
                                               {
                                                 callback.ErrorHandler.Invoke( fault );
                                               }
                                             } ) ), fault =>
                                               {
                                                 if( callback != null )
                                                 {
                                                   callback.ErrorHandler.Invoke( fault );
                                                 }
                                               } ) );
    }

    [TestCleanup]
    public void TearDown()
    {
      if( Backendless.UserService.CurrentUser != null )
      {
        Backendless.UserService.Logout();
      }
    }
  }
}