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

using BackendlessAPI.Async;
using BackendlessAPI.Engine;
using BackendlessAPI.Exception;
using BackendlessAPI.File;
using System;
using System.IO;
using System.Net;
using System.Text;
using System.Collections.Generic;
using BackendlessAPI.LitJson;

namespace BackendlessAPI.Service
{
  public class FileService
  {
    private const int UPLOAD_BUFFER_DEFAULT_LENGTH = 8192;

    public FileService()
    {
    }

    public void Upload( Stream stream, string remotePath, AsyncCallback<BackendlessFile> callback )
    {
      Upload(stream, remotePath, new EmptyUploadCallback(), callback);
    }

    public void Upload(Stream stream, string remotePath, UploadCallback uploadCallback, AsyncCallback<BackendlessFile> callback)
    {
      if( string.IsNullOrEmpty( remotePath ) )
        throw new ArgumentNullException( ExceptionMessage.NULL_PATH );

      if (stream == null)
        throw new ArgumentNullException( ExceptionMessage.NULL_FILE );

      MakeFileUpload(stream, remotePath, uploadCallback, callback);
    }

    public void Remove( string fileUrl )
    {
      if( string.IsNullOrEmpty( fileUrl ) )
        throw new ArgumentNullException( ExceptionMessage.NULL_PATH );

      Invoker.InvokeSync<object>( Invoker.Api.FILESERVICE_REMOVE, new Object[] {null, fileUrl} );
    }

    public void Remove( string fileUrl, AsyncCallback<object> callback )
    {
      if( string.IsNullOrEmpty( fileUrl ) )
        throw new ArgumentNullException( ExceptionMessage.NULL_PATH );

      Invoker.InvokeAsync<object>(Invoker.Api.FILESERVICE_REMOVE, new Object[] { null, fileUrl }, callback);
    }

    public void RemoveDirectory( string directoryPath )
    {
      if( string.IsNullOrEmpty( directoryPath ) )
        throw new ArgumentNullException( ExceptionMessage.NULL_PATH );

      Invoker.InvokeSync<object>(Invoker.Api.FILESERVICE_REMOVE, new Object[] { null, directoryPath });
    }

    public void RemoveDirectory( string directoryPath, AsyncCallback<object> callback )
    {
      if( string.IsNullOrEmpty( directoryPath ) )
        throw new ArgumentNullException( ExceptionMessage.NULL_PATH );

      Invoker.InvokeAsync<object>(Invoker.Api.FILESERVICE_REMOVE, new Object[] { null, directoryPath }, callback);
    }

    private void MakeFileUpload( Stream stream, string path, UploadCallback uploadCallback,
                                 AsyncCallback<BackendlessFile> callback )
    {
      string boundary = DateTime.Now.Ticks.ToString( "x" );
      byte[] boundaryBytes = Encoding.UTF8.GetBytes( "\r\n--" + boundary + "--\r\n" );

      var fileName = "";
      try
      {
        fileName = Path.GetFileName(path);
      }
      catch ( System.Exception ex ) {
        if (callback != null)
          callback.ErrorHandler.Invoke(new BackendlessFault(ex.Message));
        else
          throw;
      }

      var sb = new StringBuilder();
      sb.Append( "--" );
      sb.Append( boundary );
      sb.Append( "\r\n" );
      sb.Append( "Content-Disposition: form-data; name=\"file\"; filename=\"" );
      sb.Append(fileName);
      sb.Append( "\"" );
      sb.Append( "\r\n" );
      sb.Append( "Content-Type: " );
      sb.Append( "application/octet-stream" );
      sb.Append( "\r\n" );
      sb.Append( "Content-Transfer-Encoding: binary" );
      sb.Append( "\r\n" );
      sb.Append( "\r\n" );

      string header = sb.ToString();
      byte[] headerBytes = Encoding.UTF8.GetBytes( header );

      var httpRequest =
        (HttpWebRequest)
        WebRequest.Create(
          new Uri(
            Backendless.Url + "/" + Backendless.VersionNum + "/files/" + EncodeURL( path ),
            UriKind.Absolute ) );

      httpRequest.ContentType = "multipart/form-data; boundary=" + boundary;
      httpRequest.Method = "POST";
      httpRequest.Headers["KeepAlive"] = "true";

      foreach( var h in HeadersManager.GetInstance().Headers )
        httpRequest.Headers[h.Key] = h.Value;

      try
      {
        var async = new RequestStreamAsyncState<BackendlessFile>
          {
            Callback = callback,
            UploadCallback = uploadCallback,
            HttpRequest = httpRequest,
            HeaderBytes = headerBytes,
            BoundaryBytes = boundaryBytes,
            Stream = stream
          };
        httpRequest.BeginGetRequestStream( RequestStreamCallback, async );
      }
      catch( System.Exception ex )
      {
        if( callback != null )
          callback.ErrorHandler.Invoke( new BackendlessFault( ex.Message ) );
        else
          throw;
      }
    }

