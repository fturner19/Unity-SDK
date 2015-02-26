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
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using BackendlessAPI.Async;
using BackendlessAPI.Data;
using BackendlessAPI.Engine;
using BackendlessAPI.Exception;
using BackendlessAPI.Persistence;
using BackendlessAPI.Property;
using BackendlessAPI.LitJson;

namespace BackendlessAPI.Service
{
  public class PersistenceService
  {
    private const string DEFAULT_OBJECT_ID_PROPERTY_NAME_JAVA_STYLE = "objectId";
    private const string DEFAULT_CREATED_FIELD_NAME_JAVA_STYLE = "created";
    private const string DEFAULT_UPDATED_FIELD_NAME_JAVA_STYLE = "updated";
    private const string DEFAULT_OBJECT_ID_PROPERTY_NAME_DOTNET_STYLE = "ObjectId";
    private const string DEFAULT_CREATED_FIELD_NAME_DOTNET_STYLE = "Created";
    private const string DEFAULT_UPDATED_FIELD_NAME_DOTNET_STYLE = "Updated";
    public const string LOAD_ALL_RELATIONS = "*";
    private static string DELETION_TIME = "deletionTime";

    public PersistenceService()
    {
    }
    #region Save
    public T Save<T>( T entity )
    {
      if (entity == null)
        throw new ArgumentNullException(ExceptionMessage.NULL_ENTITY);

      CheckEntityStructure<T>();

      return GetEntityId(entity) == null ? Create(entity) : Update(entity);
    }

    public void Save<T>(T entity, AsyncCallback<T> callback)
    {
      if (entity == null)
        throw new ArgumentNullException(ExceptionMessage.NULL_ENTITY);

      CheckEntityStructure<T>();

      if (GetEntityId(entity) == null)
        Create(entity, callback);
      else
        Update(entity, callback);
    }
    #endregion
    #region Create
    internal T Create<T>(T entity)
    {
      if (entity == null)
        throw new ArgumentNullException(ExceptionMessage.NULL_ENTITY);

      return Invoker.InvokeSync<T>(Invoker.Api.PERSISTENCESERVICE_CREATE, new object[] { entity, GetTypeName(typeof(T)) });
    }

    internal void Create<T>(T entity, AsyncCallback<T> callback)
    {
      Invoker.InvokeAsync<T>(Invoker.Api.PERSISTENCESERVICE_CREATE, new object[] { entity, GetTypeName(typeof(T)) }, callback);
    }
    #endregion
    #region Update
    internal T Update<T>(T entity)
    {
      if (entity == null)
        throw new ArgumentNullException(ExceptionMessage.NULL_ENTITY);

      T result = Invoker.InvokeSync<T>(Invoker.Api.PERSISTENCESERVICE_UPDATE, new object[] { entity, GetTypeName(typeof(T)), GetEntityId(entity) });
      return result;
    }

    internal void Update<T>(T entity, AsyncCallback<T> callback)
    {
      Invoker.InvokeAsync<T>(Invoker.Api.PERSISTENCESERVICE_UPDATE, new object[] { entity, GetTypeName(typeof(T)), GetEntityId(entity) }, callback);
    }
    #endregion
    #region Remove
    internal long Remove<T>(T entity)
    {
      if (entity == null)
        throw new ArgumentNullException(ExceptionMessage.NULL_ENTITY);

      string id = GetEntityId(entity);

      if (string.IsNullOrEmpty(id))
        throw new ArgumentNullException(ExceptionMessage.NULL_ID);

      long deletionTime = 0;
      Dictionary<string, object> r = Invoker.InvokeSync<Dictionary<string, object>>(Invoker.Api.PERSISTENCESERVICE_REMOVE, new Object[] { null, GetTypeName(typeof(T)), id });
      try
      {
        deletionTime = (long)r[DELETION_TIME];
      }
      catch (System.Exception)
      {
      }
      return deletionTime;
    }

