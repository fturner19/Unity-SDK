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
using BackendlessAPI.Async;
using BackendlessAPI.Exception;
using System.Collections.Generic;
using System.Net;
using BackendlessAPI.LitJson;
using System.IO;


namespace BackendlessAPI.Engine
{
  internal static class Invoker
  {
    public enum Api
    {
      USERSERVICE_REGISTER,
      USERSERVICE_UPDATE,
      USERSERVICE_LOGIN,
      USERSERVICE_LOGOUT,
      USERSERVICE_RESTOREPASSWORD,
      USERSERVICE_GETUSERROLES,
      USERSERVICE_DESCRIBEUSERCLASS,
      PERSISTENCESERVICE_CREATE,
      PERSISTENCESERVICE_UPDATE,
      PERSISTENCESERVICE_REMOVE,
      PERSISTENCESERVICE_FIND,
      PERSISTENCESERVICE_DESCRIBE,
      FILESERVICE_REMOVE,
      GEOSERVICE_ADDCATEGORY,
      GEOSERVICE_DELETECATEGORY,
      GEOSERVICE_GETCATEGORIES,
      GEOSERVICE_SAVEPOINT,
      GEOSERVICE_UPDATEPOINT,
      GEOSERVICE_GETPOINTS,
      GEOSERVICE_GETRECT,
      GEOSERVICE_RELATIVEFIND,
      MESSAGINGSERVICE_PUBLISH,
      MESSAGINGSERVICE_CANCEL,
      MESSAGINGSERVICE_SUBSCRIBE,
      MESSAGINGSERVICE_POLLMESSAGES,
      MESSAGINGSERVICE_SENDEMAIL,
      MESSAGINGSERVICE_REGISTERDEVICEONSERVER,
      MESSAGINGSERVICE_UNREGISTERDEVICEONSERVER,
      MESSAGINGSERVICE_GETREGISTRATION,
      EVENTS_DISPATCH,
      COUNTERSERVICE_RESET,
      COUNTERSERVICE_GET,
      COUNTERSERVICE_GET_INC,
      COUNTERSERVICE_INC_GET,
      COUNTERSERVICE_GET_DEC,
      COUNTERSERVICE_DEC_GET,
      COUNTERSERVICE_ADD_GET,
      COUNTERSERVICE_GET_ADD,
      COUNTERSERVICE_COM_SET,
      CACHESERVICE_PUT,
      CACHESERVICE_GET,
      CACHESERVICE_CONTAINS,
      CACHESERVICE_EXPIREIN,
      CACHESERVICE_EXPIREAT,
      CACHESERVICE_DELETE,
      UNKNOWN
    }
    public enum Method
    {
      GET,
      POST,
      PUT,
      DELETE,
      PATCH,
      UNKNOWN
    }

    private class RequestState<T>
    {
      public HttpWebRequest Request { get; set; }
      public string RequestJsonString { get; set; }
      public AsyncCallback<T> Callback { get; set; }
      public Api Api { get; set; }

      public RequestState()
      {
        Request = null;
        RequestJsonString = null;
        Callback = null;
        Api = Api.UNKNOWN;
      }
    }

