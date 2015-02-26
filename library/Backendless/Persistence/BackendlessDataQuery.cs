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
using BackendlessAPI.Data;

namespace BackendlessAPI.Persistence
{
  public class BackendlessDataQuery : IBackendlessQuery
  {
    public BackendlessDataQuery()
    {
    }

    public BackendlessDataQuery(List<string> properties)
    {
      Properties = properties;
    }

    public BackendlessDataQuery(string whereClause)
    {
      WhereClause = whereClause;
    }

    public BackendlessDataQuery(QueryOptions queryOptions)
    {
      QueryOptions = queryOptions;
    }

    public BackendlessDataQuery(List<string> properties, string whereClause, QueryOptions queryOptions)
    {
      Properties = properties;
      WhereClause = whereClause;
      QueryOptions = queryOptions;
    }

    public string WhereClause { get; set; }

    public QueryOptions QueryOptions { get; set; }

    public List<string> Properties { get; set; }

    public int PageSize
    {
      get { return QueryOptions == null ? 10 : QueryOptions.PageSize; }
      set
      {
        if (QueryOptions == null)
          QueryOptions = new QueryOptions();

        QueryOptions.PageSize = value;
      }
    }

    public int Offset
    {
      get { return QueryOptions == null ? 0 : QueryOptions.Offset; }
      set
      {
        if (QueryOptions == null)
          QueryOptions = new QueryOptions();

        QueryOptions.Offset = value;
      }
    }

    public IBackendlessQuery NewInstance()
    {
      return new BackendlessDataQuery { Properties = Properties, WhereClause = WhereClause, QueryOptions = QueryOptions };
    }
  }
}