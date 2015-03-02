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
using System.Collections.Generic;
using System.Text;
using BackendlessAPI.Async;
using BackendlessAPI.Exception;
using BackendlessAPI.Engine;

namespace BackendlessAPI.Counters
{
  public class CounterService
  {
    private static readonly CounterService instance = new CounterService();

    public static CounterService GetInstance()
    {
      return instance;
    }

    private CounterService()
    {
    }

    public IAtomic<T> Of<T>( String counterName )
    {
      return new AtomicImpl<T>( counterName );
    }

    #region RESET
    public void Reset( String counterName )
    {
      Invoker.InvokeSync<object>(Invoker.Api.COUNTERSERVICE_RESET, new object[] { null, counterName });
    }

    public void Reset( String counterName, AsyncCallback<Object> callback )
    {
      Invoker.InvokeAsync<object>(Invoker.Api.COUNTERSERVICE_RESET, new object[] { null, counterName }, callback);
    }
    #endregion

    #region GET
    public int Get( String counterName )
    {
      return Get<int>(counterName);
    }

    public T Get<T>( String counterName )
    {
      string stringvalue = Invoker.InvokeSync<string>(Invoker.Api.COUNTERSERVICE_GET, new object[] { null, counterName });
      return (T)Convert.ChangeType(stringvalue, typeof(T));
    }

    public void Get<T>( String counterName, AsyncCallback<T> callback )
    {
      var responder = new AsyncCallback<string>(r =>
      {
        if (callback != null)
        {
          callback.ResponseHandler.Invoke((T)Convert.ChangeType(r, typeof(T)));
        }
      }, f =>
      {
        if (callback != null)
          callback.ErrorHandler.Invoke(f);
        else
          throw new BackendlessException(f);
      });

      Invoker.InvokeAsync<string>(Invoker.Api.COUNTERSERVICE_GET, new object[] { null, counterName }, responder);
    }
    #endregion

    #region GET_AND_INC
    public int GetAndIncrement( String counterName )
    {
      return GetAndIncrement<int>(counterName);
    }

    public T GetAndIncrement<T>( String counterName )
    {
      string stringvalue = Invoker.InvokeSync<string>(Invoker.Api.COUNTERSERVICE_GET_INC, new object[] { null, counterName });
      return (T)Convert.ChangeType(stringvalue, typeof(T));
    }

    public void GetAndIncrement<T>( String counterName, AsyncCallback<T> callback )
    {
      var responder = new AsyncCallback<string>(r =>
      {
        if (callback != null)
        {
          callback.ResponseHandler.Invoke((T)Convert.ChangeType(r, typeof(T)));
        }
      }, f =>
      {
        if (callback != null)
          callback.ErrorHandler.Invoke(f);
        else
          throw new BackendlessException(f);
      });

      Invoker.InvokeAsync<string>(Invoker.Api.COUNTERSERVICE_GET_INC, new object[] { null, counterName }, responder);
    }
    #endregion

    #region INC_AND_GET
    public int IncrementAndGet( String counterName )
    {
      return IncrementAndGet<int>(counterName);
    }

    public T IncrementAndGet<T>( String counterName )
    {
      string stringvalue = Invoker.InvokeSync<string>(Invoker.Api.COUNTERSERVICE_INC_GET, new object[] { null, counterName });
      return (T)Convert.ChangeType(stringvalue, typeof(T));
    }

    public void IncrementAndGet<T>( String counterName, AsyncCallback<T> callback )
    {
      var responder = new AsyncCallback<string>(r =>
      {
        if (callback != null)
        {
          callback.ResponseHandler.Invoke((T)Convert.ChangeType(r, typeof(T)));
        }
      }, f =>
      {
        if (callback != null)
          callback.ErrorHandler.Invoke(f);
        else
          throw new BackendlessException(f);
      });

      Invoker.InvokeAsync<string>(Invoker.Api.COUNTERSERVICE_INC_GET, new object[] { null, counterName }, responder);
    }
    #endregion

    #region GET_AND_DEC
    public int GetAndDecrement( String counterName )
    {
      return GetAndDecrement<int>(counterName);
    }

    public T GetAndDecrement<T>( String counterName )
    {
      string stringvalue = Invoker.InvokeSync<string>(Invoker.Api.COUNTERSERVICE_GET_DEC, new object[] { null, counterName });
      return (T)Convert.ChangeType(stringvalue, typeof(T));
    }

    public void GetAndDecrement<T>( String counterName, AsyncCallback<T> callback )
    {
      Invoker.InvokeAsync<T>(Invoker.Api.COUNTERSERVICE_GET_DEC, new object[] { null, counterName }, callback);
    }
    #endregion

    #region DEC_AND_GET
    public int DecrementAndGet( String counterName )
    {
      return DecrementAndGet<int>( counterName );
    }

    public T DecrementAndGet<T>( String counterName )
    {
      string stringvalue = Invoker.InvokeSync<string>(Invoker.Api.COUNTERSERVICE_DEC_GET, new object[] { null, counterName });
      return (T)Convert.ChangeType(stringvalue, typeof(T));
    }

    public void DecrementAndGet<T>( String counterName, AsyncCallback<T> callback )
    {
      Invoker.InvokeAsync<T>(Invoker.Api.COUNTERSERVICE_DEC_GET, new object[] { null, counterName }, callback);
    }
    #endregion

    #region ADD_AND_GET
    public int AddAndGet( String counterName, Int64 value )
    {
      return AddAndGet<int>( counterName, value );
    }

    public T AddAndGet<T>( String counterName, Int64 value )
    {
      string stringvalue = Invoker.InvokeSync<string>(Invoker.Api.COUNTERSERVICE_ADD_GET, new object[] { null, counterName, value });
      return (T)Convert.ChangeType(stringvalue, typeof(T));
    }

    public void AddAndGet<T>( String counterName, Int64 value, AsyncCallback<T> callback )
    {
      Invoker.InvokeAsync<T>(Invoker.Api.COUNTERSERVICE_ADD_GET, new object[] { null, counterName, value }, callback);
    }
    #endregion

    #region GET_AND_ADD
    public int GetAndAdd( String counterName, Int64 value )
    {
      return GetAndAdd<int>( counterName, value );
    }

    public T GetAndAdd<T>( String counterName, Int64 value )
    {
      string stringvalue = Invoker.InvokeSync<string>(Invoker.Api.COUNTERSERVICE_GET_ADD, new object[] { null, counterName, value });
      return (T)Convert.ChangeType(stringvalue, typeof(T));
    }

    public void GetAndAdd<T>( String counterName, Int64 value, AsyncCallback<T> callback )
    {
      Invoker.InvokeAsync<T>(Invoker.Api.COUNTERSERVICE_GET_ADD, new object[] { null, counterName, value }, callback);
    }
    #endregion

    #region COMPARE
    public bool CompareAndSet( String counterName, Int64 expected, Int64 updated )
    {
      string stringvalue = Invoker.InvokeSync<string>(Invoker.Api.COUNTERSERVICE_COM_SET, new object[] { null, counterName, expected, updated });
      return Convert.ToBoolean(stringvalue);
    }

    public void CompareAndSet( String counterName, Int64 expected, Int64 updated, AsyncCallback<bool> callback )
    {
      Invoker.InvokeAsync<bool>(Invoker.Api.COUNTERSERVICE_COM_SET, new object[] { null, counterName, expected, updated }, callback);
    }
    #endregion
  }
}
