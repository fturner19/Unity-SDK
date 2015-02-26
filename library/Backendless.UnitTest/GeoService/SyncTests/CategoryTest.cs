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
using BackendlessAPI.Exception;
using BackendlessAPI.Geo;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BackendlessAPI.Test.GeoService.SyncTests
{
  [TestClass]
  public class CategoryTest: TestsFrame
  {
    [TestMethod]
  public void TestAddNullCategory() 
  {
    try
    {
      Backendless.Geo.AddCategory( null );

      Assert.Fail( "Client have send a null category" );
    }
    catch( System.Exception e )
    {
      CheckErrorCode( ExceptionMessage.NULL_CATEGORY_NAME, e );
    }
  }

  [TestMethod]
  public void TestDeleteNullCategory() 
  {
    try
    {
      Backendless.Geo.DeleteCategory( null );

      Assert.Fail( "Client have send a null category" );
    }
    catch( System.Exception e )
    {
      CheckErrorCode( ExceptionMessage.NULL_CATEGORY_NAME, e );
    }
  }

  [TestMethod]
  public void TestAddEmptyCategory() 
  {
    try
    {
      Backendless.Geo.AddCategory( "" );

      Assert.Fail( "Client have send an empty category" );
    }
    catch( System.Exception e )
    {
      CheckErrorCode( ExceptionMessage.NULL_CATEGORY_NAME, e );
    }
  }

  [TestMethod]
  public void TestDeleteEmptyCategory() 
  {
    try
    {
      Backendless.Geo.DeleteCategory( "" );

      Assert.Fail( "Client have send an empty category" );
    }
    catch( System.Exception e )
    {
      CheckErrorCode( ExceptionMessage.NULL_CATEGORY_NAME, e );
    }
  }

  [TestMethod]
  public void TestAddDefaultCategory() 
  {
    try
    {
      Backendless.Geo.AddCategory( DEFAULT_CATEGORY_NAME );

      Assert.Fail( "Client have send a default category" );
    }
    catch( System.Exception e )
    {
      CheckErrorCode( ExceptionMessage.DEFAULT_CATEGORY_NAME, e );
    }
  }

  [TestMethod]
  public void TestDeleteDefaultCategory() 
  {
    try
    {
      Backendless.Geo.DeleteCategory( DEFAULT_CATEGORY_NAME );

      Assert.Fail( "Client have send a default category" );
    }
    catch( System.Exception e )
    {
      CheckErrorCode( ExceptionMessage.DEFAULT_CATEGORY_NAME, e );
    }
  }

  [TestMethod]
  public void TestAddProperCategory() 
  {
    string categoryName = GetRandomCategory();
    GeoCategory geoCategory = Backendless.Geo.AddCategory( categoryName );

    checkCategory( categoryName, geoCategory );
  }

  [TestMethod]
  public void TestAddSameCategoryTwice() 
  {
    string categoryName = GetRandomCategory();
    Backendless.Geo.AddCategory( categoryName );
    GeoCategory geoCategory = Backendless.Geo.AddCategory( categoryName );

    checkCategory( categoryName, geoCategory );
  }

  [TestMethod]
  public void TestRemoveCategory() 
  {
    string categoryName = GetRandomCategory();
    Backendless.Geo.AddCategory( categoryName );
    Assert.IsTrue(Backendless.Geo.DeleteCategory(categoryName), "Server returned wrong status");
  }

  [TestMethod]
  public void TestRemoveUnexistingCategory() 
  {
    string categoryName = GetRandomCategory();

    try
    {
      Backendless.Geo.DeleteCategory( categoryName );

      Assert.Fail( "Server deleted not existing category" );
    }
    catch( System.Exception e )
    {
      CheckErrorCode( 4001, e );
    }
  }

  private void checkCategory( string categoryName, GeoCategory geoCategory )
  {
    Assert.IsNotNull(geoCategory, "Server returned a null category");
    Assert.AreEqual(categoryName, geoCategory.Name, "Server returned a category with a wrong name");
    Assert.IsNotNull(geoCategory.Id, "Server returned a category with a wrong id");
    Assert.IsTrue(geoCategory.Size == 0, "Server returned a category with a wrong size");
  }
  }
}
