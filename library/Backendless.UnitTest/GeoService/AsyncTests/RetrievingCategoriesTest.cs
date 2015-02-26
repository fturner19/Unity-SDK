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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BackendlessAPI.Geo;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BackendlessAPI.Test.GeoService.AsyncTests
{
  [TestClass]
  public class RetrievingCategoriesTest: TestsFrame
  {
    [TestMethod]
  public void TestRetrieveCategoriesList() 
  {
      RunAndAwait( () => Backendless.Geo.GetCategories(new ResponseCallback<List<GeoCategory>>( this )
        {
          ResponseHandler = geoCategories =>
            {
              Assert.IsNotNull("Server returned a null list");
              Assert.IsTrue(geoCategories.Count != 0, "Server returned an empty list");

              foreach (GeoCategory geoCategory in geoCategories)
              {
                Assert.IsNotNull(geoCategory.Id, "Server returned a category with null id");
                Assert.IsNotNull(geoCategory.Name, "Server returned a category with null name");
              }

              CountDown();
            }
        }) );
  }
  }
}
