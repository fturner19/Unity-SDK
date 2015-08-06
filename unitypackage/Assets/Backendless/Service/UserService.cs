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
using System.IO;
using System.Net;
using System.Text;
using BackendlessAPI.Async;
using BackendlessAPI.Engine;
using BackendlessAPI.Exception;
using BackendlessAPI.Property;

namespace BackendlessAPI.Service
{
  public class UserService
  {
    private BackendlessUser _currentUser;

    public BackendlessUser CurrentUser
    {
      get { return _currentUser; }
      set { _currentUser = value; }
    }

    static UserService()
    {
    }

    public BackendlessUser Register(BackendlessUser user)
    {
      CheckUserToBeProper(user, true);
      user.PutProperties(Invoker.InvokeSync<Dictionary<string, object>>(Invoker.Api.USERSERVICE_REGISTER, new object[] { user.Properties }));

      return user;
    }

    public void Register(BackendlessUser user, AsyncCallback<BackendlessUser> callback)
    {
      try
      {
        CheckUserToBeProper(user, true);

        var responder = new AsyncCallback<Dictionary<string, object>>(r =>
            {
              user.PutProperties(r);
              if (callback != null)
                callback.ResponseHandler.Invoke(user);
            }, f =>
                {
                  if (callback != null)
                    callback.ErrorHandler.Invoke(f);
                  else
                    throw new BackendlessException(f);
                });
        Invoker.InvokeAsync<Dictionary<string, object>>(Invoker.Api.USERSERVICE_REGISTER, new object[] { user.Properties }, responder);
      }
      catch (System.Exception ex)
      {
        if (callback != null)
          callback.ErrorHandler.Invoke(new BackendlessFault(ex.Message));
        else
          throw;
      }
    }

    public BackendlessUser Update(BackendlessUser user)
    {
      CheckUserToBeProper(user, false);

      if (string.IsNullOrEmpty(user.UserId))
        throw new ArgumentNullException(ExceptionMessage.WRONG_USER_ID);

      user.PutProperties(Invoker.InvokeSync<Dictionary<string, object>>(Invoker.Api.USERSERVICE_UPDATE, new object[] { user.Properties, user.UserId }));

      return user;
    }

    public void Update(BackendlessUser user, AsyncCallback<BackendlessUser> callback)
    {
      try
      {
        CheckUserToBeProper(user, false);

        if (string.IsNullOrEmpty(user.UserId))
          throw new ArgumentNullException(ExceptionMessage.WRONG_USER_ID);

        var responder = new AsyncCallback<Dictionary<string, object>>(r =>
            {
              user.PutProperties(r);
              if (callback != null)
                callback.ResponseHandler.Invoke(user);
            }, f =>
                {
                  if (callback != null)
                    callback.ErrorHandler.Invoke(f);
                  else
                    throw new BackendlessException(f);
                });

        Invoker.InvokeAsync<Dictionary<string, object>>(Invoker.Api.USERSERVICE_UPDATE, new object[] { user.Properties, user.UserId }, responder);
      }
      catch (System.Exception ex)
      {
        if (callback != null)
          callback.ErrorHandler.Invoke(new BackendlessFault(ex.Message));
        else
          throw;
      }
    }

    public BackendlessUser Login(string login, string password)
    {
      if (CurrentUser != null)
        Logout();

      if (string.IsNullOrEmpty(login))
        throw new ArgumentNullException(ExceptionMessage.NULL_LOGIN);

      if (string.IsNullOrEmpty(password))
        throw new ArgumentNullException(ExceptionMessage.NULL_PASSWORD);

      Dictionary<string, object> requestData = new Dictionary<string, object>();
      requestData.Add("login", login);
      requestData.Add("password", password);
      HandleUserLogin(Invoker.InvokeSync<Dictionary<string, object>>(Invoker.Api.USERSERVICE_LOGIN, new object[] { requestData }));

      return CurrentUser;
    }

    public void Login(string login, string password, AsyncCallback<BackendlessUser> callback)
    {
      try
      {
        if (string.IsNullOrEmpty(login))
          throw new ArgumentNullException(ExceptionMessage.NULL_LOGIN);

        if (string.IsNullOrEmpty(password))
          throw new ArgumentNullException(ExceptionMessage.NULL_PASSWORD);

        Dictionary<string, object> requestData = new Dictionary<string, object>();
        requestData.Add("login", login);
        requestData.Add("password", password);

        Invoker.InvokeAsync<Dictionary<string, object>>(Invoker.Api.USERSERVICE_LOGIN, new object[] { requestData }, GetUserLoginAsyncHandler(callback));
      }
      catch (System.Exception ex)
      {
        if (callback != null)
          callback.ErrorHandler.Invoke(new BackendlessFault(ex.Message));
        else
          throw;
      }
    }

