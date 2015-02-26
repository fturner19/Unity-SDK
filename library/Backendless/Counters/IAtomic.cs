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
  public interface IAtomic<T>
  {
    void Reset();

    void Reset( AsyncCallback<Object> responder );

    T Get();

    void Get( AsyncCallback<T> responder );

    T GetAndIncrement();

    void GetAndIncrement( AsyncCallback<T> responder );

    T IncrementAndGet();

    void IncrementAndGet( AsyncCallback<T> responder );

    T GetAndDecrement();

    void GetAndDecrement( AsyncCallback<T> responder );

    T DecrementAndGet();

    void DecrementAndGet( AsyncCallback<T> responder );

    T AddAndGet( Int64 value );

    void AddAndGet( Int64 value, AsyncCallback<T> responder );

    T GetAndAdd( Int64 value );

    void GetAndAdd( Int64 value, AsyncCallback<T> responder );

    bool CompareAndSet( Int64 expected, Int64 updated );

    void CompareAndSet( Int64 expected, Int64 updated, AsyncCallback<Boolean> responder );
  }
}
