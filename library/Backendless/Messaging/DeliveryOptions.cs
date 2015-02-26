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

namespace BackendlessAPI.Messaging
{
  public class DeliveryOptions
  {
    public const int IOS = 1;
    public const int ANDROID = 1 << 1;
    public const int WP = 1 << 2;
    public const int ALL = IOS | ANDROID | WP;

    private PushPolicyEnum _pushPolicy = PushPolicyEnum.ALSO;

    // accepts a mask value used by Backendless to route the message
    // to the registered devices with the specified operating system. 
    // The mask value may consist of the following values:
    // DeliveryOptions.IOS, DeliveryOptions.ANDROID, DeliveryOptions.WP and PushBroadcast.ALL
    public int PushBroadcast { get; set; }

    // configures a list of registered device IDs to deliver the message to
    public List<string> PushSinglecast { get; set; }

    // sets the time when the message should be published
    public DateTime? PublishAt { get; set; }

    // sets the interval as the number of milliseconds repeated message publications.
    // When a value is set Backendless re-publishes the message with the interval.
    public long RepeatEvery { get; set; }

    // sets the time when the message republishing configured with "repeatEvery" should stop
    public DateTime? RepeatExpiresAt { get; set; }

    // Controls if the published messages are distributed as a push notification ONLY or
    // both push notification and ALSO as regular publish/subscribe messages.
    // There are two values ONLY and ALSO which correspond to the described scenarios.
    public PushPolicyEnum PushPolicy
    {
      get { return _pushPolicy; }
      set { _pushPolicy = value; }
    }
  }
}
