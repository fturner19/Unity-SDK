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
using BackendlessAPI.Async;
using BackendlessAPI.Exception;

namespace BackendlessAPI
{
  public class Events
  {
    private static readonly Events instance = new Events();

    public static Events GetInstance()
    {
      return instance;
    }

    // synchronous method
    public IDictionary Dispatch( String eventName, IDictionary eventArgs )
    {
      return Invoker.InvokeSync<Dictionary<string, object>>(Invoker.Api.EVENTS_DISPATCH, new object[] { eventArgs, eventName });
    }

    // asynchronous method
    public void Dispatch( String eventName, IDictionary eventArgs, AsyncCallback<IDictionary> callback )
    {
      var responder = new AsyncCallback<Dictionary<string, object>>(r =>
      {
        if (callback != null)
          callback.ResponseHandler.Invoke(r);
      }, f =>
      {
        if (callback != null)
          callback.ErrorHandler.Invoke(f);
        else
          throw new BackendlessException(f);
      });
      Invoker.InvokeAsync<Dictionary<string, object>>(Invoker.Api.EVENTS_DISPATCH, new object[] { eventArgs, eventName }, responder);
    }

    // synchronous method
    public T Dispatch<T>(String eventName, IDictionary eventArgs)
    {
      return Invoker.InvokeSync<T>(Invoker.Api.EVENTS_DISPATCH, new object[] { eventArgs, eventName });
    }

    // asynchronous method
    public void Dispatch<T>(String eventName, IDictionary eventArgs, AsyncCallback<T> callback)
    {
      var responder = new AsyncCallback<T>(r =>
      {
        if (callback != null)
          callback.ResponseHandler.Invoke(r);
      }, f =>
      {
        if (callback != null)
          callback.ErrorHandler.Invoke(f);
        else
          throw new BackendlessException(f);
      });
      Invoker.InvokeAsync<T>(Invoker.Api.EVENTS_DISPATCH, new object[] { eventArgs, eventName }, responder);
    }
  }
}
