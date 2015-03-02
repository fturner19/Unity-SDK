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
using BackendlessAPI.Data;
using BackendlessAPI.LitJson;

namespace BackendlessAPI.Geo
{
  public class BackendlessGeoQuery : IBackendlessQuery
  {
    private List<string> _categories;
    private bool _includeMeta = true;
    private Dictionary<string, string> _metadata = new Dictionary<string, string>();
    private Double _latitude = Double.NaN;
    private Double _longitude = Double.NaN;
    private Double _radius = Double.NaN;
    private Units? _units;
    private double[] _searchRectangle;
    private int _pageSize;
    private int _offset;
    private Dictionary<string, string> _relativeFindMetadata = new Dictionary<string, string>();
    private double _relativeFindPercentThreshold = 0;

    public BackendlessGeoQuery()
    {
      PageSize = 100;
    }

    public BackendlessGeoQuery(double latitude, double longitude, int pageSize, int offset)
    {
      Latitude = latitude;
      Longitude = longitude;
      PageSize = pageSize;
      Offset = offset;
    }

    public BackendlessGeoQuery(List<string> categories)
    {
      Categories = categories;
    }

    public BackendlessGeoQuery(double latitude, double longitude, double radius, Units units)
    {
      Latitude = latitude;
      Longitude = longitude;
      Radius = radius;
      Units = units;
      PageSize = 100;
    }

    public BackendlessGeoQuery(double latitude, double longitude, double radius, Units units,
                               List<string> categories)
    {
      Latitude = latitude;
      Longitude = longitude;
      Radius = radius;
      Units = units;
      Categories = categories;
      PageSize = 100;
    }

    public BackendlessGeoQuery(double latitude, double longitude, double radius, Units units,
                               List<string> categories,
                               Dictionary<string, string> metadata)
    {
      Latitude = latitude;
      Longitude = longitude;
      Radius = radius;
      Units = units;
      Categories = categories;
      PageSize = 100;
      Metadata = metadata;

      if (metadata != null)
        IncludeMeta = true;
    }

    public BackendlessGeoQuery(double NWLat, double NWLon, double SELat, double SWLon)
    {
      SearchRectangle = new[] { NWLat, NWLon, SELat, SWLon };
      PageSize = 100;
    }

    public BackendlessGeoQuery(double NWLat, double NWLon, double SELat, double SWLon, Units units,
                               List<string> categories)
    {
      SearchRectangle = new[] { NWLat, NWLon, SELat, SWLon };
      Units = units;
      Categories = categories;
      PageSize = 100;
    }

    public BackendlessGeoQuery(Dictionary<string, string> metadata)
    {
      PageSize = 100;
      Metadata = metadata;

      if (metadata != null)
        IncludeMeta = true;
    }

    [JsonProperty("latitude")]
    public double Latitude
    {
      get { return _latitude; }
      set { _latitude = value; }
    }

    [JsonProperty("longitude")]
    public double Longitude
    {
      get { return _longitude; }
      set { _longitude = value; }
    }

    [JsonProperty("radius")]
    public double Radius
    {
      get { return _radius; }
      set { _radius = value; }
    }

    [JsonProperty("units")]
    public Units? Units
    {
      get { return _units; }
      set { _units = value; }
    }

    [JsonProperty("categories")]
    public List<string> Categories
    {
      get { return _categories ?? (_categories = new List<string>()); }
      set { _categories = value; }
    }

    [JsonProperty("metadata")]
    public Dictionary<string, string> Metadata
    {
      get { return _metadata; }
      set { _metadata = value; }
    }

    [JsonProperty("includeMeta")]
    public bool IncludeMeta
    {
      get { return _includeMeta; }
      set { _includeMeta = value; }
    }

    [JsonProperty("searchRectangle")]
    public double[] SearchRectangle
    {
      get { return _searchRectangle; }
      set { _searchRectangle = value; }
    }

    [JsonProperty("pageSize")]
    public int PageSize
    {
      get { return _pageSize; }
      set { _pageSize = value; }
    }

    [JsonProperty("offset")]
    public int Offset
    {
      get { return _offset; }
      set { _offset = value; }
    }

    [JsonProperty("relativeFindMetadata")]
    public Dictionary<string, string> RelativeFindMetadata
    {
      get { return _relativeFindMetadata; }
      set { _relativeFindMetadata = value; }
    }

    [JsonProperty("relativeFindPercentThreshold")]
    public double RelativeFindPercentThreshold
    {
      get { return _relativeFindPercentThreshold; }
      set { _relativeFindPercentThreshold = value; }
    }

    public void SetSearchRectangle(GeoPoint topLeft, GeoPoint bottomRight)
    {
      _searchRectangle = new[] { topLeft.Latitude, topLeft.Longitude, bottomRight.Latitude, bottomRight.Longitude };
    }

    public IBackendlessQuery NewInstance()
    {
      return new BackendlessGeoQuery
          {
            Latitude = Latitude,
            Longitude = Longitude,
            Radius = Radius,
            Units = Units,
            Categories = Categories,
            IncludeMeta = IncludeMeta,
            Metadata = Metadata,
            SearchRectangle = SearchRectangle,
            PageSize = PageSize,
            Offset = Offset,
            RelativeFindMetadata = RelativeFindMetadata,
            RelativeFindPercentThreshold = RelativeFindPercentThreshold,
          };
    }
  }
}