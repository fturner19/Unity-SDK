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
using BackendlessAPI.Async;
using BackendlessAPI.Data;
using BackendlessAPI.Engine;
using BackendlessAPI.Exception;
using BackendlessAPI.Geo;
using BackendlessAPI.LitJson;

namespace BackendlessAPI.Service
{
  public class GeoService
  {
    private static string DEFAULT_CATEGORY_NAME = "Default";
    private static string RESULT = "result";
    private static string GEOPOINT = "geopoint";
    private static string COLLECTION = "collection";

    public GeoService()
    {
    }

    public GeoCategory AddCategory(string categoryName)
    {
      CheckCategoryName(categoryName);

      return Invoker.InvokeSync<GeoCategory>(Invoker.Api.GEOSERVICE_ADDCATEGORY, new object[] { null, categoryName });
    }

    public void AddCategory(string categoryName, AsyncCallback<GeoCategory> callback)
    {
      try
      {
        CheckCategoryName(categoryName);

        Invoker.InvokeAsync<GeoCategory>(Invoker.Api.GEOSERVICE_ADDCATEGORY, new object[] { null, categoryName }, callback);
      }
      catch (System.Exception ex)
      {
        if (callback != null)
          callback.ErrorHandler.Invoke(new BackendlessFault(ex));
        else
          throw;
      }
    }

    public bool DeleteCategory(string categoryName)
    {
      bool result = false;
      CheckCategoryName(categoryName);

      Dictionary<string, object> r = Invoker.InvokeSync<Dictionary<string, object>>(Invoker.Api.GEOSERVICE_DELETECATEGORY, new object[] { null, categoryName });
      try
      {
        result = (bool)r[RESULT];
      }
      catch (System.Exception)
      {
      }

      return result;
    }

    public void DeleteCategory(string categoryName, AsyncCallback<bool> callback)
    {
      try
      {
        CheckCategoryName(categoryName);

        var responder = new AsyncCallback<Dictionary<string, object>>(r =>
        {
          bool result = false;
          try
          {
            result = (bool)r[RESULT];
          }
          catch (System.Exception)
          {
          }
          if (callback != null)
            callback.ResponseHandler.Invoke(result);
        }, f =>
        {
          if (callback != null)
            callback.ErrorHandler.Invoke(f);
          else
            throw new BackendlessException(f);
        });
        Invoker.InvokeAsync<Dictionary<string, object>>(Invoker.Api.GEOSERVICE_DELETECATEGORY, new object[] { null, categoryName }, responder);
      }
      catch (System.Exception ex)
      {
        if (callback != null)
          callback.ErrorHandler.Invoke(new BackendlessFault(ex));
        else
          throw;
      }
    }

    public GeoPoint SavePoint(double latitude, double longitude, Dictionary<string, string> metadata)
    {
      return SavePoint(latitude, longitude, null, metadata);
    }

    public void SavePoint(double latitude, double longitude, Dictionary<string, string> metadata, AsyncCallback<GeoPoint> callback)
    {
      SavePoint(latitude, longitude, null, metadata, callback);
    }

    public GeoPoint SavePoint(double latitude, double longitude, Dictionary<string, object> metadata)
    {
      return SavePoint(latitude, longitude, null, metadata);
    }

    public void SavePoint(double latitude, double longitude, Dictionary<string, object> metadata, AsyncCallback<GeoPoint> callback)
    {
      SavePoint(latitude, longitude, null, metadata, callback);
    }

    public GeoPoint SavePoint(double latitude, double longitude, List<string> categoryNames, Dictionary<string, string> metadata)
    {
      return SavePoint(new GeoPoint(latitude, longitude, categoryNames, metadata));
    }

    public GeoPoint SavePoint(double latitude, double longitude, List<string> categoryNames, Dictionary<string, object> metadata)
    {
      return SavePoint(new GeoPoint(latitude, longitude, categoryNames, metadata));
    }

    public void SavePoint(double latitude, double longitude, List<string> categoryNames, Dictionary<string, object> metadata, AsyncCallback<GeoPoint> callback)
    {
      SavePoint(new GeoPoint(latitude, longitude, categoryNames, metadata), callback);
    }