    private static void GetRestApiRequestCommand(Api api, object[] args, out Method method, out string url, out Dictionary<string, string> headers)
    {
      // Common
      headers = HeadersManager.GetInstance().Headers;
      url = Backendless.Url + "/" + Backendless.VersionNum + "/";

      switch (api)
      {
        case Api.USERSERVICE_REGISTER:
          method = Method.POST;
          url += "users/register";
          break;
        case Api.USERSERVICE_UPDATE:
          method = Method.PUT;
          url += "users/";
          url += args[1]; // + <user-id>
          break;
        case Api.USERSERVICE_LOGIN:
          method = Method.POST;
          url += "users/login";
          break;
        case Api.USERSERVICE_LOGOUT:
          method = Method.GET;
          url += "users/logout";
          break;
        case Api.USERSERVICE_RESTOREPASSWORD:
          method = Method.GET;
          url += "users/restorepassword/";
          url += args[1]; // + <user-identity-property>
          break;
        case Api.USERSERVICE_GETUSERROLES:
          method = Method.GET;
          url += "users/userroles";
          break;
        case Api.USERSERVICE_DESCRIBEUSERCLASS:
          method = Method.GET;
          url += "users/userclassprops";
          break;
        case Api.PERSISTENCESERVICE_CREATE:
          method = Method.POST;
          url += "data/";
          url += args[1]; // + <table-name>
          break;
        case Api.PERSISTENCESERVICE_UPDATE:
          method = Method.PUT;
          url += "data/";
          url += args[1] + "/" + args[2]; // + <table-name>/<object-id>
          break;
        case Api.PERSISTENCESERVICE_REMOVE:
          method = Method.DELETE;
          url += "data/";
          url += args[1] + "/" + args[2]; // + <table-name>/<object-id>
          break;
        case Api.PERSISTENCESERVICE_FIND:
          method = Method.GET;
          url += "data/";
          url += args[1]; // + <table-name>
          if (args[2] != null)
            url += "/" + args[2]; // + /<object-id> or first or last
          if (args[3] != null)
            url += "?" + args[3];
          break;
        case Api.PERSISTENCESERVICE_DESCRIBE:
          method = Method.GET;
          url += "data/";
          url += args[1] + "/properties"; // + <table-name>/properties
          break;
        case Api.FILESERVICE_REMOVE:
          method = Method.DELETE;
          url += "files/";
          url += args[1]; // + <path>/<filename> or <path>
          break;
        case Api.GEOSERVICE_ADDCATEGORY:
          method = Method.PUT;
          url += "geo/categories/";
          url += args[1]; // + <categoryName>
          break;
        case Api.GEOSERVICE_DELETECATEGORY:
          method = Method.DELETE;
          url += "geo/categories/";
          url += args[1]; // + <categoryName>
          break;
        case Api.GEOSERVICE_GETCATEGORIES:
          method = Method.GET;
          url += "geo/categories";
          break;
        case Api.GEOSERVICE_SAVEPOINT:
          method = Method.PUT;
          url += "geo/points";
          url += args[1];
          break;
        case Api.GEOSERVICE_UPDATEPOINT:
          method = Method.PATCH;
          url += "geo/points";
          url += args[1];
          break;
        case Api.GEOSERVICE_GETPOINTS:
          method = Method.GET;
          url += "geo/points";
          url += args[1];
          break;
        case Api.GEOSERVICE_GETRECT:
          method = Method.GET;
          url += "geo/rect";
          url += args[1];
          break;
        case Api.GEOSERVICE_RELATIVEFIND:
          method = Method.GET;
          url += "geo/relative/points";
          url += args[1];
          break;
        case Api.MESSAGINGSERVICE_PUBLISH:
          method = Method.POST;
          url += "messaging/";
          url += args[1]; // + <channel-name>
          break;
        case Api.MESSAGINGSERVICE_CANCEL:
          method = Method.DELETE;
          url += "messaging/";
          url += args[1]; // + <message-id>
          break;
        case Api.MESSAGINGSERVICE_SUBSCRIBE:
          method = Method.POST;
          url += "messaging/";
          url += args[1] + "/subscribe"; // + <channel-name>/subscribe
          break;
        case Api.MESSAGINGSERVICE_POLLMESSAGES:
          method = Method.GET;
          url += "messaging/";
          url += args[1] + "/" + args[2]; // + <channel-name>/<subscription-id>
          break;
        case Api.MESSAGINGSERVICE_SENDEMAIL:
          method = Method.POST;
          url += "messaging/email";
          break;
        case Api.MESSAGINGSERVICE_REGISTERDEVICEONSERVER:
          method = Method.POST;
          url += "messaging/registrations";
          break;
        case Api.MESSAGINGSERVICE_GETREGISTRATION:
          method = Method.GET;
          url += "messaging/registrations/";
          url += args[1]; // + <device-id>
          break;
        case Api.MESSAGINGSERVICE_UNREGISTERDEVICEONSERVER:
          method = Method.DELETE;
          url += "messaging/registrations/";
          url += args[1]; // + <device-id>
          break;
        case Api.EVENTS_DISPATCH:
          method = Method.POST;
          url += "servercode/events/";
          url += args[1]; // + <event name>
          break;
        case Api.COUNTERSERVICE_RESET:
          method = Method.PUT;
          url += "counters/";
          url += args[1] + "/reset"; // + <counterName>/reset
          break;
        case Api.COUNTERSERVICE_GET:
          method = Method.GET;
          url += "counters/";
          url += args[1]; // + <counterName>
          break;
        case Api.COUNTERSERVICE_GET_INC:
          method = Method.PUT;
          url += "counters/";
          url += args[1] + "/get/increment"; // + <counterName>/get/increment
          break;
        case Api.COUNTERSERVICE_INC_GET:
          method = Method.PUT;
          url += "counters/";
          url += args[1] + "/increment/get"; // + <counterName>/increment/get
          break;
        case Api.COUNTERSERVICE_GET_DEC:
          method = Method.PUT;
          url += "counters/";
          url += args[1] + "/get/decrement"; // + <counterName>/get/decrement
          break;
        case Api.COUNTERSERVICE_DEC_GET:
          method = Method.PUT;
          url += "counters/";
          url += args[1] + "/decrement/get"; // + <counterName>/decrement/get
          break;
        case Api.COUNTERSERVICE_ADD_GET:
          method = Method.PUT;
          url += "counters/";
          url += args[1] + "/incrementby/get?value=" + args[2]; // + <counterName>/incrementby/get?value=<value>
          break;
        case Api.COUNTERSERVICE_GET_ADD:
          method = Method.PUT;
          url += "counters/";
          url += args[1] + "/get/incrementby?value=" + args[2]; // + <counterName>/get/incrementby?value=<value>
          break;
        case Api.COUNTERSERVICE_COM_SET:
          method = Method.PUT;
          url += "counters/";
          url += args[1] + "/get/compareandset?expected=" + args[2] + "&updatedvalue=" +  args[3]; // + <counterName>/get/compareandset?expected=<expected>&updatedvalue=<updated>
          break;
        case Api.CACHESERVICE_PUT:
          method = Method.PUT;
          url += "cache/";
          url += args[1];
          if((int)args[2] != 0)
            url += "?timeout=" + args[2]; // + <key>?timeout=<timeToLive>
          break;
        case Api.CACHESERVICE_GET:
          method = Method.GET;
          url += "cache/";
          url += args[1]; // + <key>
          break;
        case Api.CACHESERVICE_CONTAINS:
          method = Method.GET;
          url += "cache/";
          url += args[1] + "/check"; // + <key>/check
          break;
        case Api.CACHESERVICE_EXPIREIN:
          method = Method.PUT;
          url += "cache/";
          url += args[1] + "/expireIn?timeout=" + args[2]; // + <key>/expireIn?timeout=<seconds>
          break;
        case Api.CACHESERVICE_EXPIREAT:
          method = Method.PUT;
          url += "cache/";
          url += args[1] + "/expireAt?timestamp=" + args[2]; // + <key>/expireAt?timestamp=<timestamp>
          break;
        case Api.CACHESERVICE_DELETE:
          method = Method.DELETE;
          url += "cache/";
          url += args[1]; // + <key>
          break;
        default:
          throw new BackendlessException("GetRequestData() bad parameter 'api'");
      }
    }

