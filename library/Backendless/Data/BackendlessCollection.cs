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
using System.Threading;
using BackendlessAPI.Async;
using BackendlessAPI.Exception;
using BackendlessAPI.Geo;
using BackendlessAPI.Persistence;
using BackendlessAPI.LitJson;

namespace BackendlessAPI.Data
{
  public class BackendlessCollection<T>
  {
    [JsonProperty("totalObjects")]
    public int TotalObjects { get; set; }

    [JsonProperty("data")]
    public List<T> Data { get; set; }

    public IBackendlessQuery Query { get; set; }

    public int PageSize
    {
      get { return Query == null ? 0 : (int)Query.PageSize; }
      set { Query.PageSize = value; }
    }

    public List<T> GetCurrentPage()
    {
      return Data;
    }

    //Sync methods
    public BackendlessCollection<T> NextPage()
    {
      int offset = Query.Offset;
      int pageSize = Query.PageSize;

      return GetPage(pageSize, offset + pageSize);
    }

    public BackendlessCollection<T> PreviousPage()
    {
      int offset = Query.Offset;
      int pageSize = Query.PageSize;

      return (offset - pageSize) >= 0 ? GetPage(pageSize, offset - pageSize) : NewInstance();
    }

    public BackendlessCollection<T> GetPage(int pageSize, int offset)
    {
      return (BackendlessCollection<T>)DownloadPage(pageSize, offset);
    }

    //Async methods
    public void NextPage(AsyncCallback<BackendlessCollection<T>> responder)
    {
      int offset = Query.Offset;
      int pageSize = Query.PageSize;

      GetPage(pageSize, offset + pageSize, responder);
    }

    public void PreviousPage(AsyncCallback<BackendlessCollection<T>> responder)
    {
      int offset = Query.Offset;
      int pageSize = Query.PageSize;

      if ((offset - pageSize) >= 0)
        GetPage(pageSize, offset - pageSize, responder);
      else
        responder.ResponseHandler.Invoke(NewInstance());
    }

    public void GetPage(int pageSize, int offset, AsyncCallback<BackendlessCollection<T>> responder)
    {
      DownloadPage(pageSize, offset, responder);
    }

    //Download page logic
    private object DownloadPage(int pageSize, int offset)
    {
      IBackendlessQuery tempQuery = Query.NewInstance();
      tempQuery.Offset = offset;
      tempQuery.PageSize = pageSize;

      if (typeof(T) == typeof(GeoPoint))
        return Backendless.Geo.GetPoints((BackendlessGeoQuery)tempQuery);
      else
        return Backendless.Persistence.Find<T>((BackendlessDataQuery)tempQuery);
    }

    private void DownloadPage(int pageSize, int offset, AsyncCallback<BackendlessCollection<T>> responder)
    {
      IBackendlessQuery tempQuery = Query.NewInstance();
      tempQuery.Offset = offset;
      tempQuery.PageSize = pageSize;

      ThreadPool.QueueUserWorkItem(state =>
        {
          try
          {
            responder.ResponseHandler.Invoke((BackendlessCollection<T>)DownloadPage(pageSize, offset));
          }
          catch (BackendlessException e)
          {
            responder.ErrorHandler.Invoke(e.BackendlessFault);
          }
          catch (System.Exception e)
          {
            responder.ErrorHandler.Invoke(new BackendlessFault(e.Message));
          }
        });
    }

    private BackendlessCollection<T> NewInstance()
    {
      return new BackendlessCollection<T> { Data = Data, Query = Query, TotalObjects = TotalObjects };
    }
  }
}