    public void SavePoint(double latitude, double longitude, List<string> categoryNames, Dictionary<string, string> metadata, AsyncCallback<GeoPoint> callback)
    {
      SavePoint(new GeoPoint(latitude, longitude, categoryNames, metadata), callback);
    }

    public GeoPoint AddPoint(GeoPoint geoPoint)
    {
      return SavePoint(geoPoint);
    }

    public GeoPoint SavePoint(GeoPoint geoPoint)
    {
      if (geoPoint == null)
        throw new ArgumentNullException(ExceptionMessage.NULL_GEOPOINT);

      CheckCoordinates(geoPoint.Latitude, geoPoint.Longitude);

      GeoPoint result = null;
      Dictionary<string, GeoPoint> r = Invoker.InvokeSync<Dictionary<string, GeoPoint>>(Invoker.Api.GEOSERVICE_SAVEPOINT, new object[] { null, GetSavePointQuery(geoPoint) });
      if (r != null && r.ContainsKey(GEOPOINT))
        result = (GeoPoint)r[GEOPOINT];

      return result;
    }

    public void AddPoint(GeoPoint geoPoint, AsyncCallback<GeoPoint> callback)
    {
      SavePoint(geoPoint, callback);
    }

    public void SavePoint(GeoPoint geoPoint, AsyncCallback<GeoPoint> callback)
    {
      try
      {
        if (geoPoint == null)
          throw new ArgumentNullException(ExceptionMessage.NULL_GEOPOINT);

        CheckCoordinates(geoPoint.Latitude, geoPoint.Longitude);

        var responder = new AsyncCallback<Dictionary<string, GeoPoint>>(r =>
        {
          GeoPoint result = null;
          if (r != null && r.ContainsKey(GEOPOINT))
            result = (GeoPoint)r[GEOPOINT];
          if (callback != null)
            callback.ResponseHandler.Invoke(result);
        }, f =>
        {
          if (callback != null)
            callback.ErrorHandler.Invoke(f);
          else
            throw new BackendlessException(f);
        });

        Invoker.Api api = Invoker.Api.GEOSERVICE_SAVEPOINT;
        if (string.IsNullOrEmpty(geoPoint.ObjectId) == false)
          api = Invoker.Api.GEOSERVICE_UPDATEPOINT;

        Invoker.InvokeAsync<Dictionary<string, GeoPoint>>(api, new object[] { null, GetSavePointQuery(geoPoint) }, responder);
      }
      catch (System.Exception ex)
      {
        if (callback != null)
          callback.ErrorHandler.Invoke(new BackendlessFault(ex));
        else
          throw;
      }
    }

#if false // removePoint api not in the REST api.
    public void removePoint(GeoPoint geoPoint)
    {
      if (geoPoint == null)
        throw new ArgumentNullException(ExceptionMessage.NULL_GEOPOINT);

      CheckCoordinates(geoPoint.Latitude, geoPoint.Longitude);
      Invoker.InvokeSync<GeoPoint>(GEO_MANAGER_SERVER_ALIAS, "removePoint",
                                           new object[] { Backendless.AppId, Backendless.VersionNum, geoPoint.ObjectId });
    }

    public void RemovePoint(GeoPoint geoPoint, AsyncCallback<GeoPoint> callback)
    {
      try
      {
        if (geoPoint == null)
          throw new ArgumentNullException(ExceptionMessage.NULL_GEOPOINT);

        Invoker.InvokeAsync(GEO_MANAGER_SERVER_ALIAS, "removePoint",
                             new object[] { Backendless.AppId, Backendless.VersionNum, geoPoint.ObjectId }, callback);
      }
      catch (System.Exception ex)
      {
        if (callback != null)
          callback.ErrorHandler.Invoke(new BackendlessFault(ex));
        else
          throw;
      }
    }
#endif

    public BackendlessCollection<GeoPoint> GetPoints(BackendlessGeoQuery geoQuery)
    {
      checkGeoQuery(geoQuery);

      Invoker.Api api = Invoker.Api.UNKNOWN;
      string query = GetGetPointsQuery(geoQuery, out api);

      BackendlessCollection<GeoPoint> result = null;
      Dictionary<string, BackendlessCollection<GeoPoint>> r = Invoker.InvokeSync<Dictionary<string, BackendlessCollection<GeoPoint>>>(api, new Object[] { null, query });
      if (r != null && r.ContainsKey(COLLECTION))
      {
        result = (BackendlessCollection<GeoPoint>)r[COLLECTION];
        result.Query = geoQuery;
      }

      return result;
    }

