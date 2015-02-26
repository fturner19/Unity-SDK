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
using BackendlessAPI.LitJson;

namespace BackendlessAPI.Geo
{
  public class GeoPoint
  {
    private List<string> _categories;
    private Dictionary<string, object> _metadata;

    public GeoPoint()
    {
    }

    public GeoPoint(double latitude, double longitude)
    {
      Latitude = latitude;
      Longitude = longitude;
    }

    public GeoPoint(double latitude, double longitude, List<string> categories, Dictionary<string, string> metadata)
    {
      Latitude = latitude;
      Longitude = longitude;
      Categories = categories;

      foreach (KeyValuePair<string, string> keyValue in metadata)
        Metadata.Add(keyValue.Key, keyValue.Value);
    }

    public GeoPoint(double latitude, double longitude, List<string> categories, Dictionary<string, object> metadata)
    {
      Latitude = latitude;
      Longitude = longitude;
      Categories = categories;
      Metadata = metadata;
    }

    [JsonProperty("objectId")]
    public string ObjectId { get; set; }

    [JsonProperty("latitude")]
    public double Latitude { get; set; }

    [JsonProperty("longitude")]
    public double Longitude { get; set; }

    [JsonProperty("distance")]
    public double Distance { get; set; }

    [JsonProperty("categories")]
    public List<string> Categories
    {
      get { return _categories ?? (_categories = new List<string>()); }
      set { _categories = value; }
    }

    [JsonProperty("metadata")]
    public Dictionary<string, object> Metadata
    {
      get { return _metadata ?? (_metadata = new Dictionary<string, object>()); }
      set { _metadata = value; }
    }
  }
}