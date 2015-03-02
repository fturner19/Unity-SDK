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

using BackendlessAPI.LitJson;

namespace BackendlessAPI.Messaging
{
    public class SubscriptionOptions
    {
      public SubscriptionOptions()
        {
        }

        public SubscriptionOptions(string subscriberId)
        {
            this.SubscriberId = subscriberId;
        }

        public SubscriptionOptions(string subscriberId, string subtopic)
        {
            this.SubscriberId = subscriberId;
            this.Subtopic = subtopic;
        }

        public SubscriptionOptions(string subscriberId, string subtopic, string selector)
        {
            this.SubscriberId = subscriberId;
            this.Subtopic = subtopic;
            this.Selector = selector;
        }

      [JsonProperty( "subscriberId" )]
      public string SubscriberId { get; set; }

      [JsonProperty( "subtopic" )]
      public string Subtopic { get; set; }

      [JsonProperty( "selector" )]
      public string Selector { get; set; }
    }
}