    public void GetPoints(BackendlessGeoQuery geoQuery, AsyncCallback<BackendlessCollection<GeoPoint>> callback)
    {
      try
      {
        checkGeoQuery(geoQuery);

        Invoker.Api api = Invoker.Api.UNKNOWN;
        string query = GetGetPointsQuery(geoQuery, out api);

        var responder = new AsyncCallback<Dictionary<string, BackendlessCollection<GeoPoint>>>(r =>
          {
            BackendlessCollection<GeoPoint> result = null;
            if (r != null && r.ContainsKey(COLLECTION))
            {
              result = (BackendlessCollection<GeoPoint>)r[COLLECTION];
              result.Query = geoQuery;
            }

            if (callback != null)
              callback.ResponseHandler.Invoke(result);
          }, f =>
            {
              if (callback != null)
                callback.ErrorHandler.Invoke(f);
              else
                throw new BackendlessException(f);
            });

        Invoker.InvokeAsync<Dictionary<string, BackendlessCollection<GeoPoint>>>(api, new Object[] { null, query }, responder);
      }
      catch (System.Exception ex)
      {
        if (callback != null)
          callback.ErrorHandler.Invoke(new BackendlessFault(ex));
        else
          throw;
      }
    }

    public BackendlessCollection<SearchMatchesResult> RelativeFind(BackendlessGeoQuery geoQuery)
    {
      if (geoQuery == null)
        throw new ArgumentNullException(ExceptionMessage.NULL_GEO_QUERY);

      if (geoQuery.RelativeFindMetadata.Count == 0 || geoQuery.RelativeFindPercentThreshold == 0)
        throw new ArgumentException(ExceptionMessage.INCONSISTENT_GEO_RELATIVE);

      Invoker.Api api = Invoker.Api.UNKNOWN;
      string query = GetGetPointsQuery(geoQuery, out api);

      BackendlessCollection<SearchMatchesResult> result = null;
      Dictionary<string, BackendlessCollection<SearchMatchesResult>> r = Invoker.InvokeSync<Dictionary<string, BackendlessCollection<SearchMatchesResult>>>(Invoker.Api.GEOSERVICE_RELATIVEFIND, new Object[] { null, query });
      if (r != null && r.ContainsKey(COLLECTION))
      {
        result = (BackendlessCollection<SearchMatchesResult>)r[COLLECTION];
        result.Query = geoQuery;
      }

      return result;
    }

    public void RelativeFind(BackendlessGeoQuery geoQuery, AsyncCallback<BackendlessCollection<SearchMatchesResult>> callback)
    {
      try
      {
        if (geoQuery == null)
          throw new ArgumentNullException(ExceptionMessage.NULL_GEO_QUERY);

        if (geoQuery.RelativeFindMetadata.Count == 0 || geoQuery.RelativeFindPercentThreshold == 0)
          throw new ArgumentException(ExceptionMessage.INCONSISTENT_GEO_RELATIVE);

        Invoker.Api api = Invoker.Api.UNKNOWN;
        string query = GetGetPointsQuery(geoQuery, out api);

        var responder = new AsyncCallback<Dictionary<string, BackendlessCollection<SearchMatchesResult>>>(r =>
        {
          BackendlessCollection<SearchMatchesResult> result = null;
          if (r != null && r.ContainsKey(COLLECTION))
          {
            result = (BackendlessCollection<SearchMatchesResult>)r[COLLECTION];
            result.Query = geoQuery;
          }
          if (callback != null)
            callback.ResponseHandler.Invoke(result);
        }, f =>
        {
          if (callback != null)
            callback.ErrorHandler.Invoke(f);
          else
            throw new BackendlessException(f);
        });

        Invoker.InvokeAsync<Dictionary<string, BackendlessCollection<SearchMatchesResult>>>(Invoker.Api.GEOSERVICE_RELATIVEFIND, new Object[] { null, query }, responder);
      }
      catch (System.Exception ex)
      {
        if (callback != null)
          callback.ErrorHandler.Invoke(new BackendlessFault(ex));
        else
          throw;
      }
    }

