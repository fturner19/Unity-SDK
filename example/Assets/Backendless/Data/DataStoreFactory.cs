﻿/*
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

using System.Collections.Generic;
using BackendlessAPI.Async;
using BackendlessAPI.Persistence;

namespace BackendlessAPI.Data
{
  internal static class DataStoreFactory
  {
    internal static IDataStore<T> CreateDataStore<T>()
    {
      return new DataStoreImpl<T>();
    }

    private class DataStoreImpl<T> : IDataStore<T>
    {
      public T Save(T entity)
      {
        return Backendless.Persistence.Save(entity);
      }

      public void Save(T entity, AsyncCallback<T> responder)
      {
        Backendless.Persistence.Save(entity, responder);
      }

      public long Remove(T entity)
      {
        return Backendless.Persistence.Remove(entity);
      }

      public void Remove(T entity, AsyncCallback<long> responder)
      {
        Backendless.Persistence.Remove(entity, responder);
      }

      public T FindFirst()
      {
        return Backendless.Persistence.First<T>();
      }

      public T FindFirst(IList<string> relations)
      {
        return Backendless.Persistence.First<T>(relations);
      }

      public void FindFirst(AsyncCallback<T> responder)
      {
        Backendless.Persistence.First(responder);
      }

      public void FindFirst(IList<string> relations, AsyncCallback<T> responder)
      {
        Backendless.Persistence.First(relations, responder);
      }

      public T FindLast()
      {
        return Backendless.Persistence.Last<T>();
      }

      public T FindLast(IList<string> relations)
      {
        return Backendless.Persistence.Last<T>(relations);
      }

      public void FindLast(AsyncCallback<T> responder)
      {
        Backendless.Persistence.Last(responder);
      }

      public void FindLast(IList<string> relations, AsyncCallback<T> responder)
      {
        Backendless.Persistence.Last(relations, responder);
      }

      public BackendlessCollection<T> Find()
      {
        return Backendless.Persistence.Find<T>(null);
      }

      public BackendlessCollection<T> Find(BackendlessDataQuery dataQueryOptions)
      {
        return Backendless.Persistence.Find<T>(dataQueryOptions);
      }

      public void Find(AsyncCallback<BackendlessCollection<T>> responder)
      {
        Backendless.Persistence.Find(null, responder);
      }

      public void Find(BackendlessDataQuery dataQueryOptions, AsyncCallback<BackendlessCollection<T>> responder)
      {
        Backendless.Persistence.Find(dataQueryOptions, responder);
      }

      public T FindById(string id)
      {
        return Backendless.Persistence.FindById<T>(id, (IList<string>)null);
      }

      public T FindById(string id, IList<string> relations)
      {
        return Backendless.Persistence.FindById<T>(id, relations);
      }

      public void FindById(string id, AsyncCallback<T> responder)
      {
        Backendless.Persistence.FindById(id, null, responder);
      }

      public void FindById(string id, IList<string> relations, AsyncCallback<T> responder)
      {
        Backendless.Persistence.FindById(id, relations, responder);
      }

      public void LoadRelations(T entity, IList<string> relations)
      {
        Backendless.Persistence.LoadRelations(entity, relations);
      }

      public void LoadRelations(T entity, IList<string> relations, AsyncCallback<T> responder)
      {
        Backendless.Persistence.LoadRelations(entity, relations, responder);
      }
    }
  }
}