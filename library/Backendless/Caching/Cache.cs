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
using System.Text;
using BackendlessAPI.Async;
using BackendlessAPI.Engine;
using BackendlessAPI.Exception;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace BackendlessAPI.Caching
{
public class Cache
{
  private static readonly Cache instance = new Cache();

  public static Cache GetInstance()
  {
    return instance;
  }

  private Cache()
  {
  }

  public ICache<T> With<T>( String key, T type )
  {
    return new CacheService<T>( type, key );
  }
  
  public void Put( String key, Object obj, int expire, AsyncCallback<Object> callback )
  {
    Invoker.InvokeAsync<object>(Invoker.Api.CACHESERVICE_PUT, new object[] { obj, key, expire }, callback);
  }

  public void Put( String key, Object obj, AsyncCallback<Object> callback )
  {
    Put( key, obj, 0, callback );
  }

  public void Put( String key, Object obj )
  {
    Put( key, obj, 0 );
  }

  public void Put( String key, Object obj, int expire )
  {
    Invoker.InvokeSync<object>(Invoker.Api.CACHESERVICE_PUT, new object[] { obj, key, expire });
  }

  public T Get<T>( String key )
  {
    return Invoker.InvokeSync<T>(Invoker.Api.CACHESERVICE_GET, new object[] { null, key });
  }

  public void Get<T>( String key, AsyncCallback<T> callback )
  {
    Invoker.InvokeAsync<T>(Invoker.Api.CACHESERVICE_GET, new object[] { null, key }, callback);
  }

  public Boolean Contains( String key )
  {
    return Invoker.InvokeSync<Boolean>(Invoker.Api.CACHESERVICE_CONTAINS, new object[] { null, key });
  }

  public void Contains( String key, AsyncCallback<Boolean> callback )
  {
    Invoker.InvokeAsync<Boolean>(Invoker.Api.CACHESERVICE_CONTAINS, new object[] { null, key }, callback);
  }

  public void ExpireIn( String key, int seconds )
  {
    Invoker.InvokeSync<object>(Invoker.Api.CACHESERVICE_EXPIREIN, new object[] { null, key, seconds });
  }

  public void ExpireIn( String key, int seconds, AsyncCallback<Object> callback )
  {
    Invoker.InvokeAsync<Object>(Invoker.Api.CACHESERVICE_EXPIREIN, new object[] { null, key, seconds }, callback);
  }

  public void ExpireAt( String key, int timestamp )
  {
    Invoker.InvokeSync<object>(Invoker.Api.CACHESERVICE_EXPIREAT, new object[] { null, key, timestamp });
  }

  public void ExpireAt( String key, int timestamp, AsyncCallback<Object> callback )
  {
    Invoker.InvokeAsync<Object>(Invoker.Api.CACHESERVICE_EXPIREAT, new object[] { null, key, timestamp }, callback);
  }

  public void Delete( String key )
  {
    Invoker.InvokeSync<object>(Invoker.Api.CACHESERVICE_DELETE, new object[] { null, key });
  }

  public void Delete( String key, AsyncCallback<Object> callback )
  {
    Invoker.InvokeAsync<Object>(Invoker.Api.CACHESERVICE_DELETE, new object[] { null, key }, callback);
  }
}
}
