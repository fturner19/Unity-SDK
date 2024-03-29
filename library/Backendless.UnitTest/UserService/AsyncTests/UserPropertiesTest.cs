﻿/*
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
using BackendlessAPI.Property;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BackendlessAPI.Test.UserService.AsyncTests
{
  [TestClass]
  public class UserPropertiesTest : TestsFrame
  {
    [TestMethod]
    public void TestDescribeUserProperties()
    {
      RunAndAwait( () =>
        {
          Backendless.InitApp(Defaults.TEST_APP_ID, Defaults.TEST_SECRET_KEY, Defaults.TEST_VERSION);
          BackendlessUser user = GetRandomNotRegisteredUser();
          const string propertyKeyAsync = "property_key_Async";
          user.SetProperty(propertyKeyAsync, "porperty_value_" + Random.Next());
          Backendless.UserService.Register( user,
                                            new ResponseCallback<BackendlessUser>( this )
                                              {
                                                ResponseHandler =
                                                  response => {
                                                  Backendless.UserService.Login((string) user.Email, user.Password);
                                                  Backendless.UserService.DescribeUserClass(
                                                    new ResponseCallback<List<UserProperty>>( this )
                                                      {
                                                        ResponseHandler = userProperties =>
                                                          {
                                                            Assert.IsNotNull( userProperties,
                                                                              "Server returned null user properties" );
                                                            Assert.IsTrue( userProperties.Count != 0,
                                                                           "Server returned empty user properties" );

                                                            var properties = new List<string>
                                                              {
                                                                propertyKeyAsync,
                                                                ID_KEY,
                                                                LOGIN_KEY,
                                                                PASSWORD_KEY,
                                                                EMAIL_KEY
                                                              };

                                                            foreach( UserProperty userProperty in userProperties )
                                                            {
                                                              Assert.IsNotNull( userProperty, "User property was null" );
                                                              Assert.IsNotNull( userProperty.Type,
                                                                                "User properties type was null" );
                                                            }
                                                            foreach (string property in properties)
                                                            {
                                                              bool isFind = false;
                                                              foreach (UserProperty userProperty in userProperties)
                                                              {
                                                                if (userProperty.Name.Equals(property) == true)
                                                                {
                                                                  isFind = true;
                                                                  break;
                                                                }
                                                              }
                                                              Assert.IsTrue(isFind,
                                                                             "User properties contained unexpected property " + property);
                                                            }

                                                            CountDown();
                                                          }
                                                      } ); }
                                              } );
        } );
    }
  }
}