    internal void Remove<T>(T entity, AsyncCallback<long> callback)
    {
      if (entity == null)
        throw new ArgumentNullException(ExceptionMessage.NULL_ENTITY);

      string id = GetEntityId(entity);

      if (string.IsNullOrEmpty(id))
        throw new ArgumentNullException(ExceptionMessage.NULL_ID);

      var responder = new AsyncCallback<Dictionary<string, object>>(r =>
      {
        long deletionTime = 0;
        try
        {
          deletionTime = (long)r[DELETION_TIME];
        }
        catch (System.Exception)
        {
        }
        if (callback != null)
          callback.ResponseHandler.Invoke(deletionTime);
      }, f =>
      {
        if (callback != null)
          callback.ErrorHandler.Invoke(f);
        else
          throw new BackendlessException(f);
      });

      Invoker.InvokeAsync<Dictionary<string, object>>(Invoker.Api.PERSISTENCESERVICE_REMOVE, new Object[] { null, GetTypeName(typeof(T)), id }, responder);
    }
    #endregion 
    #region FindById with Id
    internal T FindById<T>(string id, IList<string> relations)
    {
      if (id == null)
        throw new ArgumentNullException(ExceptionMessage.NULL_ID);

      return Invoker.InvokeSync<T>(Invoker.Api.PERSISTENCESERVICE_FIND, new Object[] { null, GetTypeName(typeof(T)), id, GetLoadRelationsQuery(relations) });
    }

    internal void FindById<T>(string id, IList<string> relations, AsyncCallback<T> callback)
    {
      if (id == null)
        throw new ArgumentNullException(ExceptionMessage.NULL_ID);

      Invoker.InvokeAsync<T>(Invoker.Api.PERSISTENCESERVICE_FIND, new Object[] { null, GetTypeName(typeof(T)), id, GetLoadRelationsQuery(relations) }, callback);
    }

    #endregion
    #region LoadRelations
    internal void LoadRelations<T>(T entity, IList<string> relations)
    {
      if (entity == null)
        throw new ArgumentNullException(ExceptionMessage.NULL_ENTITY);

      String id = GetEntityId(entity);

      if (id == null)
        throw new ArgumentNullException(ExceptionMessage.NULL_ID);

      var loadedRelations = Invoker.InvokeSync<T>(Invoker.Api.PERSISTENCESERVICE_FIND, new Object[] { null, GetTypeName(typeof(T)), id, GetLoadRelationsQuery(relations) });
      LoadRelationsToEntity(entity, loadedRelations, relations);
    }

    internal void LoadRelations<T>(T entity, IList<string> relations, AsyncCallback<T> callback)
    {
      if (entity == null)
        throw new ArgumentNullException(ExceptionMessage.NULL_ENTITY);

      String id = GetEntityId(entity);

      if (id == null)
        throw new ArgumentNullException(ExceptionMessage.NULL_ID);

      var responder = new AsyncCallback<T>(response =>
        {
          LoadRelationsToEntity(entity, response, relations);
          if (callback != null)
            callback.ResponseHandler.Invoke(response);
        }, fault =>
          {
            if (callback != null)
              callback.ErrorHandler.Invoke(fault);
          });

      Invoker.InvokeAsync<T>(Invoker.Api.PERSISTENCESERVICE_FIND, new Object[] { null, GetTypeName(typeof(T)), id, GetLoadRelationsQuery(relations) }, responder);
    }

