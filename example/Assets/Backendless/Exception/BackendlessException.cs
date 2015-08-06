/*
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

using System;

namespace BackendlessAPI.Exception
{
  public class BackendlessException : System.Exception
  {
    private readonly BackendlessFault _backendlessFault;

    public BackendlessException(BackendlessFault backendlessFault)
    {
      _backendlessFault = backendlessFault;
    }

    public BackendlessException(String message)
    {
      _backendlessFault = new BackendlessFault(message);
    }

    public BackendlessFault BackendlessFault
    {
      get { return _backendlessFault; }
    }

    public string FaultCode
    {
      get { return _backendlessFault.FaultCode; }
    }

    public override string Message
    {
      get { return _backendlessFault.Message; }
    }

    public string Detail
    {
      get { return _backendlessFault.Detail; }
    }

    public override string ToString()
    {
      return String.Format("Error code: {0}, Message: {1}", _backendlessFault.FaultCode ?? "N/A", Message ?? "N/A");
    }
  }
}