    public List<GeoCategory> GetCategories()
    {
      return Invoker.InvokeSync<List<GeoCategory>>(Invoker.Api.GEOSERVICE_GETCATEGORIES, new object[] { null });
    }

    public void GetCategories(AsyncCallback<List<GeoCategory>> callback)
    {
      try
      {
        Invoker.InvokeAsync<List<GeoCategory>>(Invoker.Api.GEOSERVICE_GETCATEGORIES, new object[] { null }, callback);
      }
      catch (System.Exception ex)
      {
        if (callback != null)
          callback.ErrorHandler.Invoke(new BackendlessFault(ex));
        else
          throw;
      }
    }

    private void CheckCoordinates(double? latitude, double? longitude)
    {
      if (latitude > 90 || latitude < -90)
        throw new ArgumentException(ExceptionMessage.WRONG_LATITUDE_VALUE);

      if (longitude > 180 || latitude < -180)
        throw new ArgumentException(ExceptionMessage.WRONG_LONGITUDE_VALUE);
    }

    private void CheckCategoryName(string categoryName)
    {
      if (string.IsNullOrEmpty(categoryName))
        throw new ArgumentNullException(ExceptionMessage.NULL_CATEGORY_NAME);

      if (categoryName.Equals(DEFAULT_CATEGORY_NAME))
        throw new ArgumentException(ExceptionMessage.DEFAULT_CATEGORY_NAME);
    }

    private void checkGeoQuery(BackendlessGeoQuery geoQuery)
    {
      if (geoQuery == null)
        throw new ArgumentNullException(ExceptionMessage.NULL_GEO_QUERY);

      if (geoQuery.SearchRectangle != null)
      {
        if (geoQuery.SearchRectangle.Length != 4)
          throw new ArgumentException(ExceptionMessage.WRONG_SEARCH_RECTANGLE_QUERY);

        if (!Double.IsNaN(geoQuery.Radius))
          throw new ArgumentException(ExceptionMessage.INCONSISTENT_GEO_QUERY);

        if (!Double.IsNaN(geoQuery.Latitude))
          throw new ArgumentException(ExceptionMessage.INCONSISTENT_GEO_QUERY);

        if (!Double.IsNaN(geoQuery.Longitude))
          throw new ArgumentException(ExceptionMessage.INCONSISTENT_GEO_QUERY);
      }
      else if (!Double.IsNaN(geoQuery.Radius))
      {
        if (geoQuery.Radius <= 0)
          throw new ArgumentException(ExceptionMessage.WRONG_RADIUS);

        if (Double.IsNaN(geoQuery.Latitude))
          throw new ArgumentNullException(ExceptionMessage.WRONG_LATITUDE_VALUE);

        if (Double.IsNaN(geoQuery.Longitude))
          throw new ArgumentNullException(ExceptionMessage.WRONG_LONGITUDE_VALUE);

        CheckCoordinates(geoQuery.Latitude, geoQuery.Longitude);

        if (geoQuery.Units == null)
          throw new ArgumentNullException(ExceptionMessage.NULL_UNIT);
      }
      else if (geoQuery.Categories == null && geoQuery.Metadata == null)
        throw new ArgumentNullException(ExceptionMessage.WRONG_GEO_QUERY);

      if (geoQuery.Categories != null)
        foreach (string categoryName in geoQuery.Categories)
          CheckCategoryName(categoryName);

      if (geoQuery.Offset < 0)
        throw new ArgumentException(ExceptionMessage.WRONG_OFFSET);

      if (geoQuery.PageSize < 0)
        throw new ArgumentException(ExceptionMessage.WRONG_PAGE_SIZE);
    }