    private void LoadRelationsToEntity<T>(T entity, T response, IList<string> relations)
    {
      if (typeof(T).Equals(typeof(BackendlessUser)))
      {
        object source = entity;
        object updated = response;
        BackendlessUser userWithRelations = (BackendlessUser)updated;
        BackendlessUser sourceUser = (BackendlessUser)source;
        sourceUser.PutProperties(userWithRelations.Properties);
      }
      else
      {
        FieldInfo[] fields = typeof(T).GetFields();
        foreach (var fieldInfo in fields)
        {
          if (relations.Contains(LOAD_ALL_RELATIONS) == false && relations.Contains(fieldInfo.Name) == false)
            continue;

          fieldInfo.SetValue(entity, fieldInfo.GetValue(response));
        }
        PropertyInfo[] properties = typeof(T).GetProperties();
        foreach (var propertyInfo in properties)
        {
          if (relations.Contains(LOAD_ALL_RELATIONS) == false && relations.Contains(propertyInfo.Name) == false)
            continue;

          propertyInfo.SetValue(entity, propertyInfo.GetValue(response, null), null);
        }
      }
    }
    #endregion
    #region Describe
    public List<ObjectProperty> Describe(string className)
    {
      if (string.IsNullOrEmpty(className))
        throw new ArgumentNullException(ExceptionMessage.NULL_ENTITY_NAME);

      return Invoker.InvokeSync<List<ObjectProperty>>(Invoker.Api.PERSISTENCESERVICE_DESCRIBE,
                                                       new Object[] { null, className });
    }

    public void Describe(string className, AsyncCallback<List<ObjectProperty>> callback)
    {
      if (string.IsNullOrEmpty(className))
        throw new ArgumentNullException(ExceptionMessage.NULL_ENTITY_NAME);

      Invoker.InvokeAsync(Invoker.Api.PERSISTENCESERVICE_DESCRIBE,
                           new Object[] { null, className }, callback);
    }
    #endregion
    #region Find
    public BackendlessCollection<T> Find<T>(BackendlessDataQuery dataQuery)
    {
      CheckPageSizeAndOffset(dataQuery);
      var result = Invoker.InvokeSync<BackendlessCollection<T>>(Invoker.Api.PERSISTENCESERVICE_FIND, new object[] { null, GetTypeName(typeof(T)), null, GetFindQuery(dataQuery) });
      result.Query = dataQuery;

      return result;
    }

    public void Find<T>(BackendlessDataQuery dataQuery, AsyncCallback<BackendlessCollection<T>> callback)
    {
      var responder = new AsyncCallback<BackendlessCollection<T>>(r =>
        {
          r.Query = dataQuery;

          if (callback != null)
            callback.ResponseHandler.Invoke(r);
        }, f =>
          {
            if (callback != null)
              callback.ErrorHandler.Invoke(f);
            else
              throw new BackendlessException(f);
          });

      Invoker.InvokeAsync<BackendlessCollection<T>>(Invoker.Api.PERSISTENCESERVICE_FIND, new object[] { null, GetTypeName(typeof(T)), null, GetFindQuery(dataQuery) }, responder);
    }
    #endregion
    #region First
    public T First<T>()
    {
      return First<T>((IList<String>)null);
    }

    public T First<T>(IList<String> relations)
    {
      return Invoker.InvokeSync<T>(Invoker.Api.PERSISTENCESERVICE_FIND, new object[] { null, GetTypeName(typeof(T)), "first", GetLoadRelationsQuery(relations) });
    }

    public void First<T>(AsyncCallback<T> callback)
    {
      First<T>(null, callback);
    }

    public void First<T>(IList<String> relations, AsyncCallback<T> callback)
    {
      Invoker.InvokeAsync<T>(Invoker.Api.PERSISTENCESERVICE_FIND, new object[] { null, GetTypeName(typeof(T)), "first", GetLoadRelationsQuery(relations) }, callback);
    }
    #endregion 
    #region Last
    public T Last<T>()
    {
      return Last<T>((IList<String>)null);
    }

    public T Last<T>(IList<String> relations)
    {
      return Invoker.InvokeSync<T>(Invoker.Api.PERSISTENCESERVICE_FIND, new object[] { null, GetTypeName(typeof(T)), "last", GetLoadRelationsQuery(relations) });
    }

    public void Last<T>(AsyncCallback<T> callback)
    {
      Last<T>(null, callback);
    }

    public void Last<T>(IList<String> relations, AsyncCallback<T> callback)
    {
      Invoker.InvokeAsync<T>(Invoker.Api.PERSISTENCESERVICE_FIND, new object[] { null, GetTypeName(typeof(T)), "last", GetLoadRelationsQuery(relations) }, callback);
    }
    #endregion

