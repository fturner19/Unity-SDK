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

namespace BackendlessAPI.Logging
{
  class LogMessage
  {
    internal LogMessage( DateTime timestamp, String message, String exception )
    {
      this.timestamp = timestamp;
      this.message = message;
      this.exception = exception;
    }

    public DateTime timestamp { get; set; }
    public String message { get; set; }
    public String exception { get; set; }
  }
}
