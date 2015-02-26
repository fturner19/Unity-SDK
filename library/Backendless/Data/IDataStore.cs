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

using System.Collections.Generic;
using BackendlessAPI.Async;
using BackendlessAPI.Persistence;

namespace BackendlessAPI.Data
{
  public interface IDataStore<T>
  {
    T Save(T entity);

    void Save(T entity, AsyncCallback<T> responder);

    long Remove(T entity);

    void Remove(T entity, AsyncCallback<long> responder);

    T FindFirst();

    T FindFirst(IList<string> relations);

    void FindFirst(AsyncCallback<T> responder);

    void FindFirst(IList<string> relations, AsyncCallback<T> responder);

    T FindLast();

    T FindLast(IList<string> relations);

    void FindLast(AsyncCallback<T> responder);

    void FindLast(IList<string> relations, AsyncCallback<T> responder);

    BackendlessCollection<T> Find();

    BackendlessCollection<T> Find(BackendlessDataQuery dataQueryOptions);

    void Find(AsyncCallback<BackendlessCollection<T>> responder);

    void Find(BackendlessDataQuery dataQueryOptions, AsyncCallback<BackendlessCollection<T>> responder);

    T FindById(string id);

    T FindById(string id, IList<string> relations);

    void FindById(string id, AsyncCallback<T> responder);

    void FindById(string id, IList<string> relations, AsyncCallback<T> responder);

    void LoadRelations(T entity, IList<string> relations);

    void LoadRelations(T entity, IList<string> relations, AsyncCallback<T> responder);
  }
}