    private static void RequestStreamCallback( IAsyncResult asyncResult )
    {
      var asyncState = (RequestStreamAsyncState<BackendlessFile>) asyncResult.AsyncState;
      try
      {
        var stream = asyncState.Stream;
        var httpRequest = asyncState.HttpRequest;
        var uploadCallback = asyncState.UploadCallback;
        using( var postStream = httpRequest.EndGetRequestStream( asyncResult ) )
        {
          var headerBytes = asyncState.HeaderBytes;
          byte[] boundaryBytes = asyncState.BoundaryBytes;
          long length = stream.Length;
          long offset = 0;
          int bufferLen = (int)(length < UPLOAD_BUFFER_DEFAULT_LENGTH ? length : UPLOAD_BUFFER_DEFAULT_LENGTH);
          byte[] buffer = new byte[bufferLen];

          // send the headers
          postStream.Write( headerBytes, 0, headerBytes.Length );

          int progress = 0;
          stream.Seek(0, SeekOrigin.Begin);
          int size = stream.Read( buffer, 0, bufferLen );
          while( size > 0 )
          {
            postStream.Write( buffer, 0, size );
            offset += size;

            if( !(uploadCallback is EmptyUploadCallback) )
            {
              int currentProgress = (int)(((float)offset / length) * 100);
              if( progress != currentProgress )
              {
                progress = currentProgress;
                uploadCallback.ProgressHandler.Invoke( progress );
              }
            }
            size = stream.Read( buffer, 0, bufferLen );
          }

          if( !(uploadCallback is EmptyUploadCallback) && progress != 100 )
            uploadCallback.ProgressHandler.Invoke( 100 );

          postStream.Write( boundaryBytes, 0, boundaryBytes.Length );
        }

        httpRequest.BeginGetResponse( ResponseCallback, asyncState );
      }
      catch( System.Exception ex )
      {
        if( asyncState.Callback != null )
          asyncState.Callback.ErrorHandler.Invoke( new BackendlessFault( ex.Message ) );
        else
          throw;
      }
    }

    private static void ResponseCallback( IAsyncResult asyncResult )
    {
      var asyncState = (RequestStreamAsyncState<BackendlessFile>) asyncResult.AsyncState;
      using( asyncState.Stream )
      {
        try
        {
          using( var response = asyncState.HttpRequest.EndGetResponse( asyncResult ).GetResponseStream() )
          {
            var encode = System.Text.Encoding.GetEncoding( "utf-8" );
            var result = new StreamReader( response, encode ).ReadToEnd();
            string fileUrl = "";
            try
            {
              JsonData successResponse = JsonMapper.ToObject(result);
              fileUrl = (string)successResponse["fileURL"];
            }
            catch (System.Exception)
            {
            }
            asyncState.Callback.ResponseHandler.Invoke( new BackendlessFile( fileUrl ) );
          }
        }
        catch( WebException ex )
        {
          var response = new StreamReader( ex.Response.GetResponseStream() ).ReadToEnd();
          BackendlessFault fault = null;
          try
          {
            JsonData errorResponse = JsonMapper.ToObject(response);
            int code = (int)errorResponse["code"];
            string message = (string)errorResponse["message"];

            fault = new BackendlessFault(code.ToString(), message, null);
          }
          catch (System.Exception)
          {
            fault = new BackendlessFault(ex);
          }

          if( asyncState.Callback != null )
            asyncState.Callback.ErrorHandler.Invoke( fault );
          else
            throw new BackendlessException( fault );
        }
      }
    }

#if SILVERLIGHT || WINDOWS_PHONE
      private String EncodeURL( string urlStr ) 
  {
    var splitedStr = urlStr.Split( '/' );
    var result = "";

    for( var i = 0; i < splitedStr.Length; i++ )
    {
      if( i != 0 )
        result += "/";

      result += System.Net.HttpUtility.UrlEncode(splitedStr[i]);
    }

    return result;
  }
#else
    private String EncodeURL( string urlStr )
    {
      var splitedStr = urlStr.Split( '/' );
      var result = "";

      for( var i = 0; i < splitedStr.Length; i++ )
      {
        if( i != 0 )
          result += "/";

        result += UnityEngine.WWW.EscapeURL(splitedStr[i]);
      }

      return result;
    }
#endif
  }

  internal class EmptyUploadCallback : UploadCallback
  {
    public EmptyUploadCallback() : base( response =>
      {
        /*A stub. Needed for handy methods.*/
      } )
    {
    }
  }

  internal class RequestStreamAsyncState<T>
  {
    public UploadCallback UploadCallback;
    public HttpWebRequest HttpRequest;
    public Stream Stream;
    public byte[] BoundaryBytes;
    public byte[] HeaderBytes;
    public AsyncCallback<T> Callback;
  }
}