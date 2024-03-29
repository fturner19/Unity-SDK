﻿/*
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
using System.Collections.Generic;
using Backendless.Test;
using BackendlessAPI.Data;
using BackendlessAPI.Geo;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BackendlessAPI.Test.GeoService.AsyncTests
{
  [TestClass]
  public class TestsFrame : IAsyncTest
  {
    public const string DEFAULT_CATEGORY_NAME = "Default";
    public const double METER = 0.00001d;
    public static readonly List<GeoPoint> usedGeoPoints = new List<GeoPoint>();

    public Random Random = new Random();

    public const string META_KEY = "meta_key";
    public const string META_VALUE = "meta_value";

    private List<string> definedCategories = new List<string>();
    private string definedCategory;

    public void SetDefinedCategory( string category )
    {
      definedCategory = "sync_" + category;
      definedCategories.Add( category );
    }

    public string GetDefinedCategory()
    {
      return definedCategory;
    }

    public List<string> GetDefinedCategories()
    {
      return definedCategories;
    }

    public void SetDefinedCategories( List<string> definedCategories )
    {
      this.definedCategories = definedCategories;
    }

    public string GetRandomCategory()
    {
      string timestamp = (DateTime.Now.Ticks + Random.Next()).ToString();
      return "test_category_" + timestamp;
    }

    public List<string> GetRandomCategoriesList( int size )
    {
      var result = new List<string>();

      for( int i = 0; i < size; i++ )
        result.Add( GetRandomCategory() + i );

      return result;
    }

    public Dictionary<string, string> GetRandomSimpleMetadata()
    {
      string timestamp = (DateTime.Now.Ticks + Random.Next()).ToString();

      return new Dictionary<string, string> {{"metadata_key" + timestamp, "metadata_value" + timestamp}};
    }

    public Dictionary<string, string> GetRandomMetadata()
    {
      string timestamp = (DateTime.Now.Ticks + Random.Next()).ToString();

      var result = new Dictionary<string, string>
        {
          {META_KEY, "metadata_key" + timestamp},
          {META_VALUE, "metadata_value" + timestamp}
        };

      return result;
    }

    public void GetCollectionAndCheck( double startingLat, double startingLong, int maxPoints, double offset,
                                       Dictionary<string, string> meta, BackendlessGeoQuery geoQuery )
    {
      int counter = maxPoints;
      if( geoQuery.Categories.Count == 0 && GetDefinedCategories() != null )
        geoQuery.Categories = GetDefinedCategories();

      Backendless.Geo.GetPoints( geoQuery,
                                 new ResponseCallback<BackendlessCollection<GeoPoint>>( this )
                                   {
                                     ResponseHandler = geoPointBackendlessCollection =>
                                       {
                                         Assert.IsNotNull( geoPointBackendlessCollection,
                                                           "Server returned a null collection" );

                                         foreach( GeoPoint geoPoint in geoPointBackendlessCollection.GetCurrentPage() )
                                         {
                                           if( meta == null || meta.Count == 0 )
                                           {
                                             Assert.IsTrue( geoPoint.Metadata.Count == 0,
                                                            "Server returned points with unexpected metadata" );
                                           }
                                           else
                                           {
                                             foreach( KeyValuePair<string, string> keyValuePair in meta )
                                             {
                                               Assert.IsTrue( geoPoint.Metadata.ContainsKey( keyValuePair.Key ),
                                                              "Server returned points with unexpected metadata" );
                                               Assert.IsTrue(
                                                 geoPoint.Metadata[keyValuePair.Key].Equals( keyValuePair.Value ),
                                                 "Server returned points with unexpected metadata" );
                                             }
                                           }

                                           Assert.AreEqual( startingLat, geoPoint.Latitude, 0.0000000001d,
                                                            "Server returned points from unexpected latitude range" );
                                           Assert.IsTrue(
                                             geoPoint.Longitude >= startingLong &&
                                             geoPoint.Longitude <= startingLong + offset,
                                             "Server returned points from unexpected longitude range" );

                                           counter--;
                                         }

                                         Assert.AreEqual( counter, 0, "Server found wrong total points count" );
                                         CountDown();
                                       }
                                   } );
    }

    [TestInitialize]
    public void SetUp()
    {
      Backendless.InitApp( Defaults.TEST_APP_ID, Defaults.TEST_SECRET_KEY, Defaults.TEST_VERSION );
      definedCategories.Clear();
      definedCategory = null;
    }
  }
}