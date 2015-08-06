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
using System.Collections;
using System.Collections.Generic;
using BackendlessAPI.Engine;
using BackendlessAPI.Exception;
using BackendlessAPI.Service;
using System.Net;
using BackendlessAPI.Caching;
using BackendlessAPI.Counters;
using BackendlessAPI.Logging;

namespace BackendlessAPI
{
  public static class Backendless
  {
    public static string DEFAULT_URL = "https://api.backendless.com";

    public static BackendlessAPI.Service.PersistenceService Persistence;
    public static BackendlessAPI.Service.PersistenceService Data;
    public static BackendlessAPI.Service.GeoService Geo;
    public static BackendlessAPI.Service.MessagingService Messaging;
    public static BackendlessAPI.Service.FileService Files;
    public static BackendlessAPI.Service.UserService UserService;
    public static BackendlessAPI.Events Events;
    public static BackendlessAPI.Caching.Cache Cache;
    public static BackendlessAPI.Counters.CounterService Counters;
    public static BackendlessAPI.Logging.LoggingService Logging;
    public static string Url { get; private set; }

    public static string AppId { get; private set; }

    public static string SecretKey { get; private set; }

    public static string VersionNum { get; private set; }

    static Backendless()
    {
      Url = DEFAULT_URL;
      ServicePointManager.ServerCertificateValidationCallback = delegate
      {
        return true;
      };
    }

    public static string getUrl()
    {
      return Url;
    }

    public static void setUrl(string url)
    {
      if (string.IsNullOrEmpty(url))
        url = DEFAULT_URL;
      Url = url;
    }

    public static void InitApp(string applicationId, string secretKey, string version)
    {
      if (string.IsNullOrEmpty(applicationId))
        throw new ArgumentNullException(ExceptionMessage.NULL_APPLICATION_ID);

      if (string.IsNullOrEmpty(secretKey))
        throw new ArgumentNullException(ExceptionMessage.NULL_SECRET_KEY);

      if (string.IsNullOrEmpty(version))
        throw new ArgumentNullException(ExceptionMessage.NULL_VERSION);

      AppId = applicationId;
      SecretKey = secretKey;
      VersionNum = version;

      Persistence = new PersistenceService();
      Data = Persistence;
      Geo = new BackendlessAPI.Service.GeoService();
      Messaging = new BackendlessAPI.Service.MessagingService();
      Files = new BackendlessAPI.Service.FileService();
      UserService = new BackendlessAPI.Service.UserService();
      Events = BackendlessAPI.Events.GetInstance();
      Cache = BackendlessAPI.Caching.Cache.GetInstance();
      Counters = BackendlessAPI.Counters.CounterService.GetInstance();
      Logging = new BackendlessAPI.Logging.LoggingService();

      HeadersManager.CleanHeaders();
    }
  }
}