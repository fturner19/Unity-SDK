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
using BackendlessAPI.Async;
using BackendlessAPI.Exception;
using BackendlessAPI.Geo;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BackendlessAPI.Test.GeoService.AsyncTests
{
  [TestClass]
  public class CategoryTest : TestsFrame
  {
    [TestMethod]
    public void TestAddNullCategory()
    {
      RunAndAwait(
        () =>
        Backendless.Geo.AddCategory( null,
                                     new AsyncCallback<GeoCategory>(
                                       response => FailCountDownWith( "Client have send a null category" ),
                                       fault => CheckErrorCode( ExceptionMessage.NULL_CATEGORY_NAME, fault ) ) ) );
    }

    [TestMethod]
    public void TestDeleteNullCategory()
    {
      RunAndAwait(
        () =>
        Backendless.Geo.DeleteCategory( null,
                                        new AsyncCallback<Boolean>(
                                          response => FailCountDownWith( "Client have send a null category" ),
                                          fault => CheckErrorCode( ExceptionMessage.NULL_CATEGORY_NAME, fault ) ) ) );
    }

    [TestMethod]
    public void TestAddEmptyCategory()
    {
      RunAndAwait(
        () =>
        Backendless.Geo.AddCategory( "",
                                     new AsyncCallback<GeoCategory>(
                                       response => FailCountDownWith( "Client have send an empty category" ),
                                       fault => CheckErrorCode( ExceptionMessage.NULL_CATEGORY_NAME, fault ) ) ) );
    }

    [TestMethod]
    public void TestDeleteEmptyCategory()
    {
      RunAndAwait(
        () =>
        Backendless.Geo.DeleteCategory( "",
                                        new AsyncCallback<Boolean>(
                                          response => FailCountDownWith( "Client have send an empty category" ),
                                          fault => CheckErrorCode( ExceptionMessage.NULL_CATEGORY_NAME, fault ) ) ) );
    }

    [TestMethod]
    public void TestAddDefaultCategory()
    {
      RunAndAwait(
        () =>
        Backendless.Geo.AddCategory( DEFAULT_CATEGORY_NAME,
                                     new AsyncCallback<GeoCategory>(
                                       response => FailCountDownWith( "Client have send a default category" ),
                                       fault => CheckErrorCode( ExceptionMessage.DEFAULT_CATEGORY_NAME, fault ) ) ) );
    }

    [TestMethod]
    public void TestDeleteDefaultCategory()
    {
      RunAndAwait(
        () =>
        Backendless.Geo.DeleteCategory( DEFAULT_CATEGORY_NAME,
                                        new AsyncCallback<Boolean>(
                                          response => FailCountDownWith( "Client have send a default category" ),
                                          fault => CheckErrorCode( ExceptionMessage.DEFAULT_CATEGORY_NAME, fault ) ) ) );
    }

    [TestMethod]
    public void TestAddProperCategory()
    {
      string categoryName = GetRandomCategory();
      RunAndAwait(
        () =>
        Backendless.Geo.AddCategory( categoryName,
                                     new ResponseCallback<GeoCategory>( this )
                                       {
                                         ResponseHandler = geoCategory =>
                                           {
                                             Assert.IsNotNull( geoCategory, "Server returned a null category" );
                                             Assert.AreEqual( categoryName, geoCategory.Name,
                                                              "Server returned a category with a wrong name" );
                                             Assert.IsNotNull( geoCategory.Id, "Server returned a category with a null id" );
                                             Assert.IsTrue( geoCategory.Size == 0,
                                                            "Server returned a category with a wrong size" );
                                             CountDown();
                                           }
                                       } ) );
    }

    [TestMethod]
    public void TestAddSameCategoryTwice()
    {
      string categoryName = GetRandomCategory();
      RunAndAwait(
        () =>
        Backendless.Geo.AddCategory( categoryName,
                                     new ResponseCallback<GeoCategory>( this )
                                       {
                                         ResponseHandler =
                                           response =>
                                           Backendless.Geo.AddCategory( categoryName,
                                                                        new ResponseCallback<GeoCategory>( this )
                                                                          {
                                                                            ResponseHandler = geoCategory =>
                                                                              {
                                                                                Assert.IsNotNull( geoCategory,
                                                                                                  "Server returned a null category" );
                                                                                Assert.AreEqual( categoryName,
                                                                                                 geoCategory.Name,
                                                                                                 "Server returned a category with a wrong name" );
                                                                                Assert.IsNotNull( geoCategory.Id,
                                                                                                  "Server returned a category with a null id" );
                                                                                Assert.IsTrue( geoCategory.Size == 0,
                                                                                               "Server returned a category with a wrong size" );
                                                                                CountDown();
                                                                              }
                                                                          } )
                                       } ) );
    }

    [TestMethod]
    public void TestRemoveCategory()
    {
      string categoryName = GetRandomCategory();
      RunAndAwait(
        () =>
        Backendless.Geo.AddCategory( categoryName,
                                     new ResponseCallback<GeoCategory>( this )
                                       {
                                         ResponseHandler =
                                           geoCategory =>
                                           Backendless.Geo.DeleteCategory( categoryName,
                                                                           new ResponseCallback<Boolean>( this )
                                                                             {
                                                                               ResponseHandler = response =>
                                                                                 {
                                                                                   Assert.IsTrue( response,
                                                                                                  "Server returned wrong status" );
                                                                                   CountDown();
                                                                                 }
                                                                             } )
                                       } ) );
    }

    [TestMethod]
    public void TestRemoveUnexistingCategory()
    {
      string categoryName = GetRandomCategory();

      RunAndAwait(
        () =>
        Backendless.Geo.DeleteCategory( categoryName,
                                        new AsyncCallback<bool>(
                                          response => FailCountDownWith( "Server deleted unexisting category" ),
                                          fault => CheckErrorCode( 4001, fault ) ) ) );
    }
  }
}