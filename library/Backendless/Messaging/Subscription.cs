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
using System.Threading;
using System.Collections.Generic;
using BackendlessAPI.Async;
using BackendlessAPI.Exception;
using BackendlessAPI.LitJson;

namespace BackendlessAPI.Messaging
{
  public class Subscription
  {
    private Timer _timer;

    private int _pollingInterval = 1000;

    public Subscription()
    {
    }

    public Subscription( int pollingInterval )
    {
      this._pollingInterval = pollingInterval;
    }

    // returns a unique subscription identifier (assigned by Backendless)
    [JsonProperty("subscriptionId")]
    public string SubscriptionId { get; set; }

    // returns the name of the channel the subscription gets messages from
    [JsonProperty("channelName")]
    public string ChannelName { get; set; }

    // controls how frequently the client polls for messages
    [JsonProperty("pollingInterval")]
    public int PollingInterval
    {
      get { return this._pollingInterval; }
      set { this._pollingInterval = value; }
    }

    // cancels the subscription
    public bool CancelSubscription()
    {
      if( _timer != null )
      {
        _timer.Change( Timeout.Infinite, Timeout.Infinite );
        _timer = null;
      }

      SubscriptionId = null;
      return true;
    }

    // suspends the subscription (the client stops receiving new messages)
    public void PauseSubscription()
    {
      if( _timer != null )
      {
        _timer.Change( Timeout.Infinite, Timeout.Infinite );
      }
    }

    // resumes the subscription
    public void ResumeSubscription()
    {
      if( SubscriptionId == null || ChannelName == null || _timer == null )
        throw new ArgumentNullException( ExceptionMessage.WRONG_SUBSCRIPTION_STATE );

      _timer.Change( 0, _pollingInterval );
    }

    public void OnSubscribe( AsyncCallback<List<Message>> callback )
    {
      _timer = new Timer( c =>
      {

        var message = Backendless.Messaging.PollMessages( ChannelName, SubscriptionId );
        if( message.Count == 0 )
          return;

        var callback1 = (AsyncCallback<List<Message>>) c;
        callback1.ResponseHandler.Invoke( message );
      }, callback, 0, _pollingInterval );
    }
  }
}
