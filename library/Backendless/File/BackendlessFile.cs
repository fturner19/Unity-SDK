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
using BackendlessAPI.LitJson;

namespace BackendlessAPI.File
{
  public class BackendlessFile
  {
    public BackendlessFile( string fileURL )
    {
      FileURL = fileURL;
    }

    [JsonProperty("fileURL")]
    public string FileURL { get; set; }

    public void Remove()
    {
      Backendless.Files.Remove( FileURL );
    }

    public void Remove( AsyncCallback<object> callback )
    {
      Backendless.Files.Remove( FileURL, callback );
    }
  }
}