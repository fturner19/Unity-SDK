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
using BackendlessAPI.Engine;
using BackendlessAPI.LitJson;

namespace BackendlessAPI
{
  public class BackendlessUser
  {
    public const string PASSWORD_KEY = "password";
    public const string EMAIL_KEY = "email";
    private const string ID_KEY = "objectId";

    private Dictionary<string, object> _properties = new Dictionary<string, object>();

    public BackendlessUser()
    {
    }

    internal BackendlessUser( Dictionary<string, object> properties )
    {
      _properties = properties;
    }

    [JsonProperty("properties")]
    public Dictionary<string, object> Properties
    {
      get { return _properties; }
      set { _properties = value; }
    }

    public string Password
    {
      get { return Properties.ContainsKey( PASSWORD_KEY ) ? (string) Properties[PASSWORD_KEY] : null; }
      set
      {
        if( Properties.ContainsKey( PASSWORD_KEY ) )
          SetProperty( PASSWORD_KEY, value );
        else
          AddProperty( PASSWORD_KEY, value );
      }
    }

    public string Email
    {
      get { return Properties.ContainsKey( EMAIL_KEY ) ? (string) Properties[EMAIL_KEY] : null; }
      set
      {
        if( Properties.ContainsKey( EMAIL_KEY ) )
          SetProperty( EMAIL_KEY, value );
        else
          AddProperty( EMAIL_KEY, value );
      }
    }

    public string UserId
    {
      get { return Properties.ContainsKey( ID_KEY ) ? (string) Properties[ID_KEY] : null; }
      set
      {
        if( Properties.ContainsKey( ID_KEY ) )
          SetProperty( ID_KEY, value );
        else
          AddProperty( ID_KEY, value );
      }
    }

    public void PutProperties( Dictionary<string, object> dictionary )
    {
      foreach( var keyValuePair in dictionary )
      {
        if( keyValuePair.Key.Equals( HeadersEnum.USER_TOKEN_KEY.ToString() ) )
          continue;

        if( Properties.ContainsKey( keyValuePair.Key ) )
          SetProperty( keyValuePair.Key, keyValuePair.Value );
        else
          AddProperty( keyValuePair.Key, keyValuePair.Value );
      }
    }

    public void AddProperty( string key, object value )
    {
      Properties.Add( key, value );
    }

    public void SetProperty( string key, object value )
    {
      Properties[key] = value;
    }

    public object GetProperty( string key )
    {
      return Properties[key];
    }
  }
}