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

using BackendlessAPI.LitJson;

namespace BackendlessAPI.Property
{
  public abstract class AbstractProperty
  {
    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("required")]
    public bool IsRequired { get; set; }

    [JsonProperty("selected")]
    public bool IsSelected { get; set; }

    [JsonProperty("type")]
    public DateTypeEnum Type { get; set; }

    [JsonProperty("defaultValue")]
    public object DefaultValue { get; set; }
  }
}
