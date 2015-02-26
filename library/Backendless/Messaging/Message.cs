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

using System.Collections.Generic;
using BackendlessAPI.LitJson;

namespace BackendlessAPI.Messaging
{
  public class Message
  {
    private Dictionary<string, string> _headers;

    [JsonProperty( "messageId" )]
    public string MessageId { get; set; }

    [JsonProperty( "data" )]
    public object Data { get; set; }

    [JsonProperty( "headers" )]
    public Dictionary<string, string> Headers
    {
      get { return _headers ?? (_headers = new Dictionary<string, string>()); }
      set { _headers = value; }
    }

    [JsonProperty( "publisherId" )]
    public string PublisherId { get; set; }

    [JsonProperty( "timestamp" )]
    public long Timestamp { get; set; }
  }
}