    public static T InvokeSync<T>(Api api, object[] args)
    {
      Method method = Method.UNKNOWN;
      string url = null;
      Dictionary<string, string> headers = null;
      object data = null;
      if (args != null && args.Length > 0)
        data = args[0];

      try
      {
        GetRestApiRequestCommand(api, args, out method, out url, out headers);
        return InvokeSync<T>(api, method, url, headers, data);
      }
      catch (BackendlessException ex)
      {
        throw new BackendlessException(ex.BackendlessFault);
      }
    }

    public static T InvokeSync<T>(Api api, Method method, string url, Dictionary<string, string> headers, object data)
    {
      try
      {
        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);

        // Set method
        request.Method = method.ToString();

        // Set Headers
        foreach (KeyValuePair<string, string> header in headers)
          request.Headers.Add(header.Key, header.Value);

        // Convert Data to Json
        if (method == Method.GET || data == null)
        {
        }
        else
        {
          string requestJsonString = JsonMapper.ToJson(data);

          // Set ContentType, ContentLength
          request.ContentType = "application/json";
          //request.ContentLength = requestJsonString.Length;

          // Request Data
          using (Stream requestStream = request.GetRequestStream())
          {
            using (StreamWriter requestWriter = new StreamWriter(requestStream, System.Text.Encoding.UTF8))
              requestWriter.Write(requestJsonString);
          }
        }

        // Response Data
        HttpWebResponse response = (HttpWebResponse)request.GetResponse();
        using (Stream responseStream = response.GetResponseStream())
        {
          using (StreamReader responseReader = new StreamReader(responseStream, System.Text.Encoding.UTF8))
          {
            T result = default(T);
            string responseJsonString = responseReader.ReadToEnd();
            if ((api >= Api.COUNTERSERVICE_GET && api <= Api.COUNTERSERVICE_COM_SET) || api == Api.CACHESERVICE_CONTAINS)
            {
              result = (T)Convert.ChangeType(responseJsonString, typeof(T));
            }
            else
            {
              if (string.IsNullOrEmpty(responseJsonString) == false)
                result = JsonMapper.ToObject<T>(responseJsonString);
            }
            return result;
          }
        }
      }
      catch (WebException ex)
      {
        using (Stream responseStream = ex.Response.GetResponseStream())
        {
          using (StreamReader responseReader = new StreamReader(responseStream, System.Text.Encoding.UTF8))
          {
            string responseJsonString = responseReader.ReadToEnd();
            BackendlessFault fault = null;

            try
            {
              JsonData errorResponse = JsonMapper.ToObject(responseJsonString);
              int code = (int)errorResponse["code"];
              string message = (string)errorResponse["message"];

              fault = new BackendlessFault(code.ToString(), message, null);
            }
            catch (System.Exception)
            {
              fault = new BackendlessFault(ex);
            }

            throw new BackendlessException(fault);
          }
        }
      }
      catch (System.Exception ex)
      {
        BackendlessFault backendlessFault = new BackendlessFault(ex);
        throw new BackendlessException(backendlessFault);
      }
    }

