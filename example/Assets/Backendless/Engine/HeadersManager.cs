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

using System.Collections.Generic;

namespace BackendlessAPI.Engine
{
  internal class HeadersEnum
  {
    public static readonly HeadersEnum USER_TOKEN_KEY = new HeadersEnum("user-token");
    public static readonly HeadersEnum LOGGED_IN_KEY = new HeadersEnum("logged-in");
    public static readonly HeadersEnum SESSION_TIME_OUT_KEY = new HeadersEnum("session-time-out");
    public static readonly HeadersEnum APP_ID_NAME = new HeadersEnum("application-id");
    public static readonly HeadersEnum SECRET_KEY_NAME = new HeadersEnum("secret-key");
    public static readonly HeadersEnum APP_TYPE_NAME = new HeadersEnum("application-type");
    public static readonly HeadersEnum API_VERSION = new HeadersEnum("api-version");

    public static IEnumerable<HeadersEnum> Values
    {
      get
      {
        yield return USER_TOKEN_KEY;
        yield return LOGGED_IN_KEY;
        yield return SESSION_TIME_OUT_KEY;
        yield return APP_ID_NAME;
        yield return SECRET_KEY_NAME;
        yield return APP_TYPE_NAME;
        yield return API_VERSION;
      }
    }

    private readonly string name;

    HeadersEnum(string name)
    {
      this.name = name;
    }

    public string Header { get { return name; } }

    public override string ToString()
    {
      return name;
    }
  }

  internal class HeadersManager
  {
    private Dictionary<string, string> headers = new Dictionary<string, string>();
    private static object headersLock = new object();
    private volatile static HeadersManager _instance = null;

    private HeadersManager()
    {
    }

    public static HeadersManager GetInstance()
    {
      if (_instance == null)
      {
        lock (headersLock)
        {
          if (_instance == null)
          {
            _instance = new HeadersManager();
            _instance.AddHeader(HeadersEnum.APP_ID_NAME, Backendless.AppId);
            _instance.AddHeader(HeadersEnum.SECRET_KEY_NAME, Backendless.SecretKey);
            _instance.AddHeader(HeadersEnum.APP_TYPE_NAME, "REST");
          }
        }
      }

      return _instance;
    }

    public void AddHeader(HeadersEnum headersEnum, string value)
    {
      lock (headersLock)
      {
        headers.Remove(headersEnum.Header);
        headers.Add(headersEnum.Header, value);
      }
    }

    public void RemoveHeader(HeadersEnum headersEnum)
    {
      lock (headersLock)
      {
        headers.Remove(headersEnum.Header);
      }
    }

    public Dictionary<string, string> Headers
    {
      get { return headers; }
      set
      {
        lock (headersLock)
        {
          foreach (var header in headers)
          {
            this.headers.Add(header.Key, header.Value);
          }
        }
      }
    }

    public static void CleanHeaders()
    {
      lock (typeof(HeadersManager))
      {
        _instance = null;
      }
    }
  }
}
