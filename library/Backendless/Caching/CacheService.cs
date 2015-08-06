/*
Copyright 2015 Backendless Corp. All Rights Reserved.

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

namespace BackendlessAPI.Caching
{
  class CacheService<T> : ICache<T>
  {
    private String key;

    public CacheService( T type, String key )
    {
      this.key = key;
    }

    public void Put( T value, AsyncCallback<object> callback )
    {
      Cache.GetInstance().Put( key, value, callback );
    }

    public void Put( T value, int expire, AsyncCallback<object> callback )
    {
      Cache.GetInstance().Put( key, value, expire, callback );
    }

    public void Put( T value )
    {
      Cache.GetInstance().Put( key, value );
    }

    public void Put( T value, int expire )
    {
      Cache.GetInstance().Put( key, value, expire );
    }

    public void Get( AsyncCallback<T> callback )
    {
      Cache.GetInstance().Get( key, callback );
    }

    public T Get()
    {
      return (T) Cache.GetInstance().Get<T>( key );
    }

    public void Contains( AsyncCallback<bool> callback )
    {
      Cache.GetInstance().Contains( key, callback );
    }

    public bool Contains()
    {
      return Cache.GetInstance().Contains( key );
    }

    public void ExpireIn( int seconds, AsyncCallback<object> callback )
    {
      Cache.GetInstance().ExpireIn( key, seconds, callback );
    }

    public void ExpireIn( int seconds )
    {
      Cache.GetInstance().ExpireIn( key, seconds );
    }

    public void ExpireAt( int seconds, AsyncCallback<object> callback )
    {
      Cache.GetInstance().ExpireAt( key, seconds, callback );
    }

    public void ExpireAt( int seconds )
    {
       Cache.GetInstance().ExpireAt( key, seconds );
    }

    public void Delete( AsyncCallback<object> callback )
    {
      Cache.GetInstance().Delete( key, callback );
    }

    public void Delete()
    {
      Cache.GetInstance().Delete( key );
    }
  }
}