    public IDataStore<T> Of<T>()
    {
      return DataStoreFactory.CreateDataStore<T>();
    }

    private static string GetEntityId<T>(T entity)
    {
      try
      {
        Type entityType = entity.GetType();

        if (entityType.Equals(typeof(BackendlessUser)))
        {
          object entityObject = entity;
          return ((BackendlessUser)entityObject).UserId;
        }

        PropertyInfo objectIdProp = entityType.GetProperty(DEFAULT_OBJECT_ID_PROPERTY_NAME_DOTNET_STYLE);

        if (objectIdProp == null)
          objectIdProp = entityType.GetProperty(DEFAULT_OBJECT_ID_PROPERTY_NAME_JAVA_STYLE);

        if (objectIdProp != null)
          return objectIdProp.GetValue(entity, null) as string;

        return "";
      }
      catch (System.Exception)
      {
      }

      return null;
    }

    private static void CheckEntityStructure<T>()
    {
      Type entityClass = typeof(T);

      if (entityClass.IsArray || entityClass.IsAssignableFrom(typeof(IEnumerable)))
        throw new ArgumentException(ExceptionMessage.WRONG_ENTITY_TYPE);

      try
      {
        entityClass.GetConstructor(new Type[0]);
      }
      catch (System.Exception)
      {
        throw new ArgumentException(ExceptionMessage.ENTITY_MISSING_DEFAULT_CONSTRUCTOR);
      }

      // OBJECTID
      PropertyInfo objectIdProp = entityClass.GetProperty(DEFAULT_OBJECT_ID_PROPERTY_NAME_DOTNET_STYLE);
      Type objectIdType = null;

      if (objectIdProp == null)
        objectIdProp = entityClass.GetProperty(DEFAULT_OBJECT_ID_PROPERTY_NAME_JAVA_STYLE);

      if (objectIdProp != null)
      {
        objectIdType = objectIdProp.PropertyType;
      }
      else
      {
        FieldInfo objectIdField = entityClass.GetField(DEFAULT_OBJECT_ID_PROPERTY_NAME_DOTNET_STYLE);

        if (objectIdField == null)
          objectIdField = entityClass.GetField(DEFAULT_OBJECT_ID_PROPERTY_NAME_JAVA_STYLE);

        if (objectIdField != null)
          objectIdType = objectIdField.FieldType;
      }

      if (objectIdType != null && objectIdType != typeof(string))
        throw new ArgumentException(ExceptionMessage.ENTITY_WRONG_OBJECT_ID_FIELD_TYPE);

      // CREATED FIELD/PROPERTY
      Type createdType = null;
      PropertyInfo createdProp = entityClass.GetProperty(DEFAULT_CREATED_FIELD_NAME_DOTNET_STYLE);

      if (createdProp == null)
        createdProp = entityClass.GetProperty(DEFAULT_CREATED_FIELD_NAME_JAVA_STYLE);

      if (createdProp != null)
      {
        createdType = createdProp.PropertyType;
      }
      else
      {
        FieldInfo createdField = entityClass.GetField(DEFAULT_CREATED_FIELD_NAME_DOTNET_STYLE, BindingFlags.Public | BindingFlags.Instance);

        if (createdField == null)
          createdField = entityClass.GetField(DEFAULT_CREATED_FIELD_NAME_JAVA_STYLE, BindingFlags.Instance | BindingFlags.Public);

        if (createdField != null)
          createdType = createdField.FieldType;
      }

      if (createdType != null && createdType != typeof(DateTime) && createdType != typeof(DateTime?))
        throw new ArgumentException(ExceptionMessage.ENTITY_WRONG_CREATED_FIELD_TYPE);


      // UPDATED FIELD/PROPERTY
      Type updatedType = null;
      PropertyInfo updatedProp = entityClass.GetProperty(DEFAULT_UPDATED_FIELD_NAME_DOTNET_STYLE);

      if (updatedProp == null)
        updatedProp = entityClass.GetProperty(DEFAULT_UPDATED_FIELD_NAME_JAVA_STYLE);

      if (updatedProp != null)
      {
        updatedType = updatedProp.PropertyType;
      }
      else
      {
        FieldInfo updatedField = entityClass.GetField(DEFAULT_UPDATED_FIELD_NAME_DOTNET_STYLE, BindingFlags.Public | BindingFlags.Instance);

        if (updatedField == null)
          updatedField = entityClass.GetField(DEFAULT_UPDATED_FIELD_NAME_JAVA_STYLE, BindingFlags.Instance | BindingFlags.Public);

        if (updatedField != null)
          updatedType = updatedField.FieldType;
      }

      if (updatedType != null && updatedType != typeof(DateTime) && updatedType != typeof(DateTime?))
        throw new ArgumentException(ExceptionMessage.ENTITY_WRONG_UPDATED_FIELD_TYPE);
    }