    public void Logout()
    {
      try
      {
        Invoker.InvokeSync<object>(Invoker.Api.USERSERVICE_LOGOUT, null);
      }
      catch (BackendlessException exception)
      {
        BackendlessFault fault = exception.BackendlessFault;

        if (fault != null)
        {
          int faultCode = int.Parse(fault.FaultCode);

          if (faultCode != 3064 && faultCode != 3091 && faultCode != 3090 && faultCode != 3023)
            throw exception;
        }
      }

      CurrentUser = null;
      HeadersManager.GetInstance().RemoveHeader(HeadersEnum.USER_TOKEN_KEY);
    }

    public void Logout(AsyncCallback<object> callback)
    {
      var responder = new AsyncCallback<object>(r =>
          {
            CurrentUser = null;
            HeadersManager.GetInstance().RemoveHeader(HeadersEnum.USER_TOKEN_KEY);

            if (callback != null)
              callback.ResponseHandler.Invoke(null);
          }, f =>
              {
                if (callback != null)
                  callback.ErrorHandler.Invoke(f);
                else
                  throw new BackendlessException(f);
              });
      Invoker.InvokeAsync<object>(Invoker.Api.USERSERVICE_LOGOUT, null, responder);
    }

    public void RestorePassword(string identity)
    {
      if (String.IsNullOrEmpty(identity))
        throw new ArgumentNullException(ExceptionMessage.NULL_IDENTITY);

      Invoker.InvokeSync<object>(Invoker.Api.USERSERVICE_RESTOREPASSWORD, new Object[] { null, identity });
    }

    public void RestorePassword(string identity, AsyncCallback<object> callback)
    {
      try
      {
        if (String.IsNullOrEmpty(identity))
          throw new ArgumentNullException(ExceptionMessage.NULL_IDENTITY);

        Invoker.InvokeAsync<object>(Invoker.Api.USERSERVICE_RESTOREPASSWORD, new Object[] { null, identity }, callback);
      }
      catch (System.Exception ex)
      {
        if (callback != null)
          callback.ErrorHandler.Invoke(new BackendlessFault(ex.Message));
        else
          throw;
      }
    }

    public IList<string> GetUserRoles()
    {
      return Invoker.InvokeSync<IList<string>>(Invoker.Api.USERSERVICE_GETUSERROLES, null);
    }

    public void GetUserRoles(AsyncCallback<IList<string>> callback)
    {
      try
      {
        Invoker.InvokeAsync<IList<string>>(Invoker.Api.USERSERVICE_GETUSERROLES, null, callback);
      }
      catch (System.Exception ex)
      {
        if (callback != null)
          callback.ErrorHandler.Invoke(new BackendlessFault(ex.Message));
        else
          throw;
      }
    }

    public List<UserProperty> DescribeUserClass()
    {
      List<UserProperty> result = Invoker.InvokeSync<List<UserProperty>>(Invoker.Api.USERSERVICE_DESCRIBEUSERCLASS, null);

      return result;
    }

    public void DescribeUserClass(AsyncCallback<List<UserProperty>> callback)
    {
      Invoker.InvokeAsync<List<UserProperty>>(Invoker.Api.USERSERVICE_DESCRIBEUSERCLASS, null, callback);
    }

    private static void CheckUserToBeProper(BackendlessUser user, bool passwordCheck)
    {
      if (user == null)
        throw new ArgumentNullException(ExceptionMessage.NULL_USER);

      if (passwordCheck && string.IsNullOrEmpty(user.Password))
        throw new ArgumentNullException(ExceptionMessage.NULL_PASSWORD);
    }

    private void HandleUserLogin(Dictionary<string, object> invokeResult)
    {
      HeadersManager.GetInstance()
                    .AddHeader(HeadersEnum.USER_TOKEN_KEY,
                               invokeResult[HeadersEnum.USER_TOKEN_KEY.Header].ToString());

      if (CurrentUser == null)
        CurrentUser = new BackendlessUser();

      CurrentUser.PutProperties(invokeResult);
    }

    private AsyncCallback<Dictionary<string, object>> GetUserLoginAsyncHandler(
        AsyncCallback<BackendlessUser> callback)
    {
      return new AsyncCallback<Dictionary<string, object>>(r =>
          {
            HeadersManager.GetInstance()
                          .AddHeader(HeadersEnum.USER_TOKEN_KEY, r[HeadersEnum.USER_TOKEN_KEY.Header].ToString());

            if (CurrentUser == null)
              CurrentUser = new BackendlessUser();

            CurrentUser.PutProperties(r);

            if (callback != null)
              callback.ResponseHandler.Invoke(CurrentUser);
          }, f =>
              {
                if (callback != null)
                  callback.ErrorHandler.Invoke(f);
                else
                  throw new BackendlessException(f);
              });
    }
  }
}