    private static string GetSavePointQuery(GeoPoint geoPoint)
    {
      string query = null;
      if (geoPoint != null)
      {
        query = "";

        string objectId = geoPoint.ObjectId;
        if (string.IsNullOrEmpty(objectId) == false)
          query += "/" + objectId;

        query += "?lat=" + geoPoint.Latitude;

        query += "&lon=" + geoPoint.Longitude;

        List<string> categoriesList = geoPoint.Categories;
        if (categoriesList != null && categoriesList.Count > 0)
        {
          string categories = "";
          foreach (string category in categoriesList)
          {
            if (string.IsNullOrEmpty(categories) == false)
              categories += ",";
            if (category == null)
              categories += "null";
            else
              categories += category;
          }
          if (string.IsNullOrEmpty(categories) == false)
            query += "&categories=" + categories;
        }

        Dictionary<string, object> metadataList = geoPoint.Metadata;
        if (metadataList != null && metadataList.Count > 0)
        {
          string metadata = JsonMapper.ToJson(metadataList);
          if (string.IsNullOrEmpty(metadata) == false)
            query += "&metadata=" + UnityEngine.WWW.EscapeURL(metadata);
        }

      }
      return query;
    }

    private static void AddQuery(ref string query, string addQuery)
    {
      if (string.IsNullOrEmpty(query))
        query = "?";
      else
        query += "&";
      query += addQuery;
    }

    private static string GetGetPointsQuery(BackendlessGeoQuery geoQuery, out Invoker.Api api)
    {
      string query = null;
      if (geoQuery != null)
      {
        double[] searchRectangle = geoQuery.SearchRectangle;
        if (searchRectangle != null)
        {
          api = Invoker.Api.GEOSERVICE_GETRECT;

          if (searchRectangle.Length == 4)
          {
            AddQuery(ref query, "nwlat=" + searchRectangle[0]);
            AddQuery(ref query, "nwlon=" + searchRectangle[1]);
            AddQuery(ref query, "selat=" + searchRectangle[2]);
            AddQuery(ref query, "selon=" + searchRectangle[3]);
          }
        }
        else
        {
          api = Invoker.Api.GEOSERVICE_GETPOINTS;

          Dictionary<string, string> relativeFindMetadataList = geoQuery.RelativeFindMetadata;
          if (relativeFindMetadataList != null && relativeFindMetadataList.Count > 0)
          {
            api = Invoker.Api.GEOSERVICE_RELATIVEFIND;

            string metadata = JsonMapper.ToJson(relativeFindMetadataList);
            if (string.IsNullOrEmpty(metadata) == false)
              AddQuery(ref query, "relativeFindMetadata=" + UnityEngine.WWW.EscapeURL(metadata));

            AddQuery(ref query, "relativeFindPercentThreshold=" + geoQuery.RelativeFindPercentThreshold);
          }

          if (Double.NaN.Equals(geoQuery.Latitude) == false)
            AddQuery(ref query, "lat=" + geoQuery.Latitude);

          if (Double.NaN.Equals(geoQuery.Longitude) == false)
            AddQuery(ref query, "lon=" + geoQuery.Longitude);

          if (Double.NaN.Equals(geoQuery.Radius) == false)
            AddQuery(ref query, "r=" + geoQuery.Radius);

          Units? unit = geoQuery.Units;
          if (unit != null)
            AddQuery(ref query, "units=" + unit.ToString());
        }

        List<string> categoriesList = geoQuery.Categories;
        if (categoriesList != null && categoriesList.Count > 0)
        {
          string categories = "";
          foreach (string category in categoriesList)
          {
            if (string.IsNullOrEmpty(categories) == false)
              categories += ",";
            categories += category;
          }
          if (string.IsNullOrEmpty(categories) == false)
            AddQuery(ref query, "categories=" + categories);
        }

        Dictionary<string, string> metadataList = geoQuery.Metadata;
        if (metadataList != null && metadataList.Count > 0)
        {
          string metadata = JsonMapper.ToJson(metadataList);
          if (string.IsNullOrEmpty(metadata) == false)
            AddQuery(ref query, "metadata=" + UnityEngine.WWW.EscapeURL(metadata));
        }

        AddQuery(ref query, "includemetadata=" + geoQuery.IncludeMeta.ToString().ToLower());

        AddQuery(ref query, "pagesize=" + geoQuery.PageSize);

        AddQuery(ref query, "offset=" + geoQuery.Offset);
      }
      else
        api = Invoker.Api.UNKNOWN;

      return query;
    }
  }
}