    private static void CheckPageSizeAndOffset(IBackendlessQuery dataQuery)
    {
      if (dataQuery == null)
        return;

      if (dataQuery.Offset < 0)
        throw new ArgumentException(ExceptionMessage.WRONG_OFFSET);

      if (dataQuery.PageSize < 0)
        throw new ArgumentException(ExceptionMessage.WRONG_PAGE_SIZE);
    }

    private static string GetTypeName(Type type)
    {
      if (type.Equals(typeof(BackendlessUser)))
        return "Users";
      else
        return type.Name;
    }

    private static void AddQuery(ref string query, string addQuery)
    {
      if (!string.IsNullOrEmpty(query))
        query += "&";
      query += addQuery;
    }

    private static string GetLoadRelationsQuery(IList<string> relations)
    {
      string query = null;
      if (relations != null)
      {
        string loadRelations = "";
        foreach (string relation in relations)
        {
          if (string.IsNullOrEmpty(loadRelations) == false)
            loadRelations += ",";
          loadRelations += relation;
        }
        if (string.IsNullOrEmpty(loadRelations) == false)
          query = "loadRelations=" + loadRelations;
      }
      return query;
    }

    private static string GetFindQuery(BackendlessDataQuery dataQuery)
    {
      string query = null;
      if (dataQuery != null)
      {
        query = "";

        QueryOptions queryOptions = dataQuery.QueryOptions;
        if (queryOptions != null)
        {
          AddQuery(ref query, "pageSize=" + queryOptions.PageSize);
          AddQuery(ref query, "offset=" + queryOptions.Offset);

          List<String> relatedList = queryOptions.Related;
          if (relatedList != null && relatedList.Count > 0)
          {
            string loadRelations = "";
            foreach (string relation in relatedList)
            {
              if (string.IsNullOrEmpty(loadRelations) == false)
                loadRelations += ",";
              loadRelations += relation;
            }
            if (string.IsNullOrEmpty(loadRelations) == false)
              AddQuery(ref query, "loadRelations=" + loadRelations);
          }

          List<string> sortByList = queryOptions.SortBy;
          if (sortByList != null && sortByList.Count > 0)
          {
            string sortBy = "";
            foreach (string sort in sortByList)
            {
              if (string.IsNullOrEmpty(sortBy) == false)
                sortBy += ",";
              sortBy += sort;
            }
            if (string.IsNullOrEmpty(sortBy) == false)
              AddQuery(ref query, "sortBy=" + sortBy);
          }
        }

        string whereClause = dataQuery.WhereClause;
        if (string.IsNullOrEmpty(whereClause) == false)
          AddQuery(ref query, "where=" + UnityEngine.WWW.EscapeURL(whereClause));

        List<string> propertiesList = dataQuery.Properties;
        if (propertiesList != null && propertiesList.Count > 0)
        {
          string props = "";
          foreach (string property in propertiesList)
          {
            if (string.IsNullOrEmpty(props) == false)
              props += ",";
            props += property;
          }
          if (string.IsNullOrEmpty(props) == false)
            AddQuery(ref query, "props=" + props);
        }
      }
      return query;
    }
  }
}