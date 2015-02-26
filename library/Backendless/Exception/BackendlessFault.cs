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

namespace BackendlessAPI.Exception
{
  public class BackendlessFault
  {
    private readonly string _faultCode;
    private readonly string _message;
    private readonly string _detail;

    internal BackendlessFault(string message)
    {
      _message = message;
    }

    internal BackendlessFault(BackendlessException backendlessException)
    {
      _faultCode = backendlessException.FaultCode;
      _message = backendlessException.Message;
      _detail = backendlessException.Detail;
    }

    internal BackendlessFault(string faultCode, string message, string detail)
    {
      _faultCode = faultCode;
      _message = message;
      _detail = detail;
    }

    internal BackendlessFault(System.Exception ex)
    {
      _faultCode = ex.GetType().Name;
      _message = ex.Message;
      _detail = ex.StackTrace;
    }

    public string Detail
    {
      get { return _detail; }
    }

    public string FaultCode
    {
      get { return _faultCode; }
    }

    public string Message
    {
      get { return _message; }
    }

    public override string ToString()
    {
      return String.Format("Backendless BackendlessFault. Code: {0}, Message: {1}", FaultCode ?? "N/A",
                            Message ?? "N/A");
    }
  }
}