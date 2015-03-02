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
using System.Collections.Generic;
using Backendless.Test;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BackendlessAPI.Test.UserService.SyncTests
{
  [Ignore]
  [TestClass]
  public class TestsFrame : ITest
  {
    public Random Random = new Random();
    public List<String> UsedProperties = new List<string>();

    public const string LOGIN_KEY = "login";
    public const string EMAIL_KEY = "email";
    public const string PASSWORD_KEY = "password";
    public const string ID_KEY = "objectId";

    public BackendlessUser GetRandomNotRegisteredUser()
    {
      var timestamp = (DateTime.UtcNow.Ticks + Random.Next()).ToString();
      var result = new BackendlessUser();
      result.SetProperty( LOGIN_KEY, "bot" + timestamp );
      result.SetProperty( EMAIL_KEY, result.GetProperty( LOGIN_KEY ) + "@gmail.com" );
      result.Password = "somepass_" + timestamp;

      return result;
    }

    public BackendlessUser GetRandomRegisteredUser()
    {
      return Backendless.UserService.Register( GetRandomNotRegisteredUser() );
    }

    public BackendlessUser GetRandomLoggedInUser()
    {
      BackendlessUser user = GetRandomRegisteredUser();
      Backendless.UserService.Login( (string) user.Email, user.Password );

      return user;
    }

    [TestCleanup]
    public void TearDown()
    {
      if( Backendless.UserService.CurrentUser != null )
        Backendless.UserService.Logout();
    }
  }
}