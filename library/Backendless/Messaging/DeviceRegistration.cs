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
using BackendlessAPI.LitJson;

namespace BackendlessAPI.Messaging
{
  public class DeviceRegistration
  {
    [JsonProperty( "id" )]
    public string Id { get; set; }

    [JsonProperty( "channels" )]
    public List<string> Channels { get; set; }

    private DateTime? expiration;
    public DateTime? Expiration { get { return expiration; } set { expiration = value; if (expiration == null) { timestamp = 0; } else { timestamp = System.Convert.ToInt64(((DateTime)expiration - new DateTime(1970, 1, 1, 0, 0, 0)).TotalMilliseconds); }  } }

    private long? timestamp;
    [JsonProperty("expiration")]
    public long? Timestamp { get { return timestamp; } set { timestamp = value; if (expiration == null) { expiration = null; } else { double milliseconds = (double)value; expiration = new DateTime(1970, 1, 1, 0, 0, 0).AddMilliseconds(milliseconds); } } }

    [JsonProperty( "os" )]
    public string Os { get; set; }

    [JsonProperty( "osVersion" )]
    public string OsVersion { get; set; }

    [JsonProperty( "deviceToken" )]
    public string DeviceToken { get; set; }

    [JsonProperty( "registrationId" )]
    public string RegistrationId { get; set; }

    [JsonProperty( "deviceId" )]
    public string DeviceId { get; set; }

    public void AddChannel( string channel )
    {
      if( Channels == null )
        Channels = new List<string>();

      Channels.Add( channel );
    }

    public void ClearRegistration()
    {
      Id = null;
      Channels = null;
      RegistrationId = null;
      DeviceToken = null;
    }
  }
}