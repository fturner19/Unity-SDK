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
using System.Linq;
using System.Text;
using BackendlessAPI.Async;
using BackendlessAPI.Messaging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BackendlessAPI.Test.MessagingService.SyncTests
{
  [TestClass]
  public class SubscriptionTest : TestsFrame
  {
    [TestMethod]
    public void TestBasicMessageSubscription()
    {
      SetLatch();
      SetMessage();

      subscription = Backendless.Messaging.Subscribe( TEST_CHANNEL,
                                                      new ResponseCallback<List<Message>>( this )
                                                        {
                                                          ResponseHandler = messages =>
                                                            {
                                                              try
                                                              {
                                                                Assert.IsNotNull( messages,
                                                                                  "Server returned a null object instead of messages list" );
                                                                Assert.IsFalse( messages.Count == 0,
                                                                                "Server returned an empty messages list" );

                                                                foreach( Message resultMessage in messages )
                                                                {
                                                                  if(
                                                                    resultMessage.MessageId.Equals(
                                                                      messageStatus.MessageId ) )
                                                                  {
                                                                    Assert.AreEqual( message, resultMessage.Data,
                                                                                     "Server returned a message with a wrong message data" );
                                                                    Assert.AreEqual( messageStatus.MessageId,
                                                                                     resultMessage.MessageId,
                                                                                     "Server returned a message with a wrong messageId" );

                                                                    latch.Signal();
                                                                  }
                                                                }
                                                              }
                                                              catch( System.Exception e )
                                                              {
                                                                FailCountDownWith( e );
                                                              }
                                                            }
                                                        } );

      messageStatus = Backendless.Messaging.Publish( message, TEST_CHANNEL );

      Assert.IsNotNull( messageStatus.MessageId, "Server didn't set a messageId for the message" );
      Assert.IsTrue( messageStatus.Status.Equals( PublishStatusEnum.PUBLISHED ),
                     "Message status was not set as published" );

      Await();
      CheckResult();
    }

    [TestMethod]
    public void TestBasicMessageSubscriptionWithSubtopic()
    {
      SetLatch();
      SetMessage();

      string subtopic = "sub" + Random.Next();

      SubscriptionOptions subscriptionOptions = new SubscriptionOptions();
      subscriptionOptions.Subtopic = subtopic;
      subscription = Backendless.Messaging.Subscribe( TEST_CHANNEL,
                                                      new ResponseCallback<List<Message>>( this )
                                                        {
                                                          ResponseHandler = messages =>
                                                            {
                                                              try
                                                              {
                                                                Assert.IsNotNull( messages,
                                                                                  "Server returned a null object instead of messages list" );
                                                                Assert.IsFalse( messages.Count == 0,
                                                                                "Server returned an empty messages list" );

                                                                foreach( Message resultMessage in messages )
                                                                {
                                                                  if(
                                                                    resultMessage.MessageId.Equals(
                                                                      messageStatus.MessageId ) )
                                                                  {
                                                                    Assert.AreEqual( message, resultMessage.Data,
                                                                                     "Server returned a message with a wrong message data" );
                                                                    Assert.AreEqual( messageStatus.MessageId,
                                                                                     resultMessage.MessageId,
                                                                                     "Server returned a message with a wrong messageId" );

                                                                    latch.Signal();
                                                                  }
                                                                }
                                                              }
                                                              catch( System.Exception t )
                                                              {
                                                                FailCountDownWith( t );
                                                              }
                                                            }
                                                        }, subscriptionOptions );

      var publishOptions = new PublishOptions();
      publishOptions.Subtopic = subtopic;

      messageStatus = Backendless.Messaging.Publish( message, TEST_CHANNEL, publishOptions );

      Assert.IsNotNull( messageStatus.MessageId, "Server didn't set a messageId for the message" );
      Assert.IsTrue( messageStatus.Status.Equals( PublishStatusEnum.PUBLISHED ),
                     "Message status was not set as published" );

      Await();
      CheckResult();
    }

    [TestMethod]
    public void TestBasicMessageSubscriptionWithSelector()
    {
      SetLatch();
      SetMessage();

      string headerKey = "header_" + Random.Next();
      string headerValue = "someValue";
      headers = new Dictionary<string, string>();
      headers.Add( headerKey, headerValue );

      var subscriptionOptions = new SubscriptionOptions();
      subscriptionOptions.Selector = headerKey + "='" + headerValue + "'";

      subscription = Backendless.Messaging.Subscribe( TEST_CHANNEL,
                                                      new ResponseCallback<List<Message>>( this )
                                                        {
                                                          ResponseHandler = messages =>
                                                            {
                                                              try
                                                              {
                                                                Assert.IsNotNull( messages,
                                                                                  "Server returned a null object instead of messages list" );
                                                                Assert.IsFalse( messages.Count == 0,
                                                                                "Server returned an empty messages list" );

                                                                foreach( Message resultMessage in messages )
                                                                {
                                                                  if(
                                                                    resultMessage.MessageId.Equals(
                                                                      messageStatus.MessageId ) )
                                                                  {
                                                                    Assert.AreEqual( message, resultMessage.Data,
                                                                                     "Server returned a message with a wrong message data" );

                                                                    Assert.IsTrue(
                                                                      resultMessage.Headers.ContainsKey( headerKey ),
                                                                      "Server returned a message with wrong headers" );

                                                                    Assert.AreEqual( headerValue,
                                                                                     resultMessage.Headers[headerKey],
                                                                                     "Server returned a message with wrong headers" );

                                                                    Assert.AreEqual( messageStatus.MessageId,
                                                                                     resultMessage.MessageId,
                                                                                     "Server returned a message with a wrong messageId" );

                                                                    latch.Signal();
                                                                  }
                                                                }
                                                              }
                                                              catch( System.Exception t )
                                                              {
                                                                FailCountDownWith( t );
                                                              }
                                                            }
                                                        }, subscriptionOptions );

      var publishOptions = new PublishOptions {Headers = headers};

      messageStatus = Backendless.Messaging.Publish( message, TEST_CHANNEL, publishOptions );

      Assert.IsNotNull( messageStatus.MessageId, "Server didn't set a messageId for the message" );
      Assert.IsTrue( messageStatus.Status.Equals( PublishStatusEnum.PUBLISHED ),
                     "Message status was not set as published" );

      Await();
      CheckResult();
    }

    [TestMethod]
    public void TestMessageSubscriptionForUnknownChannel()
    {
      try
      {
        Backendless.Messaging.Subscribe( GetRandomstringMessage(), null );
        Assert.Fail("UnknownChannel fail");
      }
      catch( System.Exception /* e */ )
      {
      }
    }
  }
}