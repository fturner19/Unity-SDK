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

namespace BackendlessAPI.Counters
{
  class AtomicImpl<T> : IAtomic<T>
  {
    private String counterName;

    public AtomicImpl( String counterName )
    {
      this.counterName = counterName;
    }

    public void Reset()
    {
      CounterService.GetInstance().Reset( counterName );
    }

    public void Reset( AsyncCallback<Object> callback )
    {
      CounterService.GetInstance().Reset( counterName, callback );
    }

    public T Get()
    {
      return CounterService.GetInstance().Get<T>( counterName );
    }

    public void Get( Async.AsyncCallback<T> callback )
    {
      CounterService.GetInstance().Get( counterName, callback );
    }

    public T GetAndIncrement()
    {
      return CounterService.GetInstance().GetAndIncrement<T>( counterName );
    }

    public void GetAndIncrement( Async.AsyncCallback<T> callback )
    {
      CounterService.GetInstance().GetAndIncrement( counterName, callback );
    }

    public T IncrementAndGet()
    {
      return CounterService.GetInstance().IncrementAndGet<T>( counterName );
    }

    public void IncrementAndGet( Async.AsyncCallback<T> callback )
    {
      CounterService.GetInstance().IncrementAndGet( counterName, callback );
    }

    public T GetAndDecrement()
    {
      return CounterService.GetInstance().GetAndDecrement<T>( counterName );
    }

    public void GetAndDecrement( Async.AsyncCallback<T> callback )
    {
      CounterService.GetInstance().GetAndDecrement( counterName, callback );
    }

    public T DecrementAndGet()
    {
      return CounterService.GetInstance().DecrementAndGet<T>( counterName );
    }

    public void DecrementAndGet( Async.AsyncCallback<T> callback )
    {
      CounterService.GetInstance().DecrementAndGet( counterName, callback );
    }

    public T AddAndGet( Int64 value )
    {
      return CounterService.GetInstance().AddAndGet<T>( counterName, value );
    }

    public void AddAndGet( Int64 value, Async.AsyncCallback<T> callback )
    {
      CounterService.GetInstance().AddAndGet<T>( counterName, value, callback );
    }

    public T GetAndAdd( Int64 value )
    {
      return CounterService.GetInstance().GetAndAdd<T>( counterName, value );
    }

    public void GetAndAdd( Int64 value, Async.AsyncCallback<T> callback )
    {
      CounterService.GetInstance().GetAndAdd( counterName, value, callback );
    }

    public bool CompareAndSet( Int64 expected, Int64 updated )
    {
      return CounterService.GetInstance().CompareAndSet( counterName, expected, updated );
    }

    public void CompareAndSet( Int64 expected, Int64 updated, Async.AsyncCallback<bool> callback )
    {
      CounterService.GetInstance().CompareAndSet( counterName, expected, updated, callback );
    }
  }
}