    public static void InvokeAsync<T>(Api api, object[] args, AsyncCallback<T> callback)
    {
      Method method = Method.UNKNOWN;
      string url = null;
      Dictionary<string, string> headers = null;
      object data = null;
      if (args != null && args.Length > 0)
        data = args[0];

      try
      {
        GetRestApiRequestCommand(api, args, out method, out url, out headers);
        InvokeAsync<T>(api, method, url, headers, data, callback);
      }
      catch (BackendlessException ex)
      {
        BackendlessFault backendlessFault = new BackendlessFault(ex);
        if (callback != null)
          callback.ErrorHandler.Invoke(backendlessFault);
        else
          throw new BackendlessException(backendlessFault);
      }
    }

    public static void InvokeAsync<T>(Api api, Method method, string url, Dictionary<string, string> headers, object data, AsyncCallback<T> callback)
    {
      RequestState<T> requestState = new RequestState<T>();

      try
      {
        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);

        // Set method
        request.Method = method.ToString();

        // Set Headers
        foreach (KeyValuePair<string, string> header in headers)
          request.Headers.Add(header.Key, header.Value);

        requestState.Request = request;
        requestState.Callback = callback;
        requestState.Api = api;

        if (method == Method.GET || data == null)
        {
          request.BeginGetResponse(new AsyncCallback(GetResponseCallback<T>), requestState);
        }
        else
        {
          // Convert Data to Json
          string requestJsonString = JsonMapper.ToJson(data);

          // Set ContentType, ContentLength
          request.ContentType = "application/json";
          //request.ContentLength = requestJsonString.Length;

          requestState.RequestJsonString = requestJsonString;

          request.BeginGetRequestStream(new System.AsyncCallback(GetRequestStreamCallback<T>), requestState);
        }
      }
      catch (System.Exception ex)
      {
        BackendlessFault backendlessFault = new BackendlessFault(ex);
        if (requestState.Callback != null)
          requestState.Callback.ErrorHandler.Invoke(backendlessFault);
        else
          throw new BackendlessException(backendlessFault);
      }
    }

    private static void GetRequestStreamCallback<T>(IAsyncResult asynchronousResult)
    {
      RequestState<T> requestState = (RequestState<T>)asynchronousResult.AsyncState;
      try
      {
        HttpWebRequest request = requestState.Request;
        string requestJsonString = requestState.RequestJsonString;

        // Request Data
        using (Stream requestStream = request.EndGetRequestStream(asynchronousResult))
        {
          using (StreamWriter requestWriter = new StreamWriter(requestStream, System.Text.Encoding.UTF8))
            requestWriter.Write(requestJsonString);
        }

        request.BeginGetResponse(new AsyncCallback(GetResponseCallback<T>), requestState);
      }
      catch (System.Exception ex)
      {
        BackendlessFault backendlessFault = new BackendlessFault(ex.Message);
        if (requestState.Callback != null)
          requestState.Callback.ErrorHandler.Invoke(backendlessFault);
      }
    }

    private static void GetResponseCallback<T>(IAsyncResult asynchronousResult)
    {
      RequestState<T> requestState = (RequestState<T>)asynchronousResult.AsyncState;
      T result = default(T);
      try
      {
        HttpWebRequest request = requestState.Request;

        HttpWebResponse response = (HttpWebResponse)request.EndGetResponse(asynchronousResult);
        using (Stream responseStream = response.GetResponseStream())
        {
          using (StreamReader responseReader = new StreamReader(responseStream, System.Text.Encoding.UTF8))
          {
            string responseJsonString = responseReader.ReadToEnd();
            if ((requestState.Api >= Api.COUNTERSERVICE_GET && requestState.Api <= Api.COUNTERSERVICE_COM_SET) || requestState.Api == Api.CACHESERVICE_CONTAINS)
            {
              result = (T)Convert.ChangeType(responseJsonString, typeof(T));
            }
            else
            {
              if (string.IsNullOrEmpty(responseJsonString) == false)
                result = JsonMapper.ToObject<T>(responseJsonString);
            }
          }
        }

        response.Close();

        if (requestState.Callback != null)
          requestState.Callback.ResponseHandler.Invoke(result);
      }
      catch (WebException ex)
      {
        using (Stream responseStream = ex.Response.GetResponseStream())
        {
          using (StreamReader responseReader = new StreamReader(responseStream, System.Text.Encoding.UTF8))
          {
            string responseJsonString = responseReader.ReadToEnd();
            BackendlessFault fault = null;
            try
            {
              JsonData errorResponse = JsonMapper.ToObject(responseJsonString);
              int code = (int)errorResponse["code"];
              string message = (string)errorResponse["message"];

              fault = new BackendlessFault(code.ToString(), message, null);
            }
            catch (System.Exception)
            {
              fault = new BackendlessFault(ex);
            }

            if (requestState.Callback != null)
              requestState.Callback.ErrorHandler.Invoke(fault);
          }
        }
      }
      catch (System.Exception ex)
      {
        BackendlessFault backendlessFault = new BackendlessFault(ex);
        if (requestState.Callback != null)
          requestState.Callback.ErrorHandler.Invoke(backendlessFault);
      }
